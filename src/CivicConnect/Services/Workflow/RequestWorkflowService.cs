namespace CivicConnect.Services
{
    /// <summary>OWNER: Enrique de Sousa. The only place a request's Status or AssignedStaffId may change.</summary>
    public class RequestWorkflowService : IRequestWorkflowService
    {
        private const int MaxNoteLength = 2000;
        private const string ConflictMessage = "Someone else changed this request. Reload and try again.";

        private readonly IRequestRepository _requests;
        private readonly IUserRepository _users;
        private readonly IAuditService _audit;
        private readonly INotificationService _notifications;
        private readonly IAuthorisationService _authorisation;
        private readonly TransitionGuard _guard;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IClock _clock;
        private readonly IAuditLogRepository _auditLog;

        public RequestWorkflowService(
            IRequestRepository requests,
            IUserRepository users,
            IAuditService audit,
            INotificationService notifications,
            IAuthorisationService authorisation,
            TransitionGuard guard,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IClock clock,
            IAuditLogRepository auditLog)
        {
            _requests = requests;
            _users = users;
            _audit = audit;
            _notifications = notifications;
            _authorisation = authorisation;
            _guard = guard;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _clock = clock;
            _auditLog = auditLog;   // used only to stage staff-only notes (AddNoteAsync / ResolveAsync)
        }

        // Carries a business failure out of the transaction so everything staged so far is rolled back.
        private sealed class TransitionAbortedException : Exception
        {
            public Result Failure { get; }
            public TransitionAbortedException(Result failure) : base(failure.ErrorMessage) { Failure = failure; }
        }

        public async Task<IReadOnlyList<User>> ListAssignableStaffAsync()
        {
            if (_authorisation.EnsureInRole(UserRole.Staff, UserRole.Management).Failed)
                return new List<User>();

            return await _users.ListActiveStaffAsync();
        }

        public async Task<AvailableActions> GetAvailableActionsAsync(ServiceRequest request)
        {
            var actions = new AvailableActions();

            // Management and requesters are read-only: every flag stays false.
            if (_currentUser.Role != UserRole.Staff) return actions;
            if ((await _authorisation.EnsureCanWorkAsync(request)).Failed) return actions;

            var state = RequestStateFactory.For(request.Status);
            actions.CanAssign = state.CanMoveTo(RequestStatus.Assigned);
            actions.CanAccept = actions.CanAssign && request.AssignedStaffId == null;
            actions.CanStart = state.CanMoveTo(RequestStatus.InProgress);
            actions.CanResolve = state.CanMoveTo(RequestStatus.Resolved);
            actions.CanClose = state.CanMoveTo(RequestStatus.Closed);
            actions.CanAddNote = request.Status != RequestStatus.Closed;
            return actions;
        }

        public Task<Result> AssignAsync(AssignRequestCommand command)
        {
            return AssignInternalAsync(command.RequestId, command.AssigneeStaffId, command.RowVersion);
        }

        public Task<Result> AcceptAsync(AcceptRequestCommand command)
        {
            if (!_currentUser.UserId.HasValue)
                return Task.FromResult(Result.Failure(ErrorCodes.Unauthenticated, "Please sign in."));

            return AssignInternalAsync(command.RequestId, _currentUser.UserId.Value, command.RowVersion);
        }

        public async Task<Result> ChangeStatusAsync(ChangeStatusCommand command)
        {
            // Only Assigned -> InProgress is a plain status change. Everything else has its own method (Assign/Resolve/Close).
            if (command.TargetStatus != RequestStatus.InProgress)
            {
                return Result.Failure(ErrorCodes.InvalidTransition,
                    $"A request cannot be moved to {command.TargetStatus} this way.");
            }

            return await ExecuteTransitionAsync(
                command.RequestId, RequestStatus.InProgress, command.RowVersion,
                auditNote: string.IsNullOrWhiteSpace(command.Note) ? "Work started" : command.Note,
                staffComment: command.Note,
                stateAction: (state, request) => state.Start(request));
        }

        public async Task<Result> ResolveAsync(ResolveRequestCommand command)
        {
            var note = (command.ResolutionNote ?? string.Empty).Trim();
            if (note.Length == 0)
                return Result.Failure(ErrorCodes.ValidationFailed, "A resolution note is required.");
            if (note.Length > MaxNoteLength)
                return Result.Failure(ErrorCodes.ValidationFailed, $"The resolution note must be at most {MaxNoteLength} characters.");

            return await ExecuteTransitionAsync(
                command.RequestId, RequestStatus.Resolved, command.RowVersion,
                auditNote: "Request resolved",
                staffComment: null,   // the resolution note is a staff record, not requester feedback (FR-011)
                stateAction: (state, request) => state.Resolve(request, _clock.UtcNow),
                extraWork: async request =>
                {
                    await _auditLog.AddCommentAsync(new RequestComment
                    {
                        ServiceRequestId = request.Id,
                        Kind = CommentKind.ResolutionNote,
                        Body = note,
                        AuthorId = _currentUser.UserId!.Value,
                        CreatedAt = _clock.UtcNow,
                        VisibleToRequester = false
                    });
                });
        }

        public Task<Result> CloseAsync(CloseRequestCommand command)
        {
            return ExecuteTransitionAsync(
                command.RequestId, RequestStatus.Closed, command.RowVersion,
                auditNote: string.IsNullOrWhiteSpace(command.Note) ? "Request closed" : command.Note,
                staffComment: command.Note,
                stateAction: (state, request) => state.Close(request, _clock.UtcNow));
        }

        public async Task<Result> AddNoteAsync(AddNoteCommand command)
        {
            var auth = _authorisation.EnsureAuthenticated();
            if (auth.Failed) return auth;

            var body = (command.Body ?? string.Empty).Trim();
            if (body.Length == 0)
                return Result.Failure(ErrorCodes.ValidationFailed, "A note cannot be empty.");
            if (body.Length > MaxNoteLength)
                return Result.Failure(ErrorCodes.ValidationFailed, $"A note must be at most {MaxNoteLength} characters.");

            var request = await _requests.GetByIdAsync(command.RequestId);
            if (request == null) return Result.Failure(ErrorCodes.NotFound, "Request not found.");

            var work = await _authorisation.EnsureCanWorkAsync(request);
            if (work.Failed) return work;

            if (request.Status == RequestStatus.Closed)
                return Result.Failure(ErrorCodes.InvalidTransition, "A closed request cannot be changed.");

            try
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    await _auditLog.AddCommentAsync(new RequestComment
                    {
                        ServiceRequestId = request.Id,
                        Kind = CommentKind.ResolutionNote,
                        Body = body,
                        AuthorId = _currentUser.UserId!.Value,
                        CreatedAt = _clock.UtcNow,
                        VisibleToRequester = false
                    });
                });
            }
            catch (ConcurrencyConflictException)
            {
                return Result.Failure(ErrorCodes.ConcurrencyConflict, ConflictMessage);
            }

            return Result.Success();
        }

        /// <summary>Assign and Accept share everything except who the assignee is.</summary>
        private Task<Result> AssignInternalAsync(int requestId, int assigneeId, byte[] rowVersion)
        {
            User? assignee = null;

            return ExecuteTransitionAsync(
                requestId, RequestStatus.Assigned, rowVersion,
                auditNote: null,   // filled in below once the assignee's name is known
                staffComment: null,
                stateAction: (state, request) => state.Assign(request, assigneeId, _clock.UtcNow),
                preCheck: async request =>
                {
                    // Someone already holds it: say so, rather than a generic "invalid transition".
                    if (request.AssignedStaffId != null)
                        return Result.Failure(ErrorCodes.AlreadyAssigned, "This request has already been assigned.");

                    assignee = await _users.GetByIdAsync(assigneeId);
                    if (assignee == null || !assignee.IsActive || assignee.Role != UserRole.Staff)
                        return Result.Failure(ErrorCodes.ValidationFailed, "Please choose an active staff member.");

                    var scope = await _users.GetCategoryScopeIdsAsync(assigneeId);
                    if (!scope.Contains(request.CategoryId))
                        return Result.Failure(ErrorCodes.ValidationFailed, "That staff member does not handle this category.");

                    return Result.Success();
                },
                auditNoteFactory: () => $"Assigned to {assignee?.DisplayName}");
        }

        /// <summary>
        /// Shared pipeline for every status-changing method (keeps all transitions uniform):
        /// authenticate + load -> authorise -> pre-checks -> guard -> expected RowVersion ->
        /// one transaction { state action, audit row, requester feedback, extra work } -> translate concurrency conflicts.
        /// </summary>
        private async Task<Result> ExecuteTransitionAsync(
            int requestId,
            RequestStatus target,
            byte[] rowVersion,
            string? auditNote,
            string? staffComment,
            Func<RequestState, ServiceRequest, Result> stateAction,
            Func<ServiceRequest, Task<Result>>? preCheck = null,
            Func<ServiceRequest, Task>? extraWork = null,
            Func<string>? auditNoteFactory = null)
        {
            var auth = _authorisation.EnsureAuthenticated();
            if (auth.Failed) return auth;

            var request = await _requests.GetByIdAsync(requestId);
            if (request == null) return Result.Failure(ErrorCodes.NotFound, "Request not found.");

            if (preCheck != null)
            {
                // Authorise BEFORE revealing anything about the request's state (AlreadyAssigned etc.).
                var work = await _authorisation.EnsureCanWorkAsync(request);
                if (work.Failed) return work;

                var pre = await preCheck(request);
                if (pre.Failed) return pre;
            }

            var guard = await _guard.CheckAsync(request, target);
            if (guard.Failed) return guard;

            _requests.SetExpectedRowVersion(request, rowVersion);

            var fromStatus = request.Status;
            var note = auditNoteFactory != null ? auditNoteFactory() : auditNote;

            try
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var outcome = stateAction(RequestStateFactory.For(request.Status), request);
                    if (outcome.Failed) throw new TransitionAbortedException(outcome);

                    await _audit.RecordStatusChangeAsync(request, fromStatus, request.Status, note);

                    await _notifications.NotifyStatusChangedAsync(new StatusChangeNotice
                    {
                        RequestId = request.Id,
                        ReferenceNo = request.ReferenceNo,
                        FromStatus = fromStatus,
                        ToStatus = request.Status,
                        ChangedByUserId = _currentUser.UserId!.Value,
                        ChangedAt = _clock.UtcNow,
                        StaffComment = staffComment
                    });

                    if (extraWork != null) await extraWork(request);
                });
            }
            catch (TransitionAbortedException aborted)
            {
                return aborted.Failure;
            }
            catch (ConcurrencyConflictException)
            {
                return Result.Failure(ErrorCodes.ConcurrencyConflict, ConflictMessage);
            }

            return Result.Success();
        }
    }
}

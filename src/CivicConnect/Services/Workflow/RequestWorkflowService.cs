namespace CivicConnect.Services
{
    /// <summary>OWNER: Enrique de Sousa. The only place a request's Status or AssignedStaffId may change.</summary>
    public class RequestWorkflowService : IRequestWorkflowService
    {
        private readonly IRequestRepository _requests;
        private readonly IUserRepository _users;
        private readonly IAuditService _audit;
        private readonly INotificationService _notifications;
        private readonly IAuthorisationService _authorisation;
        private readonly TransitionGuard _guard;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IClock _clock;

        public RequestWorkflowService(
            IRequestRepository requests,
            IUserRepository users,
            IAuditService audit,
            INotificationService notifications,
            IAuthorisationService authorisation,
            TransitionGuard guard,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IClock clock)
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
        }

        public Task<IReadOnlyList<User>> ListAssignableStaffAsync()
        {
            // TODO: _authorisation.EnsureInRole(Staff, Management) (return empty list if not allowed); return _users.ListActiveStaffAsync().
            throw new NotImplementedException();
        }

        public Task<AvailableActions> GetAvailableActionsAsync(ServiceRequest request)
        {
            // TODO: build AvailableActions from RequestStateFactory.For(request.Status) + role/scope:
            //   CanAccept  = Staff in scope && state allows Assign && request.AssignedStaffId == null
            //   CanAssign  = Staff in scope && state allows Assign
            //   CanStart   = Staff in scope && state.CanMoveTo(InProgress)
            //   CanResolve = Staff in scope && state.CanMoveTo(Resolved)
            //   CanClose   = Staff in scope && state.CanMoveTo(Closed)
            //   CanAddNote = Staff in scope && request not Closed
            // Management and Requester: all false. Use _authorisation.EnsureCanWorkAsync for the scope check.
            throw new NotImplementedException();
        }

        public Task<Result> AssignAsync(AssignRequestCommand command)
        {
            // TODO (FR-009): follow the shared pipeline in ExecuteTransitionAsync with
            //   target = Assigned, stateAction = state => state.Assign(request, command.AssigneeStaffId, _clock.UtcNow).
            // Extra checks: assignee exists, IsActive, Role == Staff and has the request's category in scope -> ValidationFailed.
            // Already has AssignedStaffId -> AlreadyAssigned. Use command.RowVersion via _requests.SetExpectedRowVersion.
            throw new NotImplementedException();
        }

        public Task<Result> AcceptAsync(AcceptRequestCommand command)
        {
            // TODO (FR-009): same as AssignAsync but assignee = _currentUser.UserId. Two staff accepting at once: exactly one wins
            // (the loser gets ConcurrencyConflict from the RowVersion check).
            throw new NotImplementedException();
        }

        public Task<Result> ChangeStatusAsync(ChangeStatusCommand command)
        {
            // TODO (FR-010): only Assigned -> InProgress is supported here (use state.Start). Any other target -> InvalidTransition.
            throw new NotImplementedException();
        }

        public Task<Result> ResolveAsync(ResolveRequestCommand command)
        {
            // TODO (FR-011, FR-012): ResolutionNote required (not blank) -> ValidationFailed. state.Resolve(request, now).
            // In the same transaction also add a RequestComment { Kind = ResolutionNote, VisibleToRequester = false, Body = note }.
            throw new NotImplementedException();
        }

        public Task<Result> CloseAsync(CloseRequestCommand command)
        {
            // TODO (FR-012): state.Close(request, now) -> sets ClosedAt. Request is NOT deleted.
            throw new NotImplementedException();
        }

        public Task<Result> AddNoteAsync(AddNoteCommand command)
        {
            // TODO (FR-011): staff in scope only; Body required (<= 2000 chars); add RequestComment { Kind = ResolutionNote, VisibleToRequester = false }; save.
            // No status change, so no StatusHistory row and no notification.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shared pipeline for every status-changing method (keeps all transitions uniform):
        /// 1. EnsureAuthenticated + load request via _requests.GetByIdAsync (null -> NotFound).
        /// 2. _guard.CheckAsync(request, target) (role, scope, allowed transition) -> return its failure.
        /// 3. _requests.SetExpectedRowVersion(request, rowVersion).
        /// 4. Remember fromStatus; inside _unitOfWork.ExecuteInTransactionAsync: run stateAction(RequestStateFactory.For(request.Status))
        ///    (throw/return failure if it fails), then _audit.RecordStatusChangeAsync(...), then _notifications.NotifyStatusChangedAsync(...).
        /// 5. Catch ConcurrencyConflictException -> Failure(ConcurrencyConflict, "Someone else changed this request. Reload and try again.").
        /// </summary>
        private Task<Result> ExecuteTransitionAsync(int requestId, RequestStatus target, byte[] rowVersion, string? note, Func<RequestState, ServiceRequest, Result> stateAction)
        {
            // TODO: implement the five steps above exactly once and reuse from Assign/Accept/ChangeStatus/Resolve/Close.
            throw new NotImplementedException();
        }
    }
}

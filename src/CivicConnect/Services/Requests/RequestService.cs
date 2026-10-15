namespace CivicConnect.Services
{
    /// <summary>OWNER: Enrique de Sousa.</summary>
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requests;
        private readonly ICategoryRepository _categories;
        private readonly IAuditService _audit;
        private readonly IAuthorisationService _authorisation;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IClock _clock;
        private const int MaxDescriptionLength = 2000;

        public RequestService(
            IRequestRepository requests,
            ICategoryRepository categories,
            IAuditService audit,
            IAuthorisationService authorisation,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IClock clock)
        {
            _requests = requests;
            _categories = categories;
            _audit = audit;
            _authorisation = authorisation;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _clock = clock;
        }

        public Task<IReadOnlyList<Category>> ListActiveCategoriesAsync()
        {
            return _categories.ListActiveAsync();
        }

        public async Task<Result<ServiceRequest>> SubmitAsync(SubmitRequestCommand command)
        {
            // 1. Role check (requesters only).
            var role = _authorisation.EnsureInRole(UserRole.Requester);
            if (role.Failed) return Result<ServiceRequest>.Failure(role.ErrorCode!, role.ErrorMessage!);

            // 2. Service-layer validation (middle layer of the three-layer rule; the view model is the first, the DB the third).
            var description = (command.Description ?? string.Empty).Trim();
            if (description.Length == 0)
                return Result<ServiceRequest>.Failure(ErrorCodes.ValidationFailed, "Please describe the problem.");
            if (description.Length > MaxDescriptionLength)
                return Result<ServiceRequest>.Failure(ErrorCodes.ValidationFailed, $"The description must be at most {MaxDescriptionLength} characters.");
            if (!await _categories.IsActiveAsync(command.CategoryId))
                return Result<ServiceRequest>.Failure(ErrorCodes.ValidationFailed, "Please select a valid category.");

            // 3. One transaction: the request and its first audit row commit together or not at all (NFR-010).
            ServiceRequest? created = null;
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var request = new ServiceRequest
                {
                    ReferenceNo = await _requests.NextReferenceNoAsync(),
                    CategoryId = command.CategoryId,
                    Description = description,
                    RequesterId = _currentUser.UserId!.Value,
                    Status = RequestStatus.Submitted,
                    SubmittedAt = _clock.UtcNow
                };

                // Audit first: the request has no Id yet, so the history row is linked by navigation and EF inserts both together.
                await _audit.RecordStatusChangeAsync(request, null, RequestStatus.Submitted, "Request submitted");
                await _requests.AddAsync(request);
                created = request;
            });

            return Result<ServiceRequest>.Success(created!);
        }

        public async Task<Result<ServiceRequest>> GetDetailsAsync(int requestId)
        {
            var auth = _authorisation.EnsureAuthenticated();
            if (auth.Failed) return Result<ServiceRequest>.Failure(auth.ErrorCode!, auth.ErrorMessage!);

            var request = await _requests.GetByIdWithDetailsAsync(requestId);
            if (request == null)
                return Result<ServiceRequest>.Failure(ErrorCodes.NotFound, "Request not found.");

            var view = await _authorisation.EnsureCanViewAsync(request);
            if (view.Failed) return Result<ServiceRequest>.Failure(view.ErrorCode!, view.ErrorMessage!);

            // Requesters never see staff-only notes (FR-011).
            if (_currentUser.Role == UserRole.Requester)
            {
                request.Comments = request.Comments.Where(c => c.VisibleToRequester).ToList();
            }

            return Result<ServiceRequest>.Success(request);
        }

        public async Task<IReadOnlyList<ServiceRequest>> ListMineAsync()
        {
            if (_authorisation.EnsureInRole(UserRole.Requester).Failed)
                return new List<ServiceRequest>();

            return await _requests.ListByRequesterAsync(_currentUser.UserId!.Value);
        }

        public async Task<PagedResult<ServiceRequest>> SearchAsync(RequestSearchCriteria criteria)
        {
            if (_authorisation.EnsureInRole(UserRole.Staff, UserRole.Management).Failed)
            {
                return new PagedResult<ServiceRequest> { Page = Math.Max(1, criteria.Page), PageSize = criteria.PageSize };
            }

            var visible = await _authorisation.GetVisibleCategoryIdsAsync();
            return await _requests.SearchAsync(criteria, visible);
        }
    }
}

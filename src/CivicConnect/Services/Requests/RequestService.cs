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
            // TODO: return _categories.ListActiveAsync().
            throw new NotImplementedException();
        }

        public Task<Result<ServiceRequest>> SubmitAsync(SubmitRequestCommand command)
        {
            // TODO (FR-001, FR-002, NFR-008, NFR-010):
            // 1. _authorisation.EnsureInRole(UserRole.Requester) -> return failure if not ok.
            // 2. SERVICE-LAYER validation: Description not blank and <= 2000 chars; _categories.IsActiveAsync(command.CategoryId)
            //    (this replaces the old controller check "Categories.Any(c => c.Id == id && c.IsActive)") -> ValidationFailed.
            // 3. Inside _unitOfWork.ExecuteInTransactionAsync: build ServiceRequest { ReferenceNo = await _requests.NextReferenceNoAsync(),
            //    CategoryId, Description (trimmed), RequesterId = current user, Status = Submitted, SubmittedAt = _clock.UtcNow };
            //    _requests.AddAsync; _audit.RecordStatusChangeAsync(request, null, Submitted, "Request submitted").
            // 4. Return Success(request). If anything throws, nothing is saved (atomic).
            throw new NotImplementedException();
        }

        public Task<Result<ServiceRequest>> GetDetailsAsync(int requestId)
        {
            // TODO (FR-008): _requests.GetByIdWithDetailsAsync -> null => NotFound; _authorisation.EnsureCanViewAsync(request) => Forbidden;
            // for a Requester, strip comments where VisibleToRequester == false (staff resolution notes) before returning.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ServiceRequest>> ListMineAsync()
        {
            // TODO (FR-003, FR-004): EnsureInRole(Requester) else return empty list; return _requests.ListByRequesterAsync(_currentUser.UserId!.Value).
            throw new NotImplementedException();
        }

        public Task<PagedResult<ServiceRequest>> SearchAsync(RequestSearchCriteria criteria)
        {
            // TODO (FR-006, FR-007, FR-015): EnsureInRole(Staff, Management); visible = await _authorisation.GetVisibleCategoryIdsAsync();
            // return _requests.SearchAsync(criteria, visible). If a staff user filters on a category outside their scope, the result is simply empty.
            throw new NotImplementedException();
        }
    }
}

namespace CivicConnect.Services
{
    /// <summary>OWNER: Aidan Smith.</summary>
    public class AuthorisationService : IAuthorisationService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUserRepository _users;

        public AuthorisationService(ICurrentUser currentUser, IUserRepository users)
        {
            _currentUser = currentUser;
            _users = users;
        }

        public Result EnsureAuthenticated()
        {
            // TODO: _currentUser.IsAuthenticated ? Success : Failure(ErrorCodes.Unauthenticated, ...)
            throw new NotImplementedException();
        }

        public Result EnsureInRole(params UserRole[] roles)
        {
            // TODO: not authenticated -> Unauthenticated; Role not in roles -> Forbidden; else Success.
            throw new NotImplementedException();
        }

        public Task<Result> EnsureCanViewAsync(ServiceRequest request)
        {
            // TODO: Requester -> request.RequesterId == UserId; Staff -> request.CategoryId in GetVisibleCategoryIdsAsync();
            // Management -> allowed. Anything else -> Forbidden. Never reveal whether a forbidden request exists (callers return NotFound/Forbid uniformly).
            throw new NotImplementedException();
        }

        public Task<Result> EnsureCanWorkAsync(ServiceRequest request)
        {
            // TODO: role must be Staff AND category in scope; Management/Requester -> Forbidden.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<int>?> GetVisibleCategoryIdsAsync()
        {
            // TODO: Management -> null; Staff -> _users.GetCategoryScopeIdsAsync(UserId); otherwise empty collection.
            throw new NotImplementedException();
        }
    }
}

namespace CivicConnect.Services
{
    // The single enforcement point for role + category access (NFR-003)
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
            return _currentUser.IsAuthenticated && _currentUser.UserId.HasValue && _currentUser.Role.HasValue
                ? Result.Success()
                : Result.Failure(ErrorCodes.Unauthenticated, "You must log in to continue.");
        }

        public Result EnsureInRole(params UserRole[] roles)
        {
            var auth = EnsureAuthenticated();
            if (auth.Failed) return auth;
            return roles.Contains(_currentUser.Role!.Value)
                ? Result.Success()
                : Result.Failure(ErrorCodes.Forbidden, "You do not have permission to do this.");
        }

        public async Task<Result> EnsureCanViewAsync(ServiceRequest request)
        {
            var auth = EnsureAuthenticated();
            if (auth.Failed) return auth;

            switch (_currentUser.Role!.Value)
            {
                case UserRole.Management:
                    return Result.Success();
                case UserRole.Requester:
                    return request.RequesterId == _currentUser.UserId!.Value ? Result.Success() : Forbidden();
                case UserRole.Staff:
                    return await IsInStaffScopeAsync(request) ? Result.Success() : Forbidden();
                default:
                    return Forbidden();
            }
        }

        public async Task<Result> EnsureCanWorkAsync(ServiceRequest request)
        {
            var role = EnsureInRole(UserRole.Staff);   // Management is read-only; Requesters never work requests
            if (role.Failed) return role;
            return await IsInStaffScopeAsync(request) ? Result.Success() : Forbidden();
        }

        public async Task<IReadOnlyCollection<int>?> GetVisibleCategoryIdsAsync()
        {
            if (!_currentUser.IsAuthenticated || !_currentUser.Role.HasValue || !_currentUser.UserId.HasValue)
                return Array.Empty<int>();

            switch (_currentUser.Role.Value)
            {
                case UserRole.Management:
                    return null;   // unrestricted
                case UserRole.Staff:
                    return (await _users.GetCategoryScopeIdsAsync(_currentUser.UserId.Value)).ToList();
                default:
                    return Array.Empty<int>();
            }
        }

        private async Task<bool> IsInStaffScopeAsync(ServiceRequest request)
        {
            var scope = await _users.GetCategoryScopeIdsAsync(_currentUser.UserId!.Value);
            return scope.Contains(request.CategoryId);
        }

        // One uniform message for every refusal: never reveal whether a request exists or why access failed.
        private static Result Forbidden() => Result.Failure(ErrorCodes.Forbidden, "You do not have permission to view or change this request.");
    }
}


namespace CivicConnect.Services
{
    /// <summary>OWNER: Aidan Smith. Reads the cookie principal via IHttpContextAccessor.</summary>
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated => throw new NotImplementedException();
        // TODO: _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true

        public int? UserId => throw new NotImplementedException();
        // TODO: parse the AppClaimTypes.UserId claim to int; null if missing/unparseable/not authenticated.

        public UserRole? Role => throw new NotImplementedException();
        // TODO: parse ClaimTypes.Role claim with Enum.TryParse<UserRole>; null if missing.

        public string DisplayName => throw new NotImplementedException();
        // TODO: AppClaimTypes.DisplayName claim, or empty string when absent.
    }
}

namespace CivicConnect.Services
{
    // Reads the cookie principal via IHttpContextAccessor
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

        public int? UserId
        {
            get
            {
                if (!IsAuthenticated) return null;
                var raw = Principal!.FindFirst(AppClaimTypes.UserId)?.Value;
                return int.TryParse(raw, out var id) ? id : null;
            }
        }

        public UserRole? Role
        {
            get
            {
                if (!IsAuthenticated) return null;
                var raw = Principal!.FindFirst(ClaimTypes.Role)?.Value;
                // Accept only the exact role names, Enum.TryParse would also accept numeric strings
                return raw != null && Enum.GetNames(typeof(UserRole)).Contains(raw) ? Enum.Parse<UserRole>(raw) : null;
            }
        }

        public string DisplayName => IsAuthenticated ? Principal!.FindFirst(AppClaimTypes.DisplayName)?.Value ?? string.Empty : string.Empty;
    }
}


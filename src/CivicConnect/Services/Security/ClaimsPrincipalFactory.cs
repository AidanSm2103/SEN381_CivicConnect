namespace CivicConnect.Services
{
    // Builds the cookie principal after a successful login
    public static class ClaimsPrincipalFactory
    {
        public static ClaimsPrincipal Create(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            var claims = new List<Claim>
            {
                new Claim(AppClaimTypes.UserId, user.Id.ToString()),
                new Claim(AppClaimTypes.DisplayName, user.DisplayName),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            return new ClaimsPrincipal(identity);
        }
    }
}


namespace CivicConnect.Services
{
    /// <summary>OWNER: Aidan Smith. Builds the cookie principal after a successful login.</summary>
    public static class ClaimsPrincipalFactory
    {
        public static ClaimsPrincipal Create(User user)
        {
            // TODO: claims = [ AppClaimTypes.UserId = user.Id, AppClaimTypes.DisplayName = user.DisplayName,
            //                  ClaimTypes.Name = user.Email, ClaimTypes.Role = user.Role.ToString() ].
            // Identity authenticationType must be CookieAuthenticationDefaults.AuthenticationScheme.
            throw new NotImplementedException();
        }
    }
}

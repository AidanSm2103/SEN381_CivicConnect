namespace CivicConnect.Services
{
    /// <summary>
    /// OWNER: Aidan Smith. Registers the named policies (PolicyNames). Called from SecurityServiceExtensions.
    /// SAFE NO-OP until implemented so the app starts (requests to [Authorize(Policy=..)] pages will error until done).
    /// </summary>
    public static class AuthorizationPolicies
    {
        public static void Register(AuthorizationOptions options)
        {
            // TODO: options.AddPolicy(PolicyNames.RequesterAccess,  p => p.RequireAuthenticatedUser().RequireRole(RoleNames.Requester));
            //       options.AddPolicy(PolicyNames.StaffAccess,      p => ... RoleNames.Staff);
            //       options.AddPolicy(PolicyNames.ManagementAccess, p => ... RoleNames.Management);
            //       options.AddPolicy(PolicyNames.StaffOrManagement,p => ... RequireRole(RoleNames.Staff, RoleNames.Management));
            // Also set options.FallbackPolicy = null (public pages stay public: Home, Account/Login, Account/Register).
        }
    }
}

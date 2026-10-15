namespace CivicConnect.Services
{

    // Registers the named policies called from SecurityServiceExtensions
    // The fine-grained check called inside every service method (NFR-003 single enforcement point)
   
    public static class AuthorizationPolicies
    {
        public static void Register(AuthorizationOptions options)
        {
            options.AddPolicy(PolicyNames.RequesterAccess, p => p.RequireAuthenticatedUser().RequireRole(RoleNames.Requester));
            options.AddPolicy(PolicyNames.StaffAccess, p => p.RequireAuthenticatedUser().RequireRole(RoleNames.Staff));
            options.AddPolicy(PolicyNames.ManagementAccess, p => p.RequireAuthenticatedUser().RequireRole(RoleNames.Management));
            options.AddPolicy(PolicyNames.StaffOrManagement, p => p.RequireAuthenticatedUser().RequireRole(RoleNames.Staff, RoleNames.Management));
            // No FallbackPolicy: public pages stay public; every other controller carries [Authorize(Policy=...)]
        }
    }
}

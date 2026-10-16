namespace CivicConnect.Common
{
    /// <summary>
    /// Authorisation policy names. Controllers use ONLY these in [Authorize(Policy = ...)].
    /// Policies are registered in Services/Security/AuthorizationPolicies.cs. (NFR-003)
    /// </summary>
    public static class PolicyNames
    {
        public const string RequesterAccess = "RequesterAccess";
        public const string StaffAccess = "StaffAccess";
        public const string ManagementAccess = "ManagementAccess";
        public const string StaffOrManagement = "StaffOrManagement";
    }
}

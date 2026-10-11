namespace CivicConnect.Common
{
    /// <summary>Custom claim type names stored in the auth cookie.</summary>
    public static class AppClaimTypes
    {
        public const string UserId = "civicconnect:user_id";
        public const string DisplayName = "civicconnect:display_name";
        // The role is stored under the standard ClaimTypes.Role.
    }
}

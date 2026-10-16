namespace CivicConnect.Common
{
    /// <summary>String names of the three roles. Used in claims and [Authorize] policies. (NFR-003)</summary>
    public static class RoleNames
    {
        public const string Requester = "Requester";
        public const string Staff = "Staff";
        public const string Management = "Management";
    }
}

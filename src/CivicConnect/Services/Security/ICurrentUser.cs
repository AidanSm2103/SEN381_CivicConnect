namespace CivicConnect.Services
{
    /// <summary>The authenticated user of the current HTTP request. Services use this instead of touching HttpContext.</summary>
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }
        /// <summary>Null when not authenticated.</summary>
        int? UserId { get; }
        /// <summary>Null when not authenticated.</summary>
        UserRole? Role { get; }
        string DisplayName { get; }
    }
}

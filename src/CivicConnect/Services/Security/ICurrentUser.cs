namespace CivicConnect.Services
{
    // The authenticated user of the current HTTP request
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }
        // Null when not authenticated.
        int? UserId { get; }
        // Null when not authenticated.
        UserRole? Role { get; }
        string DisplayName { get; }
    }
}

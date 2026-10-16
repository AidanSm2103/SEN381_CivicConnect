namespace CivicConnect.Models
{
    /// <summary>
    /// Application user. Never deleted - deactivated (IsActive = false) so audit references stay valid.
    /// Only PasswordHash is stored, never the password (NFR-004).
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Requester;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Staff only: which categories this person may see/work (FR-006).
        public ICollection<StaffCategoryScope> CategoryScopes { get; set; } = new List<StaffCategoryScope>();
    }
}

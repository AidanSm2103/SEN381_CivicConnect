namespace CivicConnect.Data
{
    /// <summary>Data access for User and StaffCategoryScope.</summary>
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);

        /// <summary>CategoryIds this staff member is scoped to (FR-006). Empty list = sees nothing.</summary>
        Task<IReadOnlyList<int>> GetCategoryScopeIdsAsync(int staffUserId);

        /// <summary>Active users with Role == Staff, ordered by DisplayName (for the assign dropdown, FR-009).</summary>
        Task<IReadOnlyList<User>> ListActiveStaffAsync();
    }
}

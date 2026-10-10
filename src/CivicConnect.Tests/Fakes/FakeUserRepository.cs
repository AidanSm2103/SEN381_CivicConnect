namespace CivicConnect.Tests.Fakes
{
    /// <summary>In-memory IUserRepository. OWNER: Aidan Smith.</summary>
    public class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = new();
        public Dictionary<int, List<int>> CategoryScopes { get; } = new();   // staffUserId -> categoryIds

        public Task<User?> GetByIdAsync(int id)
        {
            // TODO: Users.FirstOrDefault(u => u.Id == id).
            throw new NotImplementedException();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            // TODO: case-insensitive match.
            throw new NotImplementedException();
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            // TODO: case-insensitive Any.
            throw new NotImplementedException();
        }

        public Task AddAsync(User user)
        {
            // TODO: assign Id if 0 and add to Users.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<int>> GetCategoryScopeIdsAsync(int staffUserId)
        {
            // TODO: return CategoryScopes[staffUserId] or empty list.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<User>> ListActiveStaffAsync()
        {
            // TODO: Users where Role == Staff && IsActive.
            throw new NotImplementedException();
        }
    }
}

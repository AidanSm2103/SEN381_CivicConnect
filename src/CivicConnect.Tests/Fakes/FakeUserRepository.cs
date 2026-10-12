namespace CivicConnect.Tests.Fakes
{
    // In-memory IUserRepository 
    public class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = new();
        public Dictionary<int, List<int>> CategoryScopes { get; } = new();   // staffUserId -> categoryIds

        public Task<User?> GetByIdAsync(int id) => Task.FromResult(Users.FirstOrDefault(u => u.Id == id));

        public Task<User?> GetByEmailAsync(string email) =>
            Task.FromResult(Users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));

        public Task<bool> EmailExistsAsync(string email) =>
            Task.FromResult(Users.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));

        public Task AddAsync(User user)
        {
            if (user.Id == 0) user.Id = Users.Count + 1;
            Users.Add(user);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<int>> GetCategoryScopeIdsAsync(int staffUserId)
        {
            IReadOnlyList<int> ids = CategoryScopes.TryGetValue(staffUserId, out var list) ? list.ToList() : new List<int>();
            return Task.FromResult(ids);
        }

        public Task<IReadOnlyList<User>> ListActiveStaffAsync()
        {
            IReadOnlyList<User> staff = Users.Where(u => u.Role == UserRole.Staff && u.IsActive).OrderBy(u => u.DisplayName).ToList();
            return Task.FromResult(staff);
        }
    }
}


namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson.</summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        // SQL Server's default collation is case-insensitive; the explicit ToLower keeps the rule identical on any provider.
        private static string Normalise(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();

        public Task<User?> GetByIdAsync(int id)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            var normalised = Normalise(email);
            return _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalised);
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            var normalised = Normalise(email);
            return _db.Users.AnyAsync(u => u.Email.ToLower() == normalised);
        }

        public async Task AddAsync(User user)
        {
            await _db.Users.AddAsync(user);
        }

        public async Task<IReadOnlyList<int>> GetCategoryScopeIdsAsync(int staffUserId)
        {
            return await _db.StaffCategoryScopes
                .AsNoTracking()
                .Where(s => s.UserId == staffUserId)
                .Select(s => s.CategoryId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<User>> ListActiveStaffAsync()
        {
            return await _db.Users
                .AsNoTracking()
                .Where(u => u.Role == UserRole.Staff && u.IsActive)
                .OrderBy(u => u.DisplayName)
                .ToListAsync();
        }
    }
}

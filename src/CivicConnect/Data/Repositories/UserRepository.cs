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

        public Task<User?> GetByIdAsync(int id)
        {
            // TODO: FirstOrDefaultAsync by Id.
            throw new NotImplementedException();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            // TODO: case-insensitive match on Email (normalise with ToLower / ToUpperInvariant on both sides).
            throw new NotImplementedException();
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            // TODO: AnyAsync with the same case-insensitive rule as GetByEmailAsync.
            throw new NotImplementedException();
        }

        public Task AddAsync(User user)
        {
            // TODO: await _db.Users.AddAsync(user). No SaveChanges.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<int>> GetCategoryScopeIdsAsync(int staffUserId)
        {
            // TODO: select CategoryId from StaffCategoryScopes where UserId == staffUserId.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<User>> ListActiveStaffAsync()
        {
            // TODO: Users where Role == UserRole.Staff && IsActive, order by DisplayName, AsNoTracking.
            throw new NotImplementedException();
        }
    }
}

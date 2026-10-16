namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson.</summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _db;

        public CategoryRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Category>> ListActiveAsync()
        {
            return await _db.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            return _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<bool> IsActiveAsync(int id)
        {
            return _db.Categories.AnyAsync(c => c.Id == id && c.IsActive);
        }
    }
}

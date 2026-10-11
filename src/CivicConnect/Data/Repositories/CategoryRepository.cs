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

        public Task<IReadOnlyList<Category>> ListActiveAsync()
        {
            // TODO: port the query from the old ServiceRequestsController.LoadCategories():
            //   _db.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync()
            throw new NotImplementedException();
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            // TODO: FirstOrDefaultAsync by Id.
            throw new NotImplementedException();
        }

        public Task<bool> IsActiveAsync(int id)
        {
            // TODO: port from old controller: _db.Categories.AnyAsync(c => c.Id == id && c.IsActive)
            throw new NotImplementedException();
        }
    }
}

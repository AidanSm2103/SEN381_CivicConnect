namespace CivicConnect.Tests.Fakes
{
    /// <summary>In-memory ICategoryRepository. OWNER: Aidan Smith.</summary>
    public class FakeCategoryRepository : ICategoryRepository
    {
        public List<Category> Categories { get; } = new()
        {
            new Category { Id = 1, Name = "Maintenance", IsActive = true },
            new Category { Id = 2, Name = "IT Support", IsActive = true },
            new Category { Id = 99, Name = "Retired", IsActive = false }
        };

        public Task<IReadOnlyList<Category>> ListActiveAsync()
        {
            // TODO: active categories ordered by Name.
            throw new NotImplementedException();
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            // TODO: FirstOrDefault by Id.
            throw new NotImplementedException();
        }

        public Task<bool> IsActiveAsync(int id)
        {
            // TODO: Categories.Any(c => c.Id == id && c.IsActive).
            throw new NotImplementedException();
        }
    }
}

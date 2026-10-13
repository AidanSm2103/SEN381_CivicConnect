namespace CivicConnect.Tests.Fakes
{
    // In-memory ICategoryRepository
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
            IReadOnlyList<Category> list = Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToList();
            return Task.FromResult(list);
        }

        public Task<Category?> GetByIdAsync(int id) => Task.FromResult(Categories.FirstOrDefault(c => c.Id == id));

        public Task<bool> IsActiveAsync(int id) => Task.FromResult(Categories.Any(c => c.Id == id && c.IsActive));
    }
}

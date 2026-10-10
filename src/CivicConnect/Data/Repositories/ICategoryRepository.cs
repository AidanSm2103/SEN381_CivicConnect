namespace CivicConnect.Data
{
    /// <summary>Data access for the controlled Category list (FR-002).</summary>
    public interface ICategoryRepository
    {
        /// <summary>Active categories ordered by Name.</summary>
        Task<IReadOnlyList<Category>> ListActiveAsync();
        Task<Category?> GetByIdAsync(int id);
        /// <summary>True only if the category exists AND IsActive. Used to defeat crafted POSTs.</summary>
        Task<bool> IsActiveAsync(int id);
    }
}

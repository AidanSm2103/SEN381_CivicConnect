namespace CivicConnect.Models
{
    /// <summary>Join row: staff user X may see/work requests in category Y (FR-006, NFR-003). Owner: Identity &amp; Access.</summary>
    public class StaffCategoryScope
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}

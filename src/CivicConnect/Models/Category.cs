using System.ComponentModel.DataAnnotations;

namespace CivicConnect.Models
{
    // Controlled reference data for FR-002: requesters choose from this list,
    // they cannot type their own category. Retired categories are deactivated
    // (IsActive = false), never deleted, because existing requests reference them.
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}

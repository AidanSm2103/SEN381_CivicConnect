using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CivicConnect.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        // FR-002: category is mandatory and must come from the controlled Category list.
        [Required(ErrorMessage = "Please select a category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category? Category { get; set; }

        [Required]
        public string Description { get; set; }

        public string Status { get; set; } = "Submitted";

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}

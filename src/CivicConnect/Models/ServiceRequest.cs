using System.ComponentModel.DataAnnotations;

namespace CivicConnect.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Description { get; set; }

        public string Status { get; set; } = "Submitted"; // matches FR-003's status flow

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}

namespace CivicConnect.Models
{
    /// <summary>Append-only audit row, one per status change, written in the SAME transaction as the change (NFR-010). Never updated or deleted.</summary>
    public class StatusHistory
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }
        public RequestStatus? FromStatus { get; set; }   // null for the very first row (creation)
        public RequestStatus ToStatus { get; set; }
        public int ChangedByUserId { get; set; }
        public User? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Note { get; set; }
    }
}

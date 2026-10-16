namespace CivicConnect.Models
{
    /// <summary>Append-only message on a request: requester-visible Feedback (FR-005) or staff ResolutionNote (FR-011).</summary>
    public class RequestComment
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }
        public CommentKind Kind { get; set; }
        public string Body { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public User? Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool VisibleToRequester { get; set; }
    }
}

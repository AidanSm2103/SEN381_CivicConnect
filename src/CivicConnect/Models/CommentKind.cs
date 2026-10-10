namespace CivicConnect.Models
{
    /// <summary>Feedback = requester-visible message written on a status change (FR-005). ResolutionNote = staff record of how it was handled (FR-011).</summary>
    public enum CommentKind
    {
        Feedback = 0,
        ResolutionNote = 1
    }
}

namespace CivicConnect.Services
{
    /// <summary>
    /// Requester feedback on status change (FR-005, decision D-003): synchronous, in-process, writes a
    /// RequestComment (Kind = Feedback, VisibleToRequester = true) in the SAME transaction as the status change.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>Stage the Feedback comment. Does NOT save. Message wording: see InProcessNotificationService.</summary>
        Task NotifyStatusChangedAsync(StatusChangeNotice notice);

        /// <summary>Feedback the requester may see for a request, oldest first. Caller has already passed the view-authorisation check.</summary>
        Task<IReadOnlyList<RequestComment>> GetFeedbackAsync(int requestId);
    }
}

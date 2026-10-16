namespace CivicConnect.Services
{
    // Requester feedback on status change (FR-005, D-003): synchronous, in-process, writes a RequestComment 
    public interface INotificationService
    {
        // Stage the Feedback comment 
        Task NotifyStatusChangedAsync(StatusChangeNotice notice);

        // Feedback the requester may see for a request, oldest first
        Task<IReadOnlyList<RequestComment>> GetFeedbackAsync(int requestId);
    }
}


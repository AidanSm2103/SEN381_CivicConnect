namespace CivicConnect.Services
{
    /// <summary>OWNER: Aidan Smith.</summary>
    public class InProcessNotificationService : INotificationService
    {
        private readonly IAuditLogRepository _auditLog;

        public InProcessNotificationService(IAuditLogRepository auditLog)
        {
            _auditLog = auditLog;
        }

        public Task NotifyStatusChangedAsync(StatusChangeNotice notice)
        {
            // TODO: compose a plain-language message, e.g. "Your request CC-2026-000123 is now In Progress." (+ staff comment if present).
            // Create RequestComment { ServiceRequestId = notice.RequestId, Kind = Feedback, Body, AuthorId = notice.ChangedByUserId,
            // CreatedAt = notice.ChangedAt, VisibleToRequester = true } and stage it with _auditLog.AddCommentAsync. DO NOT save.
            // Never put requester contact details in the message.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<RequestComment>> GetFeedbackAsync(int requestId)
        {
            // TODO: _auditLog.GetCommentsAsync(requestId, requesterVisibleOnly: true) filtered to Kind == Feedback.
            throw new NotImplementedException();
        }
    }
}

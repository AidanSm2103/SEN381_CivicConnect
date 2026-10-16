namespace CivicConnect.Services
{
    // In-app requester feedback written in the same transaction as the status change (D-003)
    public class InProcessNotificationService : INotificationService
    {
        private const int MaxBodyLength = 2000;
        private readonly IAuditLogRepository _auditLog;

        public InProcessNotificationService(IAuditLogRepository auditLog)
        {
            _auditLog = auditLog;
        }

        public Task NotifyStatusChangedAsync(StatusChangeNotice notice)
        {
            var body = BuildMessage(notice);
            if (body.Length > MaxBodyLength) body = body.Substring(0, MaxBodyLength);

            var comment = new RequestComment
            {
                ServiceRequestId = notice.RequestId,
                Kind = CommentKind.Feedback,
                Body = body,
                AuthorId = notice.ChangedByUserId,
                CreatedAt = notice.ChangedAt,
                VisibleToRequester = true
            };
            return _auditLog.AddCommentAsync(comment);   // staged only, never saves
        }

        public async Task<IReadOnlyList<RequestComment>> GetFeedbackAsync(int requestId)
        {
            var comments = await _auditLog.GetCommentsAsync(requestId, requesterVisibleOnly: true);
            return comments.Where(c => c.Kind == CommentKind.Feedback).ToList();
        }

        // Plain language, no contact details or internal notes
        private static string BuildMessage(StatusChangeNotice notice)
        {
            var reference = notice.ReferenceNo;
            var text = notice.ToStatus switch
            {
                RequestStatus.Submitted => $"We have received your request {reference}.",
                RequestStatus.Assigned => $"Your request {reference} has been accepted and assigned to a staff member.",
                RequestStatus.InProgress => $"Work has started on your request {reference}.",
                RequestStatus.Resolved => $"Your request {reference} has been resolved.",
                RequestStatus.Closed => $"Your request {reference} has been closed.",
                _ => $"The status of your request {reference} has changed."
            };
            if (!string.IsNullOrWhiteSpace(notice.StaffComment))
                text += " Staff comment: " + notice.StaffComment.Trim();
            return text;
        }
    }
}


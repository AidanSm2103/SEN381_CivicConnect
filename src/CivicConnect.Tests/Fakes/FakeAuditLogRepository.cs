namespace CivicConnect.Tests.Fakes
{
    /// <summary>In-memory IAuditLogRepository. Tests assert on Histories/Comments. OWNER: Aidan Smith.</summary>
    public class FakeAuditLogRepository : IAuditLogRepository
    {
        public List<StatusHistory> Histories { get; } = new();
        public List<RequestComment> Comments { get; } = new();

        public Task AddStatusHistoryAsync(StatusHistory entry)
        {
            // TODO: Histories.Add(entry); return Task.CompletedTask.
            throw new NotImplementedException();
        }

        public Task AddCommentAsync(RequestComment comment)
        {
            // TODO: Comments.Add(comment); return Task.CompletedTask.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<StatusHistory>> GetHistoryAsync(int requestId)
        {
            // TODO: filter Histories by ServiceRequestId, order by ChangedAt.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<RequestComment>> GetCommentsAsync(int requestId, bool requesterVisibleOnly)
        {
            // TODO: filter Comments (and VisibleToRequester when requested), order by CreatedAt.
            throw new NotImplementedException();
        }
    }
}

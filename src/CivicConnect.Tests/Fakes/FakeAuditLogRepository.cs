namespace CivicConnect.Tests.Fakes
{
    // In-memory IAuditLogRepository. Tests assert on Histories/Comments
    public class FakeAuditLogRepository : IAuditLogRepository
    {
        public List<StatusHistory> Histories { get; } = new();
        public List<RequestComment> Comments { get; } = new();

        public Task AddStatusHistoryAsync(StatusHistory entry)
        {
            Histories.Add(entry);
            return Task.CompletedTask;
        }

        public Task AddCommentAsync(RequestComment comment)
        {
            Comments.Add(comment);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<StatusHistory>> GetHistoryAsync(int requestId)
        {
            IReadOnlyList<StatusHistory> list = Histories.Where(h => h.ServiceRequestId == requestId).OrderBy(h => h.ChangedAt).ToList();
            return Task.FromResult(list);
        }

        public Task<IReadOnlyList<RequestComment>> GetCommentsAsync(int requestId, bool requesterVisibleOnly)
        {
            IReadOnlyList<RequestComment> list = Comments
                .Where(c => c.ServiceRequestId == requestId && (!requesterVisibleOnly || c.VisibleToRequester))
                .OrderBy(c => c.CreatedAt)
                .ToList();
            return Task.FromResult(list);
        }
    }
}

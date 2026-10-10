namespace CivicConnect.Data
{
    /// <summary>Append-only access to StatusHistory and RequestComment (NFR-010). There is deliberately NO update or delete method.</summary>
    public interface IAuditLogRepository
    {
        /// <summary>Stage a new history row (does not save).</summary>
        Task AddStatusHistoryAsync(StatusHistory entry);

        /// <summary>Stage a new comment row (does not save).</summary>
        Task AddCommentAsync(RequestComment comment);

        /// <summary>History for one request, oldest first, ChangedBy included.</summary>
        Task<IReadOnlyList<StatusHistory>> GetHistoryAsync(int requestId);

        /// <summary>Comments for one request, oldest first, Author included. requesterVisibleOnly = true filters VisibleToRequester == true.</summary>
        Task<IReadOnlyList<RequestComment>> GetCommentsAsync(int requestId, bool requesterVisibleOnly);
    }
}

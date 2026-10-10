namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson.</summary>
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _db;

        public AuditLogRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task AddStatusHistoryAsync(StatusHistory entry)
        {
            // TODO: await _db.StatusHistories.AddAsync(entry). No SaveChanges.
            throw new NotImplementedException();
        }

        public Task AddCommentAsync(RequestComment comment)
        {
            // TODO: await _db.RequestComments.AddAsync(comment). No SaveChanges.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<StatusHistory>> GetHistoryAsync(int requestId)
        {
            // TODO: where ServiceRequestId == requestId, Include ChangedBy, order by ChangedAt ascending, AsNoTracking.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<RequestComment>> GetCommentsAsync(int requestId, bool requesterVisibleOnly)
        {
            // TODO: where ServiceRequestId == requestId (and VisibleToRequester when requesterVisibleOnly), Include Author, order by CreatedAt ascending.
            throw new NotImplementedException();
        }
    }
}

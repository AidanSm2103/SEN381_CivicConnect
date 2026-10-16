namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson. Append-only: add and read only.</summary>
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _db;

        public AuditLogRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddStatusHistoryAsync(StatusHistory entry)
        {
            await _db.StatusHistories.AddAsync(entry);
        }

        public async Task AddCommentAsync(RequestComment comment)
        {
            await _db.RequestComments.AddAsync(comment);
        }

        public async Task<IReadOnlyList<StatusHistory>> GetHistoryAsync(int requestId)
        {
            return await _db.StatusHistories
                .AsNoTracking()
                .Include(h => h.ChangedBy)
                .Where(h => h.ServiceRequestId == requestId)
                .OrderBy(h => h.ChangedAt).ThenBy(h => h.Id)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<RequestComment>> GetCommentsAsync(int requestId, bool requesterVisibleOnly)
        {
            var query = _db.RequestComments
                .AsNoTracking()
                .Include(c => c.Author)
                .Where(c => c.ServiceRequestId == requestId);

            if (requesterVisibleOnly)
            {
                query = query.Where(c => c.VisibleToRequester);
            }

            return await query.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id).ToListAsync();
        }
    }
}

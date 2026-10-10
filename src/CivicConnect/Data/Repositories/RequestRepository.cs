namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson.</summary>
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _db;

        public RequestRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<ServiceRequest?> GetByIdAsync(int id)
        {
            // TODO: return _db.ServiceRequests.FirstOrDefaultAsync(r => r.Id == id). No Includes.
            throw new NotImplementedException();
        }

        public Task<ServiceRequest?> GetByIdWithDetailsAsync(int id)
        {
            // TODO: Include Category, Requester, AssignedStaff, StatusHistory(ThenInclude ChangedBy), Comments(ThenInclude Author).
            // Order StatusHistory by ChangedAt ascending and Comments by CreatedAt ascending when mapping (or in a split query).
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ServiceRequest>> ListByRequesterAsync(int requesterId)
        {
            // TODO: where RequesterId == requesterId, Include Category, AsNoTracking, order by SubmittedAt descending.
            throw new NotImplementedException();
        }

        public Task<PagedResult<ServiceRequest>> SearchAsync(RequestSearchCriteria criteria, IReadOnlyCollection<int>? visibleCategoryIds)
        {
            // TODO: build IQueryable. Apply: visibleCategoryIds (if not null), IncludeClosed (exclude Closed when false),
            // Text (ReferenceNo or Description contains), CategoryId, Status, AssignedStaffId.
            // Sort by criteria.SortBy (whitelist: SubmittedAt, Status, ReferenceNo, Category - ignore anything else) + Descending.
            // Count BEFORE paging; Skip((Page-1)*PageSize).Take(PageSize); clamp Page>=1 and 1<=PageSize<=100. Include Category, AssignedStaff. AsNoTracking.
            throw new NotImplementedException();
        }

        public Task AddAsync(ServiceRequest request)
        {
            // TODO: await _db.ServiceRequests.AddAsync(request). Do NOT call SaveChanges (the unit of work does).
            throw new NotImplementedException();
        }

        public Task<string> NextReferenceNoAsync()
        {
            // TODO: produce "CC-{year}-{sequence:D6}". Simple approach: count existing + 1 inside the transaction and rely on the
            // unique index on ReferenceNo as the safety net (retry once on conflict). Do not use Guid or random numbers.
            throw new NotImplementedException();
        }

        public void SetExpectedRowVersion(ServiceRequest request, byte[] rowVersion)
        {
            // TODO: _db.Entry(request).Property(r => r.RowVersion).OriginalValue = rowVersion;
            throw new NotImplementedException();
        }
    }
}

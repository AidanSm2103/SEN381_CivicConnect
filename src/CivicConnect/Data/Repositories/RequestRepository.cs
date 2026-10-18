namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson. Queries and persistence only - no business rules (D-006).</summary>
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _db;

        public RequestRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return _db.ServiceRequests.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<ServiceRequest?> GetByIdWithDetailsAsync(int id)
        {
            // Read-only snapshot (AsNoTracking): the service may filter the comment list for a requester without EF seeing a change.
            var request = await _db.ServiceRequests
                .AsNoTracking()
                .Include(r => r.Category)
                .Include(r => r.Requester)
                .Include(r => r.AssignedStaff)
                .Include(r => r.StatusHistory).ThenInclude(h => h.ChangedBy)
                .Include(r => r.Comments).ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request != null)
            {
                // Oldest first, as documented on the interface.
                request.StatusHistory = request.StatusHistory.OrderBy(h => h.ChangedAt).ThenBy(h => h.Id).ToList();
                request.Comments = request.Comments.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id).ToList();
            }

            return request;
        }

        public async Task<IReadOnlyList<ServiceRequest>> ListByRequesterAsync(int requesterId)
        {
            return await _db.ServiceRequests
                .AsNoTracking()
                .Include(r => r.Category)
                .Where(r => r.RequesterId == requesterId)
                .OrderByDescending(r => r.SubmittedAt).ThenByDescending(r => r.Id)
                .ToListAsync();
        }

        public async Task<PagedResult<ServiceRequest>> SearchAsync(RequestSearchCriteria criteria, IReadOnlyCollection<int>? visibleCategoryIds)
        {
            var query = _db.ServiceRequests.AsNoTracking().AsQueryable();

            if (visibleCategoryIds != null)
            {
                var ids = visibleCategoryIds.ToList();
                query = query.Where(r => ids.Contains(r.CategoryId));
            }

            if (!criteria.IncludeClosed && criteria.Status != RequestStatus.Closed)
            {
                query = query.Where(r => r.Status != RequestStatus.Closed);
            }

            if (!string.IsNullOrWhiteSpace(criteria.Text))
            {
                var text = criteria.Text.Trim();
                query = query.Where(r => r.ReferenceNo.Contains(text) || r.Description.Contains(text));
            }

            if (criteria.CategoryId.HasValue)
            {
                var categoryId = criteria.CategoryId.Value;
                query = query.Where(r => r.CategoryId == categoryId);
            }

            if (criteria.Status.HasValue)
            {
                var status = criteria.Status.Value;
                query = query.Where(r => r.Status == status);
            }

            if (criteria.AssignedStaffId.HasValue)
            {
                var staffId = criteria.AssignedStaffId.Value;
                query = query.Where(r => r.AssignedStaffId == staffId);
            }

            var total = await query.CountAsync();

            // Whitelisted sort keys only: anything else falls back to SubmittedAt (no injection / no surprise columns).
            var sortKey = (criteria.SortBy ?? string.Empty).Trim().ToLowerInvariant();
            IOrderedQueryable<ServiceRequest> ordered = sortKey switch
            {
                "status" => criteria.Descending ? query.OrderByDescending(r => r.Status) : query.OrderBy(r => r.Status),
                "referenceno" => criteria.Descending ? query.OrderByDescending(r => r.ReferenceNo) : query.OrderBy(r => r.ReferenceNo),
                "category" => criteria.Descending ? query.OrderByDescending(r => r.Category!.Name) : query.OrderBy(r => r.Category!.Name),
                _ => criteria.Descending ? query.OrderByDescending(r => r.SubmittedAt) : query.OrderBy(r => r.SubmittedAt)
            };

            var page = Math.Max(1, criteria.Page);
            var pageSize = Math.Clamp(criteria.PageSize, 1, 100);

            var items = await ordered
                .ThenBy(r => r.Id)
                .Include(r => r.Category)
                .Include(r => r.AssignedStaff)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ServiceRequest>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task AddAsync(ServiceRequest request)
        {
            await _db.ServiceRequests.AddAsync(request);
        }

        public async Task<string> NextReferenceNoAsync()
        {
            // "CC-{year}-{sequence:D6}".
            // SQL Server: the number comes from the database SEQUENCE dbo.ServiceRequestRefSeq (migration AddReferenceNoSequence).
            // A sequence hands out every value exactly once, even when many requests are submitted at the same moment
            // (DEF-008: the old "highest + 1" approach let concurrent submits pick the same number). Repays TD-001.
            // The unique index on ReferenceNo stays as a second safety net.
            var prefix = $"CC-{DateTime.UtcNow.Year}-";

            if (_db.Database.IsRelational())
            {
                await _db.Database.OpenConnectionAsync();
                try
                {
                    await using var command = _db.Database.GetDbConnection().CreateCommand();
                    command.CommandText = "SELECT NEXT VALUE FOR dbo.ServiceRequestRefSeq";
                    // Join the unit of work's transaction when there is one (EfUnitOfWork starts it before SubmitAsync runs).
                    var transaction = _db.Database.CurrentTransaction;
                    if (transaction != null)
                    {
                        command.Transaction = Microsoft.EntityFrameworkCore.Storage.DbContextTransactionExtensions.GetDbTransaction(transaction);
                    }

                    var next = Convert.ToInt64(await command.ExecuteScalarAsync());
                    return $"{prefix}{next:D6}";
                }
                finally
                {
                    await _db.Database.CloseConnectionAsync();
                }
            }

            // In-memory provider (automated tests only): it has no sequences, so keep "highest existing + 1".
            // Tests submit one request at a time, so the race in DEF-008 cannot occur there.
            var latest = await _db.ServiceRequests
                .AsNoTracking()
                .Where(r => r.ReferenceNo.StartsWith(prefix))
                .OrderByDescending(r => r.ReferenceNo)
                .Select(r => r.ReferenceNo)
                .FirstOrDefaultAsync();

            var current = 0;
            if (latest != null)
            {
                int.TryParse(latest.Substring(prefix.Length), out current);
            }

            return $"{prefix}{current + 1:D6}";
        }

        public void SetExpectedRowVersion(ServiceRequest request, byte[] rowVersion)
        {
            _db.Entry(request).Property(r => r.RowVersion).OriginalValue = rowVersion;
        }
    }
}

namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson. Read-only aggregates.</summary>
    public class ReportingRepository : IReportingRepository
    {
        private readonly AppDbContext _db;

        public ReportingRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<StatusCountRow>> GetStatusCountsAsync()
        {
            var counts = await _db.ServiceRequests
                .AsNoTracking()
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            // Every status appears, even with zero requests, in lifecycle order.
            return Enum.GetValues<RequestStatus>()
                .Select(s => new StatusCountRow
                {
                    Status = s,
                    Count = counts.FirstOrDefault(c => c.Status == s)?.Count ?? 0
                })
                .ToList();
        }

        public async Task<IReadOnlyList<StaffActivityRow>> GetStaffActivityAsync()
        {
            var grouped = await _db.ServiceRequests
                .AsNoTracking()
                .Where(r => r.AssignedStaffId != null)
                .GroupBy(r => r.AssignedStaffId!.Value)
                .Select(g => new
                {
                    StaffId = g.Key,
                    AssignedOpen = g.Count(r => r.Status == RequestStatus.Assigned || r.Status == RequestStatus.InProgress),
                    Resolved = g.Count(r => r.Status == RequestStatus.Resolved),
                    Closed = g.Count(r => r.Status == RequestStatus.Closed)
                })
                .ToListAsync();

            // All active staff are listed (zero rows included) so management can see idle capacity (FR-016).
            var staff = await _db.Users
                .AsNoTracking()
                .Where(u => u.Role == UserRole.Staff && u.IsActive)
                .Select(u => new { u.Id, u.DisplayName })
                .ToListAsync();

            var groupedIds = grouped.Select(g => g.StaffId).ToList();
            var extraNames = await _db.Users
                .AsNoTracking()
                .Where(u => groupedIds.Contains(u.Id))
                .Select(u => new { u.Id, u.DisplayName })
                .ToListAsync();

            var names = staff.Concat(extraNames).GroupBy(s => s.Id).ToDictionary(g => g.Key, g => g.First().DisplayName);

            return names
                .Select(n =>
                {
                    var g = grouped.FirstOrDefault(x => x.StaffId == n.Key);
                    return new StaffActivityRow
                    {
                        StaffId = n.Key,
                        StaffName = n.Value,
                        AssignedOpen = g?.AssignedOpen ?? 0,
                        Resolved = g?.Resolved ?? 0,
                        Closed = g?.Closed ?? 0
                    };
                })
                .OrderBy(r => r.StaffName)
                .ToList();
        }
    }
}

namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson.</summary>
    public class ReportingRepository : IReportingRepository
    {
        private readonly AppDbContext _db;

        public ReportingRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<IReadOnlyList<StatusCountRow>> GetStatusCountsAsync()
        {
            // TODO: GroupBy Status -> StatusCountRow. Then ensure every RequestStatus value appears (add Count = 0 rows). AsNoTracking.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<StaffActivityRow>> GetStaffActivityAsync()
        {
            // TODO: group requests with AssignedStaffId != null by staff; AssignedOpen = Assigned+InProgress, Resolved, Closed counts.
            // Join to Users for StaffName. AsNoTracking.
            throw new NotImplementedException();
        }
    }
}

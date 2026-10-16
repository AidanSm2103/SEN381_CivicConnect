namespace CivicConnect.ViewModels
{
    // OWNER: Michael-John Robinson. COMPLETE data holder.
    public class ManagementDashboardViewModel
    {
        public IReadOnlyList<StatusCountRow> StatusCounts { get; set; } = new List<StatusCountRow>();
        public IReadOnlyList<StaffActivityRow> StaffActivity { get; set; } = new List<StaffActivityRow>();
    }
}

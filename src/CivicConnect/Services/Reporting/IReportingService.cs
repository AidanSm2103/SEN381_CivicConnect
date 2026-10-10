namespace CivicConnect.Services
{
    /// <summary>Read-only management overview (FR-013, FR-014, FR-016). Filtering lists (FR-015) reuses IRequestService.SearchAsync.</summary>
    public interface IReportingService
    {
        /// <summary>Management role only (Failure/empty otherwise). Combines status counts and staff activity.</summary>
        Task<Result<ManagementOverview>> GetOverviewAsync();
    }
}

namespace CivicConnect.Data
{
    /// <summary>Read-only aggregate queries for the Management overview (FR-013, FR-014, FR-016). Never writes.</summary>
    public interface IReportingRepository
    {
        /// <summary>Count of requests per status (include statuses with zero count).</summary>
        Task<IReadOnlyList<StatusCountRow>> GetStatusCountsAsync();

        /// <summary>Per staff member: open assigned, resolved and closed counts.</summary>
        Task<IReadOnlyList<StaffActivityRow>> GetStaffActivityAsync();
    }
}

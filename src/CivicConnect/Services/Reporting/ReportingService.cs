namespace CivicConnect.Services
{
    /// <summary>OWNER: Michael-John Robinson. Never changes data.</summary>
    public class ReportingService : IReportingService
    {
        private readonly IReportingRepository _reporting;
        private readonly IAuthorisationService _authorisation;

        public ReportingService(IReportingRepository reporting, IAuthorisationService authorisation)
        {
            _reporting = reporting;
            _authorisation = authorisation;
        }

        public Task<Result<ManagementOverview>> GetOverviewAsync()
        {
            // TODO: _authorisation.EnsureInRole(UserRole.Management) -> map failure to Result<ManagementOverview>.Failure(code, message).
            // Then fill ManagementOverview from _reporting.GetStatusCountsAsync() and GetStaffActivityAsync().
            // NOTE: "overdue" (FR-014) is still undefined - leave out until the team defines it (see controlled-change record).
            throw new NotImplementedException();
        }
    }
}

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

        public async Task<Result<ManagementOverview>> GetOverviewAsync()
        {
            var role = _authorisation.EnsureInRole(UserRole.Management);
            if (role.Failed) return Result<ManagementOverview>.Failure(role.ErrorCode!, role.ErrorMessage!);

            // "Overdue" (FR-014) is not defined yet - deliberately left out until the team agrees a definition (controlled change).
            var overview = new ManagementOverview
            {
                StatusCounts = await _reporting.GetStatusCountsAsync(),
                StaffActivity = await _reporting.GetStaffActivityAsync()
            };
            return Result<ManagementOverview>.Success(overview);
        }
    }
}

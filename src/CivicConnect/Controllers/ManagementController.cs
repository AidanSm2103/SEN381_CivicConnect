namespace CivicConnect.Controllers
{
    /// <summary>OWNER: Michael-John Robinson. Read-only management overview (FR-013, FR-014, FR-015, FR-016). Management role only.</summary>
    [Authorize(Policy = PolicyNames.ManagementAccess)]
    public class ManagementController : AppControllerBase
    {
        private readonly IReportingService _reporting;
        private readonly IRequestService _requests;

        public ManagementController(IReportingService reporting, IRequestService requests)
        {
            _reporting = reporting;
            _requests = requests;
        }

        // GET /Management/Dashboard
        [HttpGet]
        public Task<IActionResult> Dashboard()
        {
            // TODO: result = _reporting.GetOverviewAsync(); failure -> MapFailure; success -> View(new ManagementDashboardViewModel{...}).
            throw new NotImplementedException();
        }

        // GET /Management/Requests?Status=...&CategoryId=...
        [HttpGet]
        public Task<IActionResult> Requests(RequestSearchCriteria criteria)
        {
            // TODO: page = _requests.SearchAsync(criteria) -> RequestSearchViewModel (same shape as StaffRequests/Index, IncludeClosed allowed).
            // Categories SelectList: use _requests.ListActiveCategoriesAsync().
            throw new NotImplementedException();
        }
    }
}

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
        public async Task<IActionResult> Dashboard()
        {
            var result = await _reporting.GetOverviewAsync();
            if (result.Failed) return MapFailure(result);

            return View(new ManagementDashboardViewModel
            {
                StatusCounts = result.Value!.StatusCounts,
                StaffActivity = result.Value.StaffActivity
            });
        }

        // GET /Management/Requests?Status=...&CategoryId=...
        [HttpGet]
        public async Task<IActionResult> Requests(RequestSearchCriteria criteria)
        {
            var page = await _requests.SearchAsync(criteria);
            var categories = await _requests.ListActiveCategoriesAsync();

            return View(new RequestSearchViewModel
            {
                Criteria = criteria,
                Results = RequestMapper.ToPagedListItems(page),
                Categories = new SelectList(categories, nameof(Category.Id), nameof(Category.Name), criteria.CategoryId)
            });
        }
    }
}

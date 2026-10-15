namespace CivicConnect.Controllers
{
    /// <summary>OWNER: Enrique de Sousa. Requester-facing pages (FR-001..FR-005). Default route = /ServiceRequests/Create.</summary>
    [Authorize(Policy = PolicyNames.RequesterAccess)]
    public class ServiceRequestsController : AppControllerBase
    {
        private readonly IRequestService _requests;

        public ServiceRequestsController(IRequestService requests)
        {
            _requests = requests;
        }

        // GET: /ServiceRequests/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateRequestViewModel { Categories = await BuildCategoryListAsync(null) };
            return View(vm);
        }

        // POST: /ServiceRequests/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await BuildCategoryListAsync(model.CategoryId);
                return View(model);
            }

            var result = await _requests.SubmitAsync(new SubmitRequestCommand
            {
                CategoryId = model.CategoryId,
                Description = model.Description
            });

            if (result.Failed)
            {
                if (IsAccessFailure(result)) return MapFailure(result);

                // Business/validation failure: show it on the form, keep what the user typed.
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "The request could not be submitted.");
                model.Categories = await BuildCategoryListAsync(model.CategoryId);
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = result.Value!.Id });
        }

        // GET: /ServiceRequests/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _requests.GetDetailsAsync(id);
            if (result.Failed) return MapFailure(result);

            return View(RequestMapper.ToDetails(result.Value!, new AvailableActions(), new List<User>()));
        }

        // GET: /ServiceRequests/MyRequests
        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            var mine = await _requests.ListMineAsync();
            return View(new MyRequestsViewModel
            {
                Requests = mine.Select(RequestMapper.ToListItem).ToList()
            });
        }

        // Only active categories are offered to requesters (FR-002).
        private async Task<SelectList> BuildCategoryListAsync(int? selectedId)
        {
            var categories = await _requests.ListActiveCategoriesAsync();
            return new SelectList(categories, nameof(Category.Id), nameof(Category.Name), selectedId);
        }
    }
}

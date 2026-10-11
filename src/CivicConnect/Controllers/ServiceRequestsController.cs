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
        public Task<IActionResult> Create()
        {
            // TODO: vm = new CreateRequestViewModel { Categories = await BuildCategoryListAsync(null) }; return View(vm).
            throw new NotImplementedException();
        }

        // POST: /ServiceRequests/Create
        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> Create(CreateRequestViewModel model)
        {
            // TODO: if !ModelState.IsValid -> reload Categories (keep selected id) and View(model).
            // Call _requests.SubmitAsync(new SubmitRequestCommand{...}). ValidationFailed -> ModelState error on CategoryId/Description + View(model).
            // Access failure -> MapFailure. Success -> RedirectToAction(nameof(Details), new { id = result.Value!.Id }).
            throw new NotImplementedException();
        }

        // GET: /ServiceRequests/Details/5
        [HttpGet]
        public Task<IActionResult> Details(int id)
        {
            // TODO: _requests.GetDetailsAsync(id); failure -> MapFailure; success -> View(RequestMapper.ToDetails(request, new AvailableActions(), empty staff list)).
            throw new NotImplementedException();
        }

        // GET: /ServiceRequests/MyRequests
        [HttpGet]
        public Task<IActionResult> MyRequests()
        {
            // TODO: _requests.ListMineAsync() -> map each with RequestMapper.ToListItem -> View(new MyRequestsViewModel{...}).
            throw new NotImplementedException();
        }

        // Only active categories are offered to requesters (FR-002).
        private Task<SelectList> BuildCategoryListAsync(int? selectedId)
        {
            // TODO: var categories = await _requests.ListActiveCategoriesAsync();
            // return new SelectList(categories, nameof(Category.Id), nameof(Category.Name), selectedId);
            throw new NotImplementedException();
        }
    }
}

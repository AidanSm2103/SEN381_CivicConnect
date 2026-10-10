namespace CivicConnect.Controllers
{
    /// <summary>OWNER: Enrique de Sousa. Staff work queue + request actions (FR-006..FR-012). Management may VIEW (Index/Details) but not act.</summary>
    [Authorize(Policy = PolicyNames.StaffOrManagement)]
    public class StaffRequestsController : AppControllerBase
    {
        private readonly IRequestService _requests;
        private readonly IRequestWorkflowService _workflow;

        public StaffRequestsController(IRequestService requests, IRequestWorkflowService workflow)
        {
            _requests = requests;
            _workflow = workflow;
        }

        // GET /StaffRequests?Status=...&CategoryId=...&Text=...
        [HttpGet]
        public Task<IActionResult> Index(RequestSearchCriteria criteria)
        {
            // TODO: page = _requests.SearchAsync(criteria); vm = new RequestSearchViewModel { Criteria, Results = RequestMapper.ToPagedListItems(page), Categories = active categories SelectList }.
            throw new NotImplementedException();
        }

        // GET /StaffRequests/Details/5
        [HttpGet]
        public Task<IActionResult> Details(int id)
        {
            // TODO: _requests.GetDetailsAsync(id) (failure -> MapFailure); actions = _workflow.GetAvailableActionsAsync(request);
            // staff = _workflow.ListAssignableStaffAsync(); return View(RequestMapper.ToDetails(request, actions, staff)).
            throw new NotImplementedException();
        }

        // POST /StaffRequests/Accept
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public Task<IActionResult> Accept(RequestActionForm form)
        {
            // TODO: _workflow.AcceptAsync(new AcceptRequestCommand{...}); then RedirectToActionAfter(result, form.RequestId).
            throw new NotImplementedException();
        }

        // POST /StaffRequests/Assign
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public Task<IActionResult> Assign(AssignForm form)
        {
            // TODO: _workflow.AssignAsync(new AssignRequestCommand{...}); then RedirectToActionAfter.
            throw new NotImplementedException();
        }

        // POST /StaffRequests/StartWork
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public Task<IActionResult> StartWork(RequestActionForm form)
        {
            // TODO: _workflow.ChangeStatusAsync(new ChangeStatusCommand{ TargetStatus = RequestStatus.InProgress, ...}); then RedirectToActionAfter.
            throw new NotImplementedException();
        }

        // POST /StaffRequests/Resolve
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public Task<IActionResult> Resolve(ResolveForm form)
        {
            // TODO: ModelState invalid -> TempData["Error"] + redirect to Details. _workflow.ResolveAsync(...); RedirectToActionAfter.
            throw new NotImplementedException();
        }

        // POST /StaffRequests/Close
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public Task<IActionResult> Close(CloseForm form)
        {
            // TODO: _workflow.CloseAsync(...); RedirectToActionAfter.
            throw new NotImplementedException();
        }

        // POST /StaffRequests/AddNote
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public Task<IActionResult> AddNote(AddNoteForm form)
        {
            // TODO: _workflow.AddNoteAsync(...); RedirectToActionAfter.
            throw new NotImplementedException();
        }

        // Shared tail of every POST: access failure -> MapFailure; business failure -> TempData["Error"]; success -> TempData["Success"]; then back to Details.
        private IActionResult RedirectToActionAfter(Result result, int requestId)
        {
            // TODO: implement exactly as described above, ending in RedirectToAction(nameof(Details), new { id = requestId }).
            throw new NotImplementedException();
        }
    }
}

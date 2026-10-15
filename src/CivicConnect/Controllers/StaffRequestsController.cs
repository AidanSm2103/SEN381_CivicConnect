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

        // GET /StaffRequests/Index?Status=...&CategoryId=...&Text=...
        [HttpGet]
        public async Task<IActionResult> Index(RequestSearchCriteria criteria)
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

        // GET /StaffRequests/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _requests.GetDetailsAsync(id);
            if (result.Failed) return MapFailure(result);

            var request = result.Value!;
            var actions = await _workflow.GetAvailableActionsAsync(request);
            var staff = await _workflow.ListAssignableStaffAsync();
            return View(RequestMapper.ToDetails(request, actions, staff));
        }

        // POST /StaffRequests/Accept
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public async Task<IActionResult> Accept(RequestActionForm form)
        {
            var result = await _workflow.AcceptAsync(new AcceptRequestCommand { RequestId = form.RequestId, RowVersion = form.RowVersion });
            return RedirectToActionAfter(result, form.RequestId, "Request accepted.");
        }

        // POST /StaffRequests/Assign
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public async Task<IActionResult> Assign(AssignForm form)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please choose a staff member.";
                return RedirectToAction(nameof(Details), new { id = form.RequestId });
            }

            var result = await _workflow.AssignAsync(new AssignRequestCommand
            {
                RequestId = form.RequestId,
                AssigneeStaffId = form.AssigneeStaffId,
                RowVersion = form.RowVersion
            });
            return RedirectToActionAfter(result, form.RequestId, "Request assigned.");
        }

        // POST /StaffRequests/StartWork
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public async Task<IActionResult> StartWork(RequestActionForm form)
        {
            var result = await _workflow.ChangeStatusAsync(new ChangeStatusCommand
            {
                RequestId = form.RequestId,
                TargetStatus = RequestStatus.InProgress,
                RowVersion = form.RowVersion
            });
            return RedirectToActionAfter(result, form.RequestId, "Work started.");
        }

        // POST /StaffRequests/Resolve
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public async Task<IActionResult> Resolve(ResolveForm form)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "A resolution note is required.";
                return RedirectToAction(nameof(Details), new { id = form.RequestId });
            }

            var result = await _workflow.ResolveAsync(new ResolveRequestCommand
            {
                RequestId = form.RequestId,
                ResolutionNote = form.ResolutionNote,
                RowVersion = form.RowVersion
            });
            return RedirectToActionAfter(result, form.RequestId, "Request resolved.");
        }

        // POST /StaffRequests/Close
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public async Task<IActionResult> Close(CloseForm form)
        {
            var result = await _workflow.CloseAsync(new CloseRequestCommand
            {
                RequestId = form.RequestId,
                Note = form.Note,
                RowVersion = form.RowVersion
            });
            return RedirectToActionAfter(result, form.RequestId, "Request closed.");
        }

        // POST /StaffRequests/AddNote
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = PolicyNames.StaffAccess)]
        public async Task<IActionResult> AddNote(AddNoteForm form)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "A note cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = form.RequestId });
            }

            var result = await _workflow.AddNoteAsync(new AddNoteCommand { RequestId = form.RequestId, Body = form.Body });
            return RedirectToActionAfter(result, form.RequestId, "Note added.");
        }

        // Shared tail of every POST: access failure -> MapFailure; business failure -> TempData["Error"]; success -> TempData["Success"]; then back to Details.
        private IActionResult RedirectToActionAfter(Result result, int requestId, string successMessage = "Done.")
        {
            if (result.Failed)
            {
                if (IsAccessFailure(result)) return MapFailure(result);
                TempData["Error"] = result.ErrorMessage;
            }
            else
            {
                TempData["Success"] = successMessage;
            }

            return RedirectToAction(nameof(Details), new { id = requestId });
        }
    }
}

namespace CivicConnect.ViewModels
{
    // OWNER: Enrique de Sousa. COMPLETE data holders - add properties only if a view needs them (tell the lead).

    /// <summary>Form model for submitting a request (FR-001, FR-002). Replaces binding the entity directly.</summary>
    public class CreateRequestViewModel
    {
        [Required(ErrorMessage = "Please select a category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [ValidateNever]
        public SelectList? Categories { get; set; }
    }

    /// <summary>One row in any request list (requester, staff or management).</summary>
    public class RequestListItemViewModel
    {
        public int Id { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public RequestStatus Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? AssignedStaffName { get; set; }
    }

    public class MyRequestsViewModel
    {
        public IReadOnlyList<RequestListItemViewModel> Requests { get; set; } = new List<RequestListItemViewModel>();
    }

    public class HistoryItemViewModel
    {
        public RequestStatus? FromStatus { get; set; }
        public RequestStatus ToStatus { get; set; }
        public string ChangedByName { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string? Note { get; set; }
    }

    public class CommentItemViewModel
    {
        public CommentKind Kind { get; set; }
        public string Body { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>Everything the Details page needs, for every role (FR-003, FR-008).</summary>
    public class RequestDetailsViewModel
    {
        public int Id { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RequestStatus Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string RequesterName { get; set; } = string.Empty;
        public string? AssignedStaffName { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public IReadOnlyList<HistoryItemViewModel> History { get; set; } = new List<HistoryItemViewModel>();
        public IReadOnlyList<CommentItemViewModel> Comments { get; set; } = new List<CommentItemViewModel>();
        public AvailableActions Actions { get; set; } = new AvailableActions();
        public SelectList? AssignableStaff { get; set; }
    }

    /// <summary>Staff and Management request list with filters (FR-006, FR-007, FR-015).</summary>
    public class RequestSearchViewModel
    {
        public RequestSearchCriteria Criteria { get; set; } = new RequestSearchCriteria();
        public PagedResult<RequestListItemViewModel> Results { get; set; } = new PagedResult<RequestListItemViewModel>();
        public SelectList? Categories { get; set; }
    }

    // ---- Staff action forms (POST bodies). RowVersion round-trips as a hidden base64 field. ----
    public class RequestActionForm
    {
        public int RequestId { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }

    public class AssignForm : RequestActionForm
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a staff member.")]
        public int AssigneeStaffId { get; set; }
    }

    public class ResolveForm : RequestActionForm
    {
        [Required(ErrorMessage = "A resolution note is required."), StringLength(2000)]
        public string ResolutionNote { get; set; } = string.Empty;
    }

    public class CloseForm : RequestActionForm
    {
        [StringLength(1000)]
        public string? Note { get; set; }
    }

    public class AddNoteForm
    {
        public int RequestId { get; set; }
        [Required, StringLength(2000)]
        public string Body { get; set; } = string.Empty;
    }
}

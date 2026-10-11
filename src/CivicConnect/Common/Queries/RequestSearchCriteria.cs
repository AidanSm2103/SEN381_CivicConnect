namespace CivicConnect.Common
{
    /// <summary>
    /// Filter/sort/paging options for listing requests (FR-006, FR-007, FR-015).
    /// Plain data holder - COMPLETE, do not change. Bound directly from query strings by StaffRequests/Management controllers.
    /// </summary>
    public class RequestSearchCriteria
    {
        public string? Text { get; set; }                 // matches ReferenceNo or Description
        public int? CategoryId { get; set; }
        public RequestStatus? Status { get; set; }
        public int? AssignedStaffId { get; set; }
        public bool IncludeClosed { get; set; } = false;  // FR-012: closed requests leave the active list
        public string SortBy { get; set; } = "SubmittedAt"; // allowed: SubmittedAt, Status, ReferenceNo, Category
        public bool Descending { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    /// <summary>One row of "requests per status" for the management overview (FR-014).</summary>
    public class StatusCountRow
    {
        public RequestStatus Status { get; init; }
        public int Count { get; init; }
    }

    /// <summary>One row of "work per staff member" for the management overview (FR-013, FR-016).</summary>
    public class StaffActivityRow
    {
        public int StaffId { get; init; }
        public string StaffName { get; init; } = string.Empty;
        public int AssignedOpen { get; init; }
        public int Resolved { get; init; }
        public int Closed { get; init; }
    }
}

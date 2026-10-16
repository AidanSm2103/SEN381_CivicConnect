namespace CivicConnect.Services
{
    /// <summary>Data passed from the workflow to INotificationService after a successful status change (D-003). No contact details included.</summary>
    public class StatusChangeNotice
    {
        public int RequestId { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public RequestStatus? FromStatus { get; set; }
        public RequestStatus ToStatus { get; set; }
        public int ChangedByUserId { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? StaffComment { get; set; }
    }

    /// <summary>Which buttons the UI may show for the current user on a given request. Computed by IRequestWorkflowService.</summary>
    public class AvailableActions
    {
        public bool CanAccept { get; set; }
        public bool CanAssign { get; set; }
        public bool CanStart { get; set; }
        public bool CanResolve { get; set; }
        public bool CanClose { get; set; }
        public bool CanAddNote { get; set; }
    }

    /// <summary>Management overview data (FR-013, FR-014, FR-016).</summary>
    public class ManagementOverview
    {
        public IReadOnlyList<StatusCountRow> StatusCounts { get; set; } = new List<StatusCountRow>();
        public IReadOnlyList<StaffActivityRow> StaffActivity { get; set; } = new List<StaffActivityRow>();
    }
}

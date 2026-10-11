namespace CivicConnect.Services
{
    /// <summary>
    /// Assignment &amp; Workflow module (FR-005, FR-009..FR-012, NFR-010). Every method that changes status:
    /// authorise -> load -> TransitionGuard -> state action -> (request update + StatusHistory + Feedback comment) in ONE transaction.
    /// </summary>
    public interface IRequestWorkflowService
    {
        /// <summary>Active staff for the assign dropdown (FR-009).</summary>
        Task<IReadOnlyList<User>> ListAssignableStaffAsync();

        /// <summary>Which action buttons apply to this request for the current user.</summary>
        Task<AvailableActions> GetAvailableActionsAsync(ServiceRequest request);

        /// <summary>Submitted -> Assigned to the given staff member. Failure(AlreadyAssigned) if someone already holds it; Failure(ConcurrencyConflict) on stale RowVersion (FR-009).</summary>
        Task<Result> AssignAsync(AssignRequestCommand command);

        /// <summary>Current staff member takes an unassigned request (same rules as AssignAsync with assignee = current user).</summary>
        Task<Result> AcceptAsync(AcceptRequestCommand command);

        /// <summary>Generic guarded transition used for Assigned -> InProgress (FR-010).</summary>
        Task<Result> ChangeStatusAsync(ChangeStatusCommand command);

        /// <summary>InProgress -> Resolved, storing the ResolutionNote comment (FR-011, FR-012).</summary>
        Task<Result> ResolveAsync(ResolveRequestCommand command);

        /// <summary>Resolved -> Closed; request leaves the active list but stays retrievable (FR-012).</summary>
        Task<Result> CloseAsync(CloseRequestCommand command);

        /// <summary>Staff-only internal note on a request (ResolutionNote, VisibleToRequester = false) (FR-011).</summary>
        Task<Result> AddNoteAsync(AddNoteCommand command);
    }
}

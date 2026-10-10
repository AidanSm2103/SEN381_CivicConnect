namespace CivicConnect.Services
{
    /// <summary>Requests module: submit, view, list, search (FR-001..FR-004, FR-006..FR-008, FR-015).</summary>
    public interface IRequestService
    {
        /// <summary>Active categories for the dropdown (FR-002).</summary>
        Task<IReadOnlyList<Category>> ListActiveCategoriesAsync();

        /// <summary>Create a request + first StatusHistory row in ONE transaction (FR-001, NFR-010). Requester role only.</summary>
        Task<Result<ServiceRequest>> SubmitAsync(SubmitRequestCommand command);

        /// <summary>Full details (with history/comments) if the current user may view it; Failure(NotFound) or Failure(Forbidden) otherwise (FR-008).</summary>
        Task<Result<ServiceRequest>> GetDetailsAsync(int requestId);

        /// <summary>The current requester's own requests, newest first (FR-003, FR-004).</summary>
        Task<IReadOnlyList<ServiceRequest>> ListMineAsync();

        /// <summary>Search/filter/sort/page. Staff are limited to their category scope; Management is unrestricted; Requesters are rejected (FR-006, FR-007, FR-015).</summary>
        Task<PagedResult<ServiceRequest>> SearchAsync(RequestSearchCriteria criteria);
    }
}

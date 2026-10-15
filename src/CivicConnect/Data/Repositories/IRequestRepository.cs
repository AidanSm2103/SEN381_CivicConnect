namespace CivicConnect.Data
{
    /// <summary>Data access for ServiceRequest. No business rules here - queries and persistence only (D-006).</summary>
    public interface IRequestRepository
    {
        /// <summary>Tracked entity (will be modified by workflow). Null if not found. No Includes.</summary>
        Task<ServiceRequest?> GetByIdAsync(int id);

        /// <summary>Read-only (untracked) request plus Category, Requester, AssignedStaff, StatusHistory (with ChangedBy) and Comments (with Author). Null if not found.</summary>
        Task<ServiceRequest?> GetByIdWithDetailsAsync(int id);

        /// <summary>All requests submitted by one requester, newest first, Category included (FR-003, FR-004).</summary>
        Task<IReadOnlyList<ServiceRequest>> ListByRequesterAsync(int requesterId);

        /// <summary>
        /// Filtered/sorted/paged search (FR-006, FR-007, FR-015).
        /// visibleCategoryIds: null = no category restriction (management); otherwise only those categories (staff scope).
        /// </summary>
        Task<PagedResult<ServiceRequest>> SearchAsync(RequestSearchCriteria criteria, IReadOnlyCollection<int>? visibleCategoryIds);

        /// <summary>Stage a new request for insert (does not save).</summary>
        Task AddAsync(ServiceRequest request);

        /// <summary>Generate the next unique reference, e.g. "CC-2026-000123" (FR-001).</summary>
        Task<string> NextReferenceNoAsync();

        /// <summary>Tell EF which RowVersion the user saw, so a concurrent change raises a concurrency conflict on save (FR-009).</summary>
        void SetExpectedRowVersion(ServiceRequest request, byte[] rowVersion);
    }
}

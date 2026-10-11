namespace CivicConnect.Tests.Fakes
{
    /// <summary>In-memory IRequestRepository for unit tests. OWNER: Aidan Smith.</summary>
    public class FakeRequestRepository : IRequestRepository
    {
        public List<ServiceRequest> Items { get; } = new();

        public Task<ServiceRequest?> GetByIdAsync(int id)
        {
            // TODO: return Items.FirstOrDefault(r => r.Id == id) wrapped in Task.FromResult.
            throw new NotImplementedException();
        }

        public Task<ServiceRequest?> GetByIdWithDetailsAsync(int id)
        {
            // TODO: same as GetByIdAsync.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ServiceRequest>> ListByRequesterAsync(int requesterId)
        {
            // TODO: filter Items by RequesterId, newest first.
            throw new NotImplementedException();
        }

        public Task<PagedResult<ServiceRequest>> SearchAsync(RequestSearchCriteria criteria, IReadOnlyCollection<int>? visibleCategoryIds)
        {
            // TODO: minimal in-memory version of the real filters (category scope, status, closed exclusion) - enough for the tests that use it.
            throw new NotImplementedException();
        }

        public Task AddAsync(ServiceRequest request)
        {
            // TODO: assign Id = Items.Count + 1 if 0; add to Items; return Task.CompletedTask.
            throw new NotImplementedException();
        }

        public Task<string> NextReferenceNoAsync()
        {
            // TODO: return $"CC-2026-{Items.Count + 1:D6}".
            throw new NotImplementedException();
        }

        public void SetExpectedRowVersion(ServiceRequest request, byte[] rowVersion)
        {
            // TODO: no-op (or record the value so a test can assert it was set).
            throw new NotImplementedException();
        }
    }
}

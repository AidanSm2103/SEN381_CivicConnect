namespace CivicConnect.Tests.Fakes
{
    // In-memory IRequestRepository for unit tests
    public class FakeRequestRepository : IRequestRepository
    {
        public List<ServiceRequest> Items { get; } = new();
        public List<(ServiceRequest Request, byte[] RowVersion)> ExpectedRowVersions { get; } = new();

        public Task<ServiceRequest?> GetByIdAsync(int id) => Task.FromResult(Items.FirstOrDefault(r => r.Id == id));

        public Task<ServiceRequest?> GetByIdWithDetailsAsync(int id) => GetByIdAsync(id);

        public Task<IReadOnlyList<ServiceRequest>> ListByRequesterAsync(int requesterId)
        {
            IReadOnlyList<ServiceRequest> list = Items.Where(r => r.RequesterId == requesterId).OrderByDescending(r => r.SubmittedAt).ToList();
            return Task.FromResult(list);
        }

        public Task<PagedResult<ServiceRequest>> SearchAsync(RequestSearchCriteria criteria, IReadOnlyCollection<int>? visibleCategoryIds)
        {
            IEnumerable<ServiceRequest> query = Items;
            if (visibleCategoryIds != null) query = query.Where(r => visibleCategoryIds.Contains(r.CategoryId));
            if (!criteria.IncludeClosed) query = query.Where(r => r.Status != RequestStatus.Closed);
            if (criteria.CategoryId.HasValue) query = query.Where(r => r.CategoryId == criteria.CategoryId.Value);
            if (criteria.Status.HasValue) query = query.Where(r => r.Status == criteria.Status.Value);
            if (criteria.AssignedStaffId.HasValue) query = query.Where(r => r.AssignedStaffId == criteria.AssignedStaffId.Value);
            if (!string.IsNullOrWhiteSpace(criteria.Text))
                query = query.Where(r => r.ReferenceNo.Contains(criteria.Text, StringComparison.OrdinalIgnoreCase)
                                      || r.Description.Contains(criteria.Text, StringComparison.OrdinalIgnoreCase));

            var all = (criteria.Descending ? query.OrderByDescending(r => r.SubmittedAt) : query.OrderBy(r => r.SubmittedAt)).ToList();
            var page = Math.Max(1, criteria.Page);
            var size = Math.Clamp(criteria.PageSize, 1, 100);
            return Task.FromResult(new PagedResult<ServiceRequest>
            {
                Items = all.Skip((page - 1) * size).Take(size).ToList(),
                TotalCount = all.Count,
                Page = page,
                PageSize = size
            });
        }

        public Task AddAsync(ServiceRequest request)
        {
            if (request.Id == 0) request.Id = Items.Count + 1;
            Items.Add(request);
            return Task.CompletedTask;
        }

        public Task<string> NextReferenceNoAsync() => Task.FromResult($"CC-2026-{Items.Count + 1:D6}");

        public void SetExpectedRowVersion(ServiceRequest request, byte[] rowVersion)
        {
            ExpectedRowVersions.Add((request, rowVersion));
        }
    }
}


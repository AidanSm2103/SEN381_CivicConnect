namespace CivicConnect.Tests.Fakes
{
    /// <summary>Builders for ready-made entities so tests stay short. OWNER: Aidan Smith.</summary>
    public static class TestData
    {
        public static ServiceRequest Request(RequestStatus status = RequestStatus.Submitted, int id = 1, int requesterId = 1, int categoryId = 1)
        {
            // TODO: return a ServiceRequest with the given values, ReferenceNo = $"CC-2026-{id:D6}", Description = "Test request", RowVersion = new byte[] { 1 }.
            throw new NotImplementedException();
        }

        public static User Staff(int id = 2, params int[] categoryIds)
        {
            // TODO: return a User { Id = id, Role = Staff, IsActive = true, Email = $"staff{id}@test", DisplayName = $"Staff {id}" } (scopes live in FakeUserRepository.CategoryScopes).
            throw new NotImplementedException();
        }
    }
}

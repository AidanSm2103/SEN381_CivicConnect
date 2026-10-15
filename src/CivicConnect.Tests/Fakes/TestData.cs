namespace CivicConnect.Tests.Fakes
{
    // Builders for ready-made entities so tests stay short
    public static class TestData
    {
        public static ServiceRequest Request(RequestStatus status = RequestStatus.Submitted, int id = 1, int requesterId = 1, int categoryId = 1)
        {
            return new ServiceRequest
            {
                Id = id,
                ReferenceNo = $"CC-2026-{id:D6}",
                RequesterId = requesterId,
                CategoryId = categoryId,
                Description = "Test request",
                Status = status,
                SubmittedAt = new DateTime(2026, 10, 19, 8, 0, 0, DateTimeKind.Utc),
                RowVersion = new byte[] { 1 }
            };
        }

        public static User Staff(int id = 2, params int[] categoryIds)
        {
            // Scopes live in FakeUserRepository.CategoryScopes - use RegisterStaff to add the user and its scopes together.
            return new User
            {
                Id = id,
                Role = UserRole.Staff,
                IsActive = true,
                Email = $"staff{id}@test",
                DisplayName = $"Staff {id}"
            };
        }

        // Adds a staff user and their category scopes to the fake repository in one call.
        public static User RegisterStaff(FakeUserRepository users, int id, params int[] categoryIds)
        {
            var staff = Staff(id, categoryIds);
            users.Users.Add(staff);
            users.CategoryScopes[id] = categoryIds.ToList();
            return staff;
        }
    }
}



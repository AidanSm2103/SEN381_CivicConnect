namespace CivicConnect.Tests
{
    /// <summary>OWNER: Aidan Smith. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class SecurityIntegrationTests
    {

        [Fact(Skip = "TODO")]
        [Trait("Category", "Integration"), Trait("Req", "NFR-002"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public Task Anonymous_IsRedirectedToLogin_ForProtectedPages()
        {
            // TODO: HttpClient with AllowAutoRedirect=false; GET /StaffRequests, /Management/Dashboard, /ServiceRequests/Create -> 302 to /Account/Login.
            return Task.CompletedTask;
        }

        [Fact(Skip = "TODO")]
        [Trait("Category", "Integration"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public Task Requester_CannotReachStaffOrManagementPages()
        {
            // TODO: log in as requester; GET /StaffRequests and /Management/Dashboard -> redirect to /Account/AccessDenied (or 403).
            return Task.CompletedTask;
        }
    }
}

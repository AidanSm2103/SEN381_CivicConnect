namespace CivicConnect.Tests
{
    /// <summary>OWNER: Michael-John Robinson. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class RequestsApiTests
    {

        [Fact(Skip = "TODO")]
        [Trait("Category", "Integration"), Trait("Req", "FR-001"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public Task Post_Create_AsRequester_PersistsRequestAndRedirectsToDetails()
        {
            // TODO: using CivicConnectFactory: log in as the seeded requester, GET Create (read antiforgery token), POST valid form,
            // assert 302 to /ServiceRequests/Details/{id} and the row exists with ReferenceNo + one StatusHistory.
            return Task.CompletedTask;
        }

        [Fact(Skip = "TODO")]
        [Trait("Category", "Integration"), Trait("Req", "FR-009"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public Task Accept_TwoStaffAtOnce_ExactlyOneWins()
        {
            // TODO: two logged-in staff HttpClients POST /StaffRequests/Accept with the same RowVersion in parallel; one redirect with success, the other conflict message.
            return Task.CompletedTask;
        }
    }
}

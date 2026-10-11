namespace CivicConnect.Tests
{
    /// <summary>OWNER: Aidan Smith. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class AuthorisationServiceTests
    {

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public Task Staff_CannotViewRequestOutsideCategoryScope()
        {
            // TODO: staff scoped to category 1, request in category 2 -> EnsureCanViewAsync Failure(Forbidden).
            return Task.CompletedTask;
        }

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public Task Requester_CannotViewAnotherRequestersRequest()
        {
            // TODO: requester 1 vs request.RequesterId == 5 -> Forbidden.
            return Task.CompletedTask;
        }
    }
}

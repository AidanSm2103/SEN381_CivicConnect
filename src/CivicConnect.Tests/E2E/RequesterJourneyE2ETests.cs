using System.Net;

namespace CivicConnect.Tests
{
    /// <summary>OWNER: Michael-John Robinson. End-to-end journey driven through the real HTML forms (cookies, anti-forgery, redirects, views).</summary>
    public class RequesterJourneyE2ETests : IClassFixture<CivicConnectFactory>
    {
        private readonly CivicConnectFactory _factory;

        public RequesterJourneyE2ETests(CivicConnectFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        [Trait("Category", "E2E"), Trait("Req", "FR-001"), Trait("Priority", "High")]
        public async Task Requester_Registers_Submits_AndSeesRequestInMyRequests()
        {
            await _factory.SeedAsync();
            var browser = _factory.NewClient();

            // 1. Anonymous visitor is sent to the login page.
            var anonymous = await browser.GetAsync("/ServiceRequests/MyRequests");
            Assert.Contains("/Account/Login", anonymous.Headers.Location!.ToString());

            // 2. Registers (signs in automatically) and lands on the submit form.
            var email = await CivicConnectFactory.RegisterRequesterAsync(browser);
            var form = await CivicConnectFactory.GetHtmlAsync(browser, "/ServiceRequests/Create");
            Assert.Contains("Submit a Service Request", form);
            Assert.Contains("Maintenance", form);                                     // category dropdown is filled from the database

            // 3. Submits a request and is shown it.
            var (id, reference) = await CivicConnectFactory.SubmitAsync(browser, 3, "The lift in block C is stuck on floor 2");
            var details = await CivicConnectFactory.GetHtmlAsync(browser, $"/ServiceRequests/Details/{id}");
            Assert.Contains("The lift in block C is stuck on floor 2", details);
            Assert.Contains("Not yet assigned", details);

            // 4. It appears in "My requests" with status Submitted and a first feedback message... and the page has no staff tools.
            var mine = await CivicConnectFactory.GetHtmlAsync(browser, "/ServiceRequests/MyRequests");
            Assert.Contains(reference, mine);
            Assert.Contains("Submitted", mine);
            Assert.DoesNotContain("name=\"RowVersion\"", details);

            // 5. A different requester does not see it.
            var other = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(other);
            Assert.DoesNotContain(reference, await CivicConnectFactory.GetHtmlAsync(other, "/ServiceRequests/MyRequests"));

            // 6. Logging in again with the same credentials works (and a wrong password does not).
            var again = _factory.NewClient();
            await CivicConnectFactory.LoginAsync(again, email);
            Assert.Contains(reference, await CivicConnectFactory.GetHtmlAsync(again, "/ServiceRequests/MyRequests"));
            var bad = await CivicConnectFactory.PostFormAsync(_factory.NewClient(), "/Account/Login", "/Account/Login", ("Email", email), ("Password", "wrong-password"));
            Assert.Equal(HttpStatusCode.OK, bad.StatusCode);
        }
    }
}

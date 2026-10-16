using System.Net;

namespace CivicConnect.Tests
{
    /// <summary>OWNER: Michael-John Robinson. The full lifecycle through the real pages: Submitted -> Assigned -> InProgress -> Resolved -> Closed (FR-005, FR-009..FR-012).</summary>
    public class StaffResolutionJourneyE2ETests : IClassFixture<CivicConnectFactory>
    {
        private readonly CivicConnectFactory _factory;

        public StaffResolutionJourneyE2ETests(CivicConnectFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        [Trait("Category", "E2E"), Trait("Req", "FR-005"), Trait("Priority", "High")]
        public async Task Staff_Accepts_Starts_Resolves_Closes_AndRequesterSeesFeedback()
        {
            await _factory.SeedAsync();
            const string internalNote = "Replaced the faulty fuse in cabinet 7";
            var requester = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(requester);
            var (id, reference) = await CivicConnectFactory.SubmitAsync(requester, 1, "Power keeps tripping in the hall");

            var staff = _factory.NewClient();
            await CivicConnectFactory.LoginAsync(staff, CivicConnectFactory.StaffEmail);

            // The new request is in the staff work queue.
            Assert.Contains(reference, await CivicConnectFactory.GetHtmlAsync(staff, "/StaffRequests/Index"));

            Assert.Contains("Request accepted.", await CivicConnectFactory.StaffActionAsync(staff, id, "Accept"));
            Assert.Contains("Work started.", await CivicConnectFactory.StaffActionAsync(staff, id, "StartWork"));

            // Resolving without a note is refused and changes nothing.
            var refused = await CivicConnectFactory.StaffActionAsync(staff, id, "Resolve", ("ResolutionNote", ""));
            Assert.Contains("resolution note is required", refused);
            Assert.DoesNotContain("Request resolved.", refused);

            Assert.Contains("Request resolved.", await CivicConnectFactory.StaffActionAsync(staff, id, "Resolve", ("ResolutionNote", internalNote)));
            Assert.Contains("Request closed.", await CivicConnectFactory.StaffActionAsync(staff, id, "Close", ("Note", "")));

            // The requester sees feedback for every step, but never the staff-only resolution note.
            var details = await CivicConnectFactory.GetHtmlAsync(requester, $"/ServiceRequests/Details/{id}");
            Assert.Contains("has been accepted and assigned", details);
            Assert.Contains("Work has started", details);
            Assert.Contains("has been resolved", details);
            Assert.Contains("has been closed", details);
            Assert.DoesNotContain(internalNote, details);

            // Staff can see the note and the full history.
            var staffView = await CivicConnectFactory.GetHtmlAsync(staff, $"/StaffRequests/Details/{id}");
            Assert.Contains(internalNote, staffView);
            Assert.Contains("Closed", staffView);

            // Closed requests leave the active list but stay retrievable (FR-012).
            Assert.DoesNotContain(reference, await CivicConnectFactory.GetHtmlAsync(staff, "/StaffRequests/Index"));
            Assert.Contains(reference, await CivicConnectFactory.GetHtmlAsync(staff, "/StaffRequests/Index?IncludeClosed=true"));
            Assert.Equal(HttpStatusCode.OK, (await staff.GetAsync($"/StaffRequests/Details/{id}")).StatusCode);

            // A closed request cannot be pushed any further.
            var again = await CivicConnectFactory.StaffActionAsync(staff, id, "Close", ("Note", ""));
            Assert.DoesNotContain("Request closed.", again);
        }
    }
}

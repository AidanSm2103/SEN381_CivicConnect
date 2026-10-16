using System.Net;
using Microsoft.EntityFrameworkCore;

namespace CivicConnect.Tests
{
    /// <summary>
    /// OWNER: Michael-John Robinson. API / integration tests: real HTTP requests through the whole pipeline
    /// (cookie auth, anti-forgery, controllers, services, repositories, Razor views) on the in-memory test database.
    /// </summary>
    public class RequestsApiTests : IClassFixture<CivicConnectFactory>
    {
        private readonly CivicConnectFactory _factory;

        public RequestsApiTests(CivicConnectFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        [Trait("Category", "Integration"), Trait("Req", "FR-001"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task Post_Create_AsRequester_PersistsRequestAndRedirectsToDetails()
        {
            await _factory.SeedAsync();
            var requester = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(requester);

            var response = await CivicConnectFactory.PostFormAsync(requester, "/ServiceRequests/Create", "/ServiceRequests/Create",
                ("CategoryId", "1"), ("Description", "Broken streetlight on Main Road"));

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/ServiceRequests/Details/", response.Headers.Location!.ToString());

            var html = await CivicConnectFactory.GetHtmlAsync(requester, response.Headers.Location.ToString());
            var reference = CivicConnectFactory.ReferenceNo(html);
            var saved = await _factory.WithDbAsync(db => db.ServiceRequests.Include(r => r.StatusHistory).FirstAsync(r => r.ReferenceNo == reference));
            Assert.Equal(RequestStatus.Submitted, saved.Status);
            Assert.Equal("Broken streetlight on Main Road", saved.Description);
            var history = Assert.Single(saved.StatusHistory);
            Assert.Null(history.FromStatus);
            Assert.Equal(RequestStatus.Submitted, history.ToStatus);
        }

        [Fact]
        [Trait("Category", "Integration"), Trait("Req", "FR-002"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Post_Create_WithAnInactiveOrUnknownCategory_IsRejectedAndSavesNothing()
        {
            await _factory.SeedAsync();
            var requester = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(requester);
            var before = await _factory.WithDbAsync(db => db.ServiceRequests.CountAsync());

            var response = await CivicConnectFactory.PostFormAsync(requester, "/ServiceRequests/Create", "/ServiceRequests/Create",
                ("CategoryId", "9999"), ("Description", "Crafted POST with a category that does not exist"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);                  // form re-displayed with the error
            Assert.Contains("valid category", await response.Content.ReadAsStringAsync());
            Assert.Equal(before, await _factory.WithDbAsync(db => db.ServiceRequests.CountAsync()));
        }

        [Fact]
        [Trait("Category", "Integration"), Trait("Req", "NFR-002"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Post_Create_WithoutAntiForgeryToken_IsRejected()
        {
            await _factory.SeedAsync();
            var requester = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(requester);

            var response = await requester.PostAsync("/ServiceRequests/Create", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["CategoryId"] = "1",
                ["Description"] = "Forged cross-site post"
            }));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // Two staff open the same request, both see the Accept button; both click. The second click must NOT also succeed (FR-009).
        [Fact]
        [Trait("Category", "Integration"), Trait("Req", "FR-009"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Accept_TwoStaffAtOnce_ExactlyOneWins()
        {
            await _factory.SeedAsync();
            var requester = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(requester);
            var (id, _) = await CivicConnectFactory.SubmitAsync(requester, 1, "Two staff will race for this one");

            var first = _factory.NewClient();
            var second = _factory.NewClient();
            await CivicConnectFactory.LoginAsync(first, CivicConnectFactory.StaffEmail);
            await CivicConnectFactory.LoginAsync(second, CivicConnectFactory.Staff2Email);
            var stalePage = await CivicConnectFactory.GetHtmlAsync(second, $"/StaffRequests/Details/{id}");   // loaded BEFORE the first accept
            Assert.Contains("Accept", stalePage);

            var firstResult = await CivicConnectFactory.StaffActionAsync(first, id, "Accept");
            Assert.Contains("Request accepted.", firstResult);

            // The second staff member clicks Accept on the page they loaded earlier (same RowVersion as the first one used).
            var secondResponse = await CivicConnectFactory.PostFormAsync(second, $"/StaffRequests/Details/{id}", "/StaffRequests/Accept",
                ("RequestId", id.ToString()), ("RowVersion", CivicConnectFactory.RowVersion(stalePage)));
            Assert.Equal(HttpStatusCode.Redirect, secondResponse.StatusCode);
            var secondResult = await CivicConnectFactory.GetHtmlAsync(second, secondResponse.Headers.Location!.ToString());

            Assert.DoesNotContain("Request accepted.", secondResult);
            Assert.True(secondResult.Contains("already been assigned") || secondResult.Contains("Someone else changed"), "The loser should see a conflict message.");

            var saved = await _factory.WithDbAsync(db => db.ServiceRequests.FirstAsync(r => r.Id == id));
            var winner = await _factory.WithDbAsync(db => db.Users.FirstAsync(u => u.Id == saved.AssignedStaffId));
            Assert.Equal(CivicConnectFactory.StaffEmail, winner.Email);
            Assert.Equal(RequestStatus.Assigned, saved.Status);
        }

        [Fact]
        [Trait("Category", "Integration"), Trait("Req", "FR-008"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Requester_CannotOpenAnotherRequestersRequest()
        {
            await _factory.SeedAsync();
            var owner = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(owner);
            var (id, _) = await CivicConnectFactory.SubmitAsync(owner, 1, "Private request");

            var stranger = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(stranger);
            var response = await stranger.GetAsync($"/ServiceRequests/Details/{id}");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/Account/AccessDenied", response.Headers.Location!.ToString());
        }

        [Fact]
        [Trait("Category", "Integration"), Trait("Req", "FR-006"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task StaffQueue_OnlyListsRequestsInTheirCategories()
        {
            await _factory.SeedAsync();
            var requester = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(requester);
            var (_, inScope) = await CivicConnectFactory.SubmitAsync(requester, 2, "IT problem (staff2 handles category 2)");
            var (outId, outOfScope) = await CivicConnectFactory.SubmitAsync(requester, 5, "Security problem (category 5, nobody seeded handles it)");

            var staff2 = _factory.NewClient();
            await CivicConnectFactory.LoginAsync(staff2, CivicConnectFactory.Staff2Email);
            var queue = await CivicConnectFactory.GetHtmlAsync(staff2, "/StaffRequests/Index?PageSize=100");

            Assert.Contains(inScope, queue);
            Assert.DoesNotContain(outOfScope, queue);
            var direct = await staff2.GetAsync($"/StaffRequests/Details/{outId}");
            Assert.Equal(HttpStatusCode.Redirect, direct.StatusCode);                 // out of scope -> access denied, not "not found"
            Assert.Contains("/Account/AccessDenied", direct.Headers.Location!.ToString());
        }

        [Fact]
        [Trait("Category", "Integration"), Trait("Req", "FR-013"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task Management_Dashboard_ShowsCounts_ButManagementCannotAct()
        {
            await _factory.SeedAsync();
            var requester = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(requester);
            var (id, _) = await CivicConnectFactory.SubmitAsync(requester, 1, "Visible on the dashboard");

            var manager = _factory.NewClient();
            await CivicConnectFactory.LoginAsync(manager, CivicConnectFactory.ManagerEmail);
            var dashboard = await CivicConnectFactory.GetHtmlAsync(manager, "/Management/Dashboard");
            Assert.Contains("Requests by status", dashboard);
            Assert.Contains("Submitted:", dashboard);
            Assert.Contains("Sam Staff", dashboard);                                  // staff activity table lists staff

            var details = await CivicConnectFactory.GetHtmlAsync(manager, $"/StaffRequests/Details/{id}");
            Assert.DoesNotContain("name=\"RowVersion\"", details);                    // read-only: no action forms at all

            var attempt = await CivicConnectFactory.PostFormAsync(manager, $"/StaffRequests/Details/{id}", "/StaffRequests/Accept",
                ("RequestId", id.ToString()), ("RowVersion", ""));
            Assert.Equal(HttpStatusCode.Redirect, attempt.StatusCode);
            Assert.Contains("/Account/AccessDenied", attempt.Headers.Location!.ToString());
            Assert.Equal(RequestStatus.Submitted, (await _factory.WithDbAsync(db => db.ServiceRequests.FirstAsync(r => r.Id == id))).Status);
        }

        [Fact]
        [Trait("Category", "Integration"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Staff_CannotOpenTheManagementDashboard()
        {
            await _factory.SeedAsync();
            var staff = _factory.NewClient();
            await CivicConnectFactory.LoginAsync(staff, CivicConnectFactory.StaffEmail);

            var response = await staff.GetAsync("/Management/Dashboard");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/Account/AccessDenied", response.Headers.Location!.ToString());
        }
    }
}

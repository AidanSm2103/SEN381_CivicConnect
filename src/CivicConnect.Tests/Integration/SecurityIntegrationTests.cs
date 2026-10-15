namespace CivicConnect.Tests
{
    // Real HTTP pipeline, in memory (NFR-002, NFR-003)
    public class SecurityIntegrationTests : IClassFixture<CivicConnectFactory>
    {
        private readonly CivicConnectFactory _factory;

        public SecurityIntegrationTests(CivicConnectFactory factory)
        {
            _factory = factory;
        }

        // "Testing" environment => Program.cs does not run the Development database seeder
        private WebApplicationFactory<Program> Isolated() =>
            _factory.WithWebHostBuilder(builder => builder.UseSetting("environment", "Testing"));

        [Theory]
        [InlineData("/ServiceRequests/Create")]
        [InlineData("/ServiceRequests/MyRequests")]
        [InlineData("/StaffRequests/Index")]
        [InlineData("/Management/Dashboard")]
        [Trait("Category", "Integration"), Trait("Req", "NFR-002"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Anonymous_IsRedirectedToLogin_ForProtectedPages(string path)
        {
            var client = Isolated().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var response = await client.GetAsync(path);

            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
        }

        [Fact]
        [Trait("Category", "Integration"), Trait("Req", "NFR-002"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task LoginAndRegisterPages_ArePublic()
        {
            var client = Isolated().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            Assert.Equal(System.Net.HttpStatusCode.OK, (await client.GetAsync("/Account/Login")).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.OK, (await client.GetAsync("/Account/Register")).StatusCode);
        }

        [Fact]
        [Trait("Category", "Integration"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Requester_CannotReachStaffOrManagementPages()
        {
            await _factory.SeedAsync();
            var client = _factory.NewClient();
            await CivicConnectFactory.RegisterRequesterAsync(client);

            foreach (var path in new[] { "/StaffRequests/Index", "/Management/Dashboard" })
            {
                var response = await client.GetAsync(path);

                Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
                Assert.Contains("/Account/AccessDenied", response.Headers.Location?.ToString());
            }
        }
    }
}


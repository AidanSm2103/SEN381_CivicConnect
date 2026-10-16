using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace CivicConnect.Tests
{
    /// <summary>
    /// Boots the real app in-memory for API/integration/E2E tests. OWNER: Michael-John Robinson.
    /// The SQL Server database is swapped for the EF Core in-memory provider so the tests need no database server
    /// (they run on the CI runner and on every laptop). The REAL controllers, services, repositories, cookie
    /// authentication, anti-forgery and Razor views are all exercised - only the storage engine differs.
    /// Not covered by the in-memory provider: SQL constraints, real RowVersion generation, real transactions (documented as a test limitation in PED §23).
    /// </summary>
    public class CivicConnectFactory : WebApplicationFactory<Program>
    {
        public const string Password = "Test-Password-123!";
        public const string StaffEmail = "staff@civicconnect.test";      // handles categories 1-3
        public const string Staff2Email = "staff2@civicconnect.test";    // handles categories 1-2
        public const string ManagerEmail = "manager@civicconnect.test";

        private readonly string _databaseName = "CivicConnectTests_" + Guid.NewGuid().ToString("N");
        private readonly InMemoryDatabaseRoot _root = new();
        private readonly SemaphoreSlim _seedLock = new(1, 1);
        private bool _seeded;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // "Testing": Program.cs does not run the Development seeder, so nothing touches LocalDB.
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var options = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (options != null) services.Remove(options);
                services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(_databaseName, _root));
            });
        }

        // ---------- data ----------

        /// <summary>Creates the schema + reference data once and adds the staff/manager accounts. Safe to call repeatedly.</summary>
        public async Task SeedAsync()
        {
            await _seedLock.WaitAsync();
            try
            {
                if (_seeded) return;
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
                await db.Database.EnsureCreatedAsync();

                var staff = new User { Email = StaffEmail, DisplayName = "Sam Staff", PasswordHash = hasher.Hash(Password), Role = UserRole.Staff };
                var staff2 = new User { Email = Staff2Email, DisplayName = "Sofia Staff", PasswordHash = hasher.Hash(Password), Role = UserRole.Staff };
                var manager = new User { Email = ManagerEmail, DisplayName = "Mo Manager", PasswordHash = hasher.Hash(Password), Role = UserRole.Management };
                db.Users.AddRange(staff, staff2, manager);
                await db.SaveChangesAsync();

                foreach (var c in new[] { 1, 2, 3 }) db.StaffCategoryScopes.Add(new StaffCategoryScope { UserId = staff.Id, CategoryId = c });
                foreach (var c in new[] { 1, 2 }) db.StaffCategoryScopes.Add(new StaffCategoryScope { UserId = staff2.Id, CategoryId = c });
                await db.SaveChangesAsync();
                _seeded = true;
            }
            finally { _seedLock.Release(); }
        }

        /// <summary>Runs a query/assert against the test database.</summary>
        public async Task<T> WithDbAsync<T>(Func<AppDbContext, Task<T>> action)
        {
            using var scope = Services.CreateScope();
            return await action(scope.ServiceProvider.GetRequiredService<AppDbContext>());
        }

        // ---------- HTTP helpers (keep the test classes short) ----------

        public HttpClient NewClient() => CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        public static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}".Substring(0, 24);

        /// <summary>Reads the anti-forgery token from a rendered page (the cookie half is kept by the client).</summary>
        public static string Token(string html)
        {
            var m = Regex.Match(html, "<input[^>]*name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
            Assert.True(m.Success, "No anti-forgery token found on the page.");
            return WebUtility.HtmlDecode(m.Groups[1].Value);
        }

        /// <summary>The hidden RowVersion a staff action form carries (empty string when the page has no such form).</summary>
        public static string RowVersion(string html)
        {
            var m = Regex.Match(html, "name=\"RowVersion\"\\s+value=\"([^\"]*)\"");
            return m.Success ? WebUtility.HtmlDecode(m.Groups[1].Value) : string.Empty;
        }

        public static string ReferenceNo(string html)
        {
            var m = Regex.Match(html, @"CC-\d{4}-\d{6}");
            Assert.True(m.Success, "No reference number found on the page.");
            return m.Value;
        }

        public static async Task<string> GetHtmlAsync(HttpClient client, string path)
        {
            var response = await client.GetAsync(path);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>GET the form page for its token, then POST the fields with that token.</summary>
        public static async Task<HttpResponseMessage> PostFormAsync(HttpClient client, string tokenPage, string postPath, params (string Key, string Value)[] fields)
        {
            var html = await GetHtmlAsync(client, tokenPage);
            var form = fields.Select(f => new KeyValuePair<string, string>(f.Key, f.Value)).ToList();
            form.Add(new KeyValuePair<string, string>("__RequestVerificationToken", Token(html)));
            return await client.PostAsync(postPath, new FormUrlEncodedContent(form));
        }

        public static async Task LoginAsync(HttpClient client, string email, string password = Password)
        {
            var response = await PostFormAsync(client, "/Account/Login", "/Account/Login", ("Email", email), ("Password", password));
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);   // a failed login re-renders the form (200)
        }

        /// <summary>Registers a brand-new requester through the real form; the response signs the client in.</summary>
        public static async Task<string> RegisterRequesterAsync(HttpClient client)
        {
            var email = $"{Unique("req")}@civicconnect.test";
            var response = await PostFormAsync(client, "/Account/Register", "/Account/Register",
                ("Email", email), ("DisplayName", "Test Requester"), ("Password", Password), ("ConfirmPassword", Password));
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            return email;
        }

        /// <summary>Submits a request as the (already signed-in) requester and returns its id and reference number.</summary>
        public static async Task<(int Id, string ReferenceNo)> SubmitAsync(HttpClient requester, int categoryId, string description)
        {
            var response = await PostFormAsync(requester, "/ServiceRequests/Create", "/ServiceRequests/Create",
                ("CategoryId", categoryId.ToString()), ("Description", description));
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            var location = response.Headers.Location!.ToString();
            var id = int.Parse(Regex.Match(location, @"/ServiceRequests/Details/(\d+)").Groups[1].Value);
            var html = await GetHtmlAsync(requester, location);
            return (id, ReferenceNo(html));
        }

        /// <summary>Posts a staff action from the request's Details page (fresh token + fresh RowVersion, like a browser would).</summary>
        public static async Task<string> StaffActionAsync(HttpClient staff, int requestId, string action, params (string Key, string Value)[] extra)
        {
            var details = $"/StaffRequests/Details/{requestId}";
            var html = await GetHtmlAsync(staff, details);
            var fields = new List<(string, string)> { ("RequestId", requestId.ToString()), ("RowVersion", RowVersion(html)) };
            fields.AddRange(extra);
            var response = await PostFormAsync(staff, details, $"/StaffRequests/{action}", fields.ToArray());
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            return await GetHtmlAsync(staff, response.Headers.Location!.ToString());   // the page after the redirect, with its flash message
        }
    }
}

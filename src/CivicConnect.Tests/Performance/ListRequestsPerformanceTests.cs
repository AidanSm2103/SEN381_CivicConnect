using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace CivicConnect.Tests
{
    /// <summary>
    /// NFR-001: 95% of standard actions complete within 2 seconds at up to 50 concurrent users.
    /// Operation under test: request submission (FR-001) followed by viewing it (FR-003/FR-008) and listing own requests.
    ///
    /// This is a LIVE test: it drives a RUNNING copy of the app over HTTP (local Release build, or the staging site) and is
    /// SKIPPED automatically unless PERF_BASE_URL is set. CI excludes Category=Performance.
    ///
    /// How to run (PowerShell):
    ///   $env:PERF_BASE_URL = "https://localhost:7000"        # or the staging URL
    ///   $env:PERF_EMAIL    = "requester@civicconnect.test"   # a seeded REQUESTER account
    ///   $env:PERF_PASSWORD = "(the seed password)"
    ///   $env:PERF_USERS    = "50"                            # optional, default 50
    ///   $env:PERF_ITERATIONS = "5"                           # optional, default 5
    ///   dotnet test src/CivicConnect.Tests --filter "Category=Performance" --logger "console;verbosity=detailed"
    /// A results file is written to TestResults/perf-results.txt as evidence.
    /// </summary>
    public class SubmitRequestPerformanceTests
    {
        private const double TargetMilliseconds = 2000;
        private readonly ITestOutputHelper _output;

        public SubmitRequestPerformanceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private sealed record Sample(string Action, double Milliseconds, bool Ok);

        [Fact]
        [Trait("Category", "Performance"), Trait("Req", "NFR-001"), Trait("Priority", "High")]
        public async Task SubmitViewList_95thPercentile_UnderTwoSeconds_With50ConcurrentUsers()
        {
            var baseUrl = Environment.GetEnvironmentVariable("PERF_BASE_URL");
            var email = Environment.GetEnvironmentVariable("PERF_EMAIL");
            var password = Environment.GetEnvironmentVariable("PERF_PASSWORD");
            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _output.WriteLine("Skipped: set PERF_BASE_URL, PERF_EMAIL and PERF_PASSWORD to run the live performance test.");
                return;   // not a failure: this test only means something against a running instance
            }

            var users = int.TryParse(Environment.GetEnvironmentVariable("PERF_USERS"), out var u) && u > 0 ? u : 50;
            var iterations = int.TryParse(Environment.GetEnvironmentVariable("PERF_ITERATIONS"), out var i) && i > 0 ? i : 5;

            var samples = new List<Sample>();
            var gate = new object();
            var overall = Stopwatch.StartNew();

            await Task.WhenAll(Enumerable.Range(0, users).Select(user =>
                RunVirtualUserAsync(baseUrl!, email!, password!, iterations, samples, gate)));

            overall.Stop();
            Report(users, iterations, overall.Elapsed, samples);

            var failures = samples.Count(s => !s.Ok);
            var measured = samples.Where(s => s.Action != "login").Select(s => s.Milliseconds).OrderBy(x => x).ToList();
            var p95 = Percentile(measured, 0.95);

            Assert.Equal(0, failures);
            Assert.True(p95 < TargetMilliseconds, $"p95 was {p95:F0} ms; NFR-001 target is {TargetMilliseconds:F0} ms.");
        }

        private static async Task RunVirtualUserAsync(string baseUrl, string email, string password, int iterations, List<Sample> samples, object gate)
        {
            var handler = new HttpClientHandler { AllowAutoRedirect = false, UseCookies = true };
            // The staging/dev certificate may be self-signed; this is a load test against a known target, not a production client.
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            using var client = new HttpClient(handler) { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(30) };

            void Add(string action, double ms, bool ok)
            {
                lock (gate) samples.Add(new Sample(action, ms, ok));
            }

            async Task<(HttpResponseMessage Response, double Ms)> TimeAsync(Func<Task<HttpResponseMessage>> send)
            {
                var sw = Stopwatch.StartNew();
                var response = await send();
                sw.Stop();
                return (response, sw.Elapsed.TotalMilliseconds);
            }

            try
            {
                // login (recorded separately - password hashing is deliberately slow and happens once per session, not per action)
                var token = await ReadTokenAsync(client, "/Account/Login");
                var login = await TimeAsync(() => client.PostAsync("/Account/Login", Form(("Email", email), ("Password", password), ("__RequestVerificationToken", token))));
                Add("login", login.Ms, login.Response.StatusCode == HttpStatusCode.Redirect);
                if (login.Response.StatusCode != HttpStatusCode.Redirect) return;

                for (var n = 0; n < iterations; n++)
                {
                    var createToken = await ReadTokenAsync(client, "/ServiceRequests/Create");

                    var submit = await TimeAsync(() => client.PostAsync("/ServiceRequests/Create", Form(
                        ("CategoryId", "1"),
                        ("Description", $"Performance test request {Guid.NewGuid()}"),
                        ("__RequestVerificationToken", createToken))));
                    var submitted = submit.Response.StatusCode == HttpStatusCode.Redirect && submit.Response.Headers.Location != null;
                    Add("submit", submit.Ms, submitted);
                    if (!submitted) continue;

                    var details = await TimeAsync(() => client.GetAsync(submit.Response.Headers.Location!));
                    Add("details", details.Ms, details.Response.StatusCode == HttpStatusCode.OK);

                    var list = await TimeAsync(() => client.GetAsync("/ServiceRequests/MyRequests"));
                    Add("list", list.Ms, list.Response.StatusCode == HttpStatusCode.OK);
                }
            }
            catch (Exception)
            {
                Add("exception", 0, false);
            }
        }

        private static FormUrlEncodedContent Form(params (string Key, string Value)[] fields) =>
            new(fields.Select(f => new KeyValuePair<string, string>(f.Key, f.Value)));

        private static async Task<string> ReadTokenAsync(HttpClient client, string url)
        {
            var html = await client.GetStringAsync(url);
            var match = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private static double Percentile(List<double> sortedValues, double fraction)
        {
            if (sortedValues.Count == 0) return double.NaN;
            var index = (int)Math.Ceiling(fraction * sortedValues.Count) - 1;
            return sortedValues[Math.Clamp(index, 0, sortedValues.Count - 1)];
        }

        private void Report(int users, int iterations, TimeSpan duration, List<Sample> samples)
        {
            var lines = new List<string>
            {
                $"NFR-001 performance run - {DateTime.UtcNow:u}",
                $"Virtual users: {users}, iterations per user: {iterations}, total wall time: {duration.TotalSeconds:F1} s",
                $"Target: p95 < {TargetMilliseconds:F0} ms, 0 errors",
                string.Empty,
                "action      count  errors    avg ms    p95 ms    max ms"
            };

            foreach (var group in samples.Where(s => s.Action != "exception").GroupBy(s => s.Action).OrderBy(g => g.Key))
            {
                var times = group.Select(s => s.Milliseconds).OrderBy(x => x).ToList();
                lines.Add($"{group.Key,-10} {group.Count(),6} {group.Count(s => !s.Ok),7} {times.Average(),9:F0} {Percentile(times, 0.95),9:F0} {times.Max(),9:F0}");
            }

            var measured = samples.Where(s => s.Action != "login" && s.Action != "exception").Select(s => s.Milliseconds).OrderBy(x => x).ToList();
            lines.Add(string.Empty);
            lines.Add($"All measured actions (excluding login): n={measured.Count}, p95={Percentile(measured, 0.95):F0} ms");
            lines.Add($"Exceptions: {samples.Count(s => s.Action == "exception")}");

            foreach (var line in lines) _output.WriteLine(line);

            Directory.CreateDirectory("TestResults");
            File.WriteAllLines(Path.Combine("TestResults", "perf-results.txt"), lines);
        }
    }
}


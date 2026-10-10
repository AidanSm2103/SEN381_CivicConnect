namespace CivicConnect.Tests
{
    /// <summary>
    /// Boots the real app in-memory for API/integration tests. OWNER: Michael-John Robinson.
    /// </summary>
    public class CivicConnectFactory : WebApplicationFactory<Program>
    {
        // TODO(Michael-John): override ConfigureWebHost(IWebHostBuilder builder):
        //   - UseEnvironment("Testing");
        //   - replace the "DefaultConnection" connection string with a separate test database (e.g. "CivicConnectDb_Test") via builder.UseSetting(...);
        //   - make sure the DB is created/migrated once per test run and reset between tests that need a clean state.
        // Add helper methods here (e.g. LoginAsync(HttpClient, email, password)) so test classes stay short.
    }
}

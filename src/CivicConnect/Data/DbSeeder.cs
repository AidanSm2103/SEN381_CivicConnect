namespace CivicConnect.Data
{
    /// <summary>
    /// Development-only demo data. Called from Program.cs when Environment is Development.
    /// OWNER: Michael-John Robinson. SAFE NO-OP until implemented, so the app still starts.
    /// </summary>
    public static class DbSeeder
    {
        public static Task SeedAsync(IServiceProvider services)
        {
            // TODO(Michael-John): create a scope, get AppDbContext + IPasswordHasher, apply pending migrations
            // (db.Database.MigrateAsync()), then - ONLY if Users is empty - insert:
            //   requester@civicconnect.test (Requester), staff@civicconnect.test (Staff, scoped to categories 1-3),
            //   staff2@civicconnect.test (Staff, scoped to categories 4-6), manager@civicconnect.test (Management).
            // Passwords come from configuration key "SeedPassword" (user-secrets), NEVER hard-coded or committed (secrets rule).
            // Also insert 3-5 sample ServiceRequests in different statuses with matching StatusHistory rows for demos/tests.
            return Task.CompletedTask;
        }
    }
}

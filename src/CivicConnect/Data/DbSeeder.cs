namespace CivicConnect.Data
{
    // Demo data. Called from Program.cs in Development, or in Staging when SeedDemoData=true.
    // Runs in Development, and in Staging only when SeedDemoData=true; never in Production. 

    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;
            var db = sp.GetRequiredService<AppDbContext>();
            var hasher = sp.GetRequiredService<IPasswordHasher>();
            var configuration = sp.GetRequiredService<IConfiguration>();
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");
            var requests = sp.GetRequiredService<IRequestRepository>();

            await db.Database.MigrateAsync();

            if (await db.Users.AnyAsync())
            {
                return; // already seeded - never touch existing data
            }

            var seedPassword = configuration["SeedPassword"];
            if (string.IsNullOrWhiteSpace(seedPassword))
            {
                logger.LogWarning("SeedPassword is not set (dotnet user-secrets set \"SeedPassword\" \"<value>\"). Demo users were NOT created.");
                return;
            }

            var hash = hasher.Hash(seedPassword);
            var now = DateTime.UtcNow;

            var requester = new User { Email = "requester@civicconnect.test", DisplayName = "Rita Requester", PasswordHash = hash, Role = UserRole.Requester, CreatedAt = now };
            var staff = new User { Email = "staff@civicconnect.test", DisplayName = "Sam Staff", PasswordHash = hash, Role = UserRole.Staff, CreatedAt = now };
            var staff2 = new User { Email = "staff2@civicconnect.test", DisplayName = "Sofia Staff", PasswordHash = hash, Role = UserRole.Staff, CreatedAt = now };
            var manager = new User { Email = "manager@civicconnect.test", DisplayName = "Mo Manager", PasswordHash = hash, Role = UserRole.Management, CreatedAt = now };
            db.Users.AddRange(requester, staff, staff2, manager);
            await db.SaveChangesAsync();

            foreach (var categoryId in new[] { 1, 2, 3 })
                db.StaffCategoryScopes.Add(new StaffCategoryScope { UserId = staff.Id, CategoryId = categoryId });
            foreach (var categoryId in new[] { 4, 5, 6 })
                db.StaffCategoryScopes.Add(new StaffCategoryScope { UserId = staff2.Id, CategoryId = categoryId });
            await db.SaveChangesAsync();

            var year = now.Year;
            var samples = new[]
            {
                (Cat: 1, Desc: "Broken streetlight outside the community hall.", Status: RequestStatus.Submitted, Assignee: (User?)null),
                (Cat: 2, Desc: "Council office Wi-Fi drops every afternoon.", Status: RequestStatus.Assigned, Assignee: staff),
                (Cat: 3, Desc: "Main entrance door will not lock after hours.", Status: RequestStatus.InProgress, Assignee: staff),
                (Cat: 4, Desc: "Overflowing bins behind the library.", Status: RequestStatus.Resolved, Assignee: staff2),
                (Cat: 5, Desc: "Gate camera at the depot is offline.", Status: RequestStatus.Closed, Assignee: staff2)
            };

            var seq = 1;
            foreach (var s in samples)
            {
                var submitted = now.AddDays(-10 + seq);
                var request = new ServiceRequest
                {
                    ReferenceNo = await requests.NextReferenceNoAsync(),
                    CategoryId = s.Cat,
                    Description = s.Desc,
                    Status = s.Status,
                    SubmittedAt = submitted,
                    RequesterId = requester.Id,
                    AssignedStaffId = s.Assignee?.Id,
                    AssignedAt = s.Assignee != null ? submitted.AddHours(2) : null,
                    ResolvedAt = s.Status >= RequestStatus.Resolved ? submitted.AddDays(2) : null,
                    ClosedAt = s.Status == RequestStatus.Closed ? submitted.AddDays(3) : null
                };

                // One history row per step actually taken, so the audit trail matches the current status.
                var when = submitted;
                RequestStatus? from = null;
                foreach (var step in Enum.GetValues<RequestStatus>().Where(v => v <= s.Status))
                {
                    var actor = step == RequestStatus.Submitted ? requester.Id : (s.Assignee?.Id ?? manager.Id);
                    request.StatusHistory.Add(new StatusHistory
                    {
                        FromStatus = from,
                        ToStatus = step,
                        ChangedByUserId = actor,
                        ChangedAt = when,
                        Note = step == RequestStatus.Submitted ? "Request submitted." : $"Moved to {step}."
                    });
                    from = step;
                    when = when.AddHours(6);
                }

                db.ServiceRequests.Add(request);
                await db.SaveChangesAsync();
                seq++;
            }

            await db.SaveChangesAsync();
            logger.LogInformation("Demo data seeded (4 users, {Count} requests).", samples.Length);
        }
    }
}

using CivicConnect.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace CivicConnect.Tests
{
    // Guards against a service that is injected but never registered 
    public class DependencyInjectionTests : IClassFixture<CivicConnectFactory>
    {
        private readonly CivicConnectFactory _factory;

        public DependencyInjectionTests(CivicConnectFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        [Trait("Category", "Integration"), Trait("Priority", "High")]
        public void EveryRegisteredServiceAndControllerCanBeResolved()
        {
            using var host = _factory.WithWebHostBuilder(builder => builder.UseSetting("environment", "Testing"));
            using var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;

            // Services and repositories
            Assert.NotNull(sp.GetRequiredService<IRequestService>());
            Assert.NotNull(sp.GetRequiredService<IRequestWorkflowService>());
            Assert.NotNull(sp.GetRequiredService<IAuthService>());
            Assert.NotNull(sp.GetRequiredService<IAuthorisationService>());
            Assert.NotNull(sp.GetRequiredService<IAuditService>());
            Assert.NotNull(sp.GetRequiredService<INotificationService>());
            Assert.NotNull(sp.GetRequiredService<IReportingService>());
            Assert.NotNull(sp.GetRequiredService<IPasswordHasher>());
            Assert.NotNull(sp.GetRequiredService<ICurrentUser>());
            Assert.NotNull(sp.GetRequiredService<IUnitOfWork>());
            Assert.NotNull(sp.GetRequiredService<IClock>());
            Assert.NotNull(sp.GetRequiredService<IRequestRepository>());
            Assert.NotNull(sp.GetRequiredService<IUserRepository>());
            Assert.NotNull(sp.GetRequiredService<ICategoryRepository>());
            Assert.NotNull(sp.GetRequiredService<IAuditLogRepository>());
            Assert.NotNull(sp.GetRequiredService<IReportingRepository>());

            // Every controller is activated through DI at request time
            foreach (var controller in new[]
            {
                typeof(AccountController), typeof(ServiceRequestsController), typeof(StaffRequestsController), typeof(ManagementController)
            })
            {
                Assert.NotNull(ActivatorUtilities.CreateInstance(sp, controller));
            }
        }
    }
}


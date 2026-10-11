namespace CivicConnect.Tests
{
    /// <summary>OWNER: Aidan Smith. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class DependencyInjectionTests
    {

        [Fact(Skip = "TODO")]
        [Trait("Category", "Integration"), Trait("Priority", "High")]
        public void EveryRegisteredServiceCanBeResolved()
        {
            // TODO: using CivicConnectFactory, create a scope and resolve IRequestService, IRequestWorkflowService, IAuthService,
            // IAuthorisationService, IAuditService, INotificationService, IReportingService and every repository. Guards against missing DI registrations.
        }
    }
}

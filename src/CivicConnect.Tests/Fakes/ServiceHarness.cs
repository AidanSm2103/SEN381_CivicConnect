namespace CivicConnect.Tests.Fakes
{
    /// Wires the REAL services to the in-memory fakes so service-level tests stay short 
    public class ServiceHarness
    {
        public FakeRequestRepository Requests { get; } = new();
        public FakeUserRepository Users { get; } = new();
        public FakeCategoryRepository Categories { get; } = new();
        public FakeAuditLogRepository AuditLog { get; } = new();
        public FakeUnitOfWork UnitOfWork { get; } = new();
        public FakeClock Clock { get; } = new();
        public FakeCurrentUser Current { get; }

        public AuthorisationService Authorisation { get; }
        public AuditService Audit { get; }
        public InProcessNotificationService Notifications { get; }
        public TransitionGuard Guard { get; }
        public RequestService RequestService { get; }
        public RequestWorkflowService Workflow { get; }

        public ServiceHarness(FakeCurrentUser current)
        {
            Current = current;
            Users.Users.Add(new User { Id = 1, Role = UserRole.Requester, IsActive = true, Email = "req@test", DisplayName = "Requester 1" });
            Users.Users.Add(new User { Id = 3, Role = UserRole.Management, IsActive = true, Email = "mgr@test", DisplayName = "Manager 3" });
            TestData.RegisterStaff(Users, 2, 1);
            TestData.RegisterStaff(Users, 4, 1, 2);
            TestData.RegisterStaff(Users, 5, 2);

            Authorisation = new AuthorisationService(Current, Users);
            Audit = new AuditService(AuditLog, Current, Clock);
            Notifications = new InProcessNotificationService(AuditLog);
            Guard = new TransitionGuard(Authorisation);
            RequestService = new RequestService(Requests, Categories, Audit, Authorisation, UnitOfWork, Current, Clock);
            Workflow = new RequestWorkflowService(Requests, Users, Audit, Notifications, Authorisation, Guard, UnitOfWork, Current, Clock, AuditLog);
        }

        public ServiceRequest AddRequest(RequestStatus status = RequestStatus.Submitted, int categoryId = 1, int requesterId = 1, int? assignedTo = null)
        {
            var r = TestData.Request(status, Requests.Items.Count + 1, requesterId, categoryId);
            r.AssignedStaffId = assignedTo;
            Requests.Items.Add(r);
            return r;
        }
    }
}

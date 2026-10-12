namespace CivicConnect.Tests
{
    // Verifies the single RBAC enforcement point (NFR-003) all use in-memory fakes 
    public class AuthorisationServiceTests
    {
        private static (AuthorisationService Service, FakeUserRepository Users) Build(FakeCurrentUser user)
        {
            var users = new FakeUserRepository();
            TestData.RegisterStaff(users, 2, 1);   // staff 2 may work category 1 only
            return (new AuthorisationService(user, users), users);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Staff_CannotViewRequestOutsideCategoryScope()
        {
            var (service, _) = Build(FakeCurrentUser.AsStaff(2));
            var outOfScope = TestData.Request(categoryId: 2);

            var result = await service.EnsureCanViewAsync(outOfScope);

            Assert.True(result.Failed);
            Assert.Equal(ErrorCodes.Forbidden, result.ErrorCode);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task Staff_CanViewAndWorkRequestInsideCategoryScope()
        {
            var (service, _) = Build(FakeCurrentUser.AsStaff(2));
            var inScope = TestData.Request(categoryId: 1);

            Assert.True((await service.EnsureCanViewAsync(inScope)).Succeeded);
            Assert.True((await service.EnsureCanWorkAsync(inScope)).Succeeded);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Requester_CannotViewAnotherRequestersRequest()
        {
            var (service, _) = Build(FakeCurrentUser.AsRequester(1));
            var someoneElses = TestData.Request(requesterId: 5);

            var result = await service.EnsureCanViewAsync(someoneElses);

            Assert.Equal(ErrorCodes.Forbidden, result.ErrorCode);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task Requester_CanViewOwnRequest_ButCannotWorkIt()
        {
            var (service, _) = Build(FakeCurrentUser.AsRequester(1));
            var own = TestData.Request(requesterId: 1);

            Assert.True((await service.EnsureCanViewAsync(own)).Succeeded);
            Assert.Equal(ErrorCodes.Forbidden, (await service.EnsureCanWorkAsync(own)).ErrorCode);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Management_CanViewAnyRequest_ButIsReadOnly()
        {
            var (service, _) = Build(FakeCurrentUser.AsManager(3));
            var any = TestData.Request(categoryId: 2, requesterId: 9);

            Assert.True((await service.EnsureCanViewAsync(any)).Succeeded);
            Assert.Equal(ErrorCodes.Forbidden, (await service.EnsureCanWorkAsync(any)).ErrorCode);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-002"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Anonymous_IsUnauthenticated_ForEveryCheck()
        {
            var (service, _) = Build(FakeCurrentUser.Anonymous());
            var request = TestData.Request();

            Assert.Equal(ErrorCodes.Unauthenticated, service.EnsureAuthenticated().ErrorCode);
            Assert.Equal(ErrorCodes.Unauthenticated, service.EnsureInRole(UserRole.Requester).ErrorCode);
            Assert.Equal(ErrorCodes.Unauthenticated, (await service.EnsureCanViewAsync(request)).ErrorCode);
            Assert.Equal(ErrorCodes.Unauthenticated, (await service.EnsureCanWorkAsync(request)).ErrorCode);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task VisibleCategoryIds_DependOnRole()
        {
            var (staffService, _) = Build(FakeCurrentUser.AsStaff(2));
            var (managerService, _) = Build(FakeCurrentUser.AsManager(3));
            var (requesterService, _) = Build(FakeCurrentUser.AsRequester(1));

            Assert.Equal(new[] { 1 }, (await staffService.GetVisibleCategoryIdsAsync())!.ToArray());
            Assert.Null(await managerService.GetVisibleCategoryIdsAsync());           // null = unrestricted
            Assert.Empty((await requesterService.GetVisibleCategoryIdsAsync())!);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public void EnsureInRole_RejectsAWrongRole()
        {
            var (service, _) = Build(FakeCurrentUser.AsRequester(1));

            Assert.Equal(ErrorCodes.Forbidden, service.EnsureInRole(UserRole.Staff, UserRole.Management).ErrorCode);
            Assert.True(service.EnsureInRole(UserRole.Requester).Succeeded);
        }
    }
}


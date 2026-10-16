namespace CivicConnect.Tests
{
    /// <summary>OWNER: Enrique de Sousa. The guard checks WHO (role + category scope) and WHAT (legal transition) before anything changes.</summary>
    public class TransitionGuardTests
    {
        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Management_CannotChangeStatus()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsManager());
            var request = h.AddRequest(RequestStatus.Submitted);

            var result = await h.Guard.CheckAsync(request, RequestStatus.Assigned);

            Assert.True(result.Failed);
            Assert.Equal(ErrorCodes.Forbidden, result.ErrorCode);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Staff_InvalidTransition_IsRejected()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.Submitted);

            var result = await h.Guard.CheckAsync(request, RequestStatus.Resolved);

            Assert.True(result.Failed);
            Assert.Equal(ErrorCodes.InvalidTransition, result.ErrorCode);
            Assert.Contains("Submitted", result.ErrorMessage);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public async Task Staff_OutsideCategoryScope_IsForbiddenEvenForALegalMove()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));      // staff 2 only works category 1
            var request = h.AddRequest(RequestStatus.Submitted, categoryId: 2);

            var result = await h.Guard.CheckAsync(request, RequestStatus.Assigned);

            Assert.Equal(ErrorCodes.Forbidden, result.ErrorCode);
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public async Task Staff_InScope_LegalMove_IsAllowed()
        {
            var h = new ServiceHarness(FakeCurrentUser.AsStaff(2));
            var request = h.AddRequest(RequestStatus.Assigned, categoryId: 1);

            Assert.True((await h.Guard.CheckAsync(request, RequestStatus.InProgress)).Succeeded);
        }
    }
}

namespace CivicConnect.Tests
{
    /// <summary>OWNER: Michael-John Robinson. Black-box decision tables over the status workflow and the role/scope rules.</summary>
    public class TransitionMatrixBlackBoxTests
    {
        // Technique: Decision table - every (current status, action) pair against the allowed flow Submitted->Assigned->InProgress->Resolved->Closed (FR-010).
        [Theory]
        [InlineData(RequestStatus.Submitted, RequestStatus.Assigned, true)]
        [InlineData(RequestStatus.Assigned, RequestStatus.InProgress, true)]
        [InlineData(RequestStatus.InProgress, RequestStatus.Resolved, true)]
        [InlineData(RequestStatus.Resolved, RequestStatus.Closed, true)]
        [InlineData(RequestStatus.Submitted, RequestStatus.Closed, false)]
        [InlineData(RequestStatus.Resolved, RequestStatus.Submitted, false)]
        [InlineData(RequestStatus.Closed, RequestStatus.Assigned, false)]
        [Trait("Category", "BlackBox"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Technique", "DecisionTable")]
        public void Transition_AllowedOrRejected(RequestStatus from, RequestStatus to, bool allowed)
        {
            Assert.Equal(allowed, RequestStateFactory.For(from).CanMoveTo(to));
        }

        [Fact]
        [Trait("Category", "BlackBox"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Technique", "DecisionTable"), Trait("Type", "Negative")]
        public void EveryStatusPair_OnlyTheNextStepIsAllowed()
        {
            var all = Enum.GetValues<RequestStatus>();
            foreach (var from in all)
                foreach (var to in all)
                    Assert.Equal((int)to == (int)from + 1, RequestStateFactory.For(from).CanMoveTo(to));
        }

        [Theory]
        [InlineData("requester", false)]
        [InlineData("staff-in-scope", true)]
        [InlineData("staff-out-of-scope", false)]
        [InlineData("management", false)]
        [InlineData("anonymous", false)]
        [Trait("Category", "BlackBox"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Technique", "DecisionTable"), Trait("Type", "Negative")]
        public async Task RoleVsAction_OnlyStaffInScopeMayChangeStatus(string actor, bool mayAssign)
        {
            // Category 1 request; staff 2 is in scope (category 1), staff 5 is not (category 2 only).
            var current = actor switch
            {
                "requester" => FakeCurrentUser.AsRequester(1),
                "staff-in-scope" => FakeCurrentUser.AsStaff(2),
                "staff-out-of-scope" => FakeCurrentUser.AsStaff(5),
                "management" => FakeCurrentUser.AsManager(),
                _ => FakeCurrentUser.Anonymous()
            };
            var h = new ServiceHarness(current);
            var request = h.AddRequest(RequestStatus.Submitted, categoryId: 1);

            var result = await h.Workflow.AssignAsync(new AssignRequestCommand { RequestId = request.Id, AssigneeStaffId = 2, RowVersion = new byte[] { 1 } });

            Assert.Equal(mayAssign, result.Succeeded);
            Assert.Equal(mayAssign ? RequestStatus.Assigned : RequestStatus.Submitted, request.Status);
        }
    }
}

namespace CivicConnect.Tests
{
    /// <summary>OWNER: Michael-John Robinson. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class TransitionMatrixBlackBoxTests
    {

        // Technique: Decision table - every (current status, action) pair against the allowed flow Submitted->Assigned->InProgress->Resolved->Closed (FR-010).
        [Theory(Skip = "TODO")]
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
            // TODO: assert RequestStateFactory.For(from).CanMoveTo(to) == allowed.
        }

        [Fact(Skip = "TODO")]
        [Trait("Category", "BlackBox"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Technique", "DecisionTable"), Trait("Type", "Negative")]
        public Task RoleVsAction_OnlyStaffInScopeMayChangeStatus()
        {
            // TODO: matrix of Requester/Staff-in-scope/Staff-out-of-scope/Management x AssignAsync; only Staff-in-scope succeeds.
            return Task.CompletedTask;
        }
    }
}

namespace CivicConnect.Tests
{
    /// <summary>OWNER: Enrique de Sousa. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class RequestStateTests
    {

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public void Submitted_Assign_SetsAssigneeAndMovesToAssigned()
        {
            // TODO: request = TestData.Request(Submitted); new SubmittedState().Assign(request, 2, now); assert Succeeded, Status == Assigned, AssignedStaffId == 2, AssignedAt == now.
        }

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public void Closed_AnyAction_FailsWithInvalidTransition()
        {
            // TODO: for Assign/Start/Resolve/Close on ClosedState assert Failed and ErrorCode == ErrorCodes.InvalidTransition; request unchanged.
        }

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public void Submitted_CannotSkipToResolved()
        {
            // TODO: assert new SubmittedState().CanMoveTo(RequestStatus.Resolved) is false and Resolve(...) fails.
        }
    }
}

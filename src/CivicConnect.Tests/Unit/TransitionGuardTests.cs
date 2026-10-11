namespace CivicConnect.Tests
{
    /// <summary>OWNER: Enrique de Sousa. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class TransitionGuardTests
    {

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "NFR-003"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public Task Management_CannotChangeStatus()
        {
            // TODO: build TransitionGuard with an AuthorisationService using FakeCurrentUser.AsManager(); CheckAsync must return Forbidden.
            return Task.CompletedTask;
        }

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "FR-010"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public Task Staff_InvalidTransition_IsRejected()
        {
            // TODO: staff in scope, request Submitted, target Resolved -> Failure(InvalidTransition).
            return Task.CompletedTask;
        }
    }
}

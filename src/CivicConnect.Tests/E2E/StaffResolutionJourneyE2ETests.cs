namespace CivicConnect.Tests
{
    /// <summary>OWNER: Michael-John Robinson. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class StaffResolutionJourneyE2ETests
    {

        [Fact(Skip = "TODO")]
        [Trait("Category", "E2E"), Trait("Req", "FR-005"), Trait("Priority", "High")]
        public Task Staff_Accepts_Starts_Resolves_Closes_AndRequesterSeesFeedback()
        {
            // TODO: requester submits; staff accepts, starts, resolves (with note), closes; requester Details page shows feedback for each step
            // and NOT the staff-only resolution note; closed request is absent from the staff active list but retrievable with IncludeClosed.
            return Task.CompletedTask;
        }
    }
}

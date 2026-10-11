namespace CivicConnect.Tests
{
    /// <summary>OWNER: Enrique de Sousa. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class RequestWorkflowServiceTests
    {

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "NFR-010"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public Task Assign_WritesHistoryAndFeedbackTogether()
        {
            // TODO: staff in scope assigns a Submitted request; assert StatusHistory + Feedback comment (VisibleToRequester) added and saved once.
            return Task.CompletedTask;
        }

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "FR-009"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public Task Assign_WhenConcurrencyConflict_ReturnsConflictAndWritesNothing()
        {
            // TODO: FakeUnitOfWork.ThrowConcurrencyConflict = true -> Failure(ConcurrencyConflict).
            return Task.CompletedTask;
        }
    }
}

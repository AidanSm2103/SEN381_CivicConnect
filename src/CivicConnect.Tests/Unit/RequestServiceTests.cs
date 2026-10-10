namespace CivicConnect.Tests
{
    /// <summary>OWNER: Enrique de Sousa. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class RequestServiceTests
    {

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "FR-001"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public Task Submit_Valid_CreatesRequestAndFirstHistoryRowInOneTransaction()
        {
            // TODO: construct RequestService with the Fakes; SubmitAsync valid command; assert 1 request with unique ReferenceNo,
            // 1 StatusHistory (null -> Submitted), FakeUnitOfWork.SaveCount == 1.
            return Task.CompletedTask;
        }

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "FR-002"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public Task Submit_InactiveCategory_IsRejectedAndNothingSaved()
        {
            // TODO: CategoryId = 99 (inactive in FakeCategoryRepository) -> ValidationFailed, Items empty, SaveCount == 0.
            return Task.CompletedTask;
        }
    }
}

namespace CivicConnect.Tests
{
    /// <summary>OWNER: Michael-John Robinson. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class SubmitRequestBlackBoxTests
    {

        // Technique: Equivalence Partitioning + Boundary Value Analysis on Description length and CategoryId (FR-001, FR-002).
        [Theory(Skip = "TODO")]
        [InlineData("", false)]            // empty partition
        [InlineData("   ", false)]         // whitespace partition
        [InlineData("Broken light", true)] // valid partition
        [Trait("Category", "BlackBox"), Trait("Req", "FR-001"), Trait("Priority", "High"), Trait("Technique", "EP")]
        public Task Description_Partitions(string description, bool expectedValid)
        {
            // TODO: call IRequestService.SubmitAsync through the fakes and assert Succeeded == expectedValid.
            return Task.CompletedTask;
        }

        [Theory(Skip = "TODO")]
        [InlineData(1999, true)]
        [InlineData(2000, true)]
        [InlineData(2001, false)]
        [Trait("Category", "BlackBox"), Trait("Req", "FR-001"), Trait("Priority", "High"), Trait("Technique", "BVA")]
        public Task Description_LengthBoundaries(int length, bool expectedValid)
        {
            // TODO: description = new string("a", length); assert outcome.
            return Task.CompletedTask;
        }

        [Theory(Skip = "TODO")]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(99, false)]   // exists but inactive
        [InlineData(1000, false)] // does not exist
        [Trait("Category", "BlackBox"), Trait("Req", "FR-002"), Trait("Priority", "High"), Trait("Technique", "EP")]
        public Task CategoryId_Partitions(int categoryId, bool expectedValid)
        {
            // TODO: assert outcome per partition.
            return Task.CompletedTask;
        }
    }
}

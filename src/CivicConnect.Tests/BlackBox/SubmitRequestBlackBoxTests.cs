namespace CivicConnect.Tests
{
    /// <summary>OWNER: Michael-John Robinson. Black-box tests: only inputs and outputs of IRequestService.SubmitAsync are checked, never its internals.</summary>
    public class SubmitRequestBlackBoxTests
    {
        private static async Task<bool> Submits(string description, int categoryId)
        {
            var h = new ServiceHarness(FakeCurrentUser.AsRequester(1));
            var result = await h.RequestService.SubmitAsync(new SubmitRequestCommand { CategoryId = categoryId, Description = description });
            return result.Succeeded;
        }

        // Technique: Equivalence Partitioning + Boundary Value Analysis on Description length and CategoryId (FR-001, FR-002).
        [Theory]
        [InlineData("", false)]            // empty partition
        [InlineData("   ", false)]         // whitespace partition
        [InlineData("Broken light", true)] // valid partition
        [Trait("Category", "BlackBox"), Trait("Req", "FR-001"), Trait("Priority", "High"), Trait("Technique", "EP")]
        public async Task Description_Partitions(string description, bool expectedValid)
        {
            Assert.Equal(expectedValid, await Submits(description, 1));
        }

        [Theory]
        [InlineData(1999, true)]
        [InlineData(2000, true)]
        [InlineData(2001, false)]
        [Trait("Category", "BlackBox"), Trait("Req", "FR-001"), Trait("Priority", "High"), Trait("Technique", "BVA")]
        public async Task Description_LengthBoundaries(int length, bool expectedValid)
        {
            Assert.Equal(expectedValid, await Submits(new string('a', length), 1));
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(99, false)]   // exists but inactive
        [InlineData(1000, false)] // does not exist
        [Trait("Category", "BlackBox"), Trait("Req", "FR-002"), Trait("Priority", "High"), Trait("Technique", "EP")]
        public async Task CategoryId_Partitions(int categoryId, bool expectedValid)
        {
            Assert.Equal(expectedValid, await Submits("Valid description", categoryId));
        }
    }
}

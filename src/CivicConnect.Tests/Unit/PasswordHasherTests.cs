namespace CivicConnect.Tests
{
    // NFR-004 (only a salted hash is ever stored)
    public class PasswordHasherTests
    {
        private readonly IdentityPasswordHasher _hasher = new();

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-004"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public void Hash_IsNotThePassword_AndVerifies()
        {
            var hash = _hasher.Hash("Correct-Horse-9");

            Assert.NotEqual("Correct-Horse-9", hash);
            Assert.True(_hasher.Verify(hash, "Correct-Horse-9"));
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-004"), Trait("Priority", "High"), Trait("Type", "Negative")]
        public void Verify_RejectsWrongPassword_EmptyInput_AndCorruptHash()
        {
            var hash = _hasher.Hash("Correct-Horse-9");

            Assert.False(_hasher.Verify(hash, "wrong-password"));
            Assert.False(_hasher.Verify(hash, ""));
            Assert.False(_hasher.Verify("", "Correct-Horse-9"));
            Assert.False(_hasher.Verify("not-a-valid-hash", "Correct-Horse-9"));
        }

        [Fact]
        [Trait("Category", "Unit"), Trait("Req", "NFR-004"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public void SamePassword_HashesDifferentlyEachTime_BecauseOfRandomSalt()
        {
            Assert.NotEqual(_hasher.Hash("Correct-Horse-9"), _hasher.Hash("Correct-Horse-9"));
        }
    }
}

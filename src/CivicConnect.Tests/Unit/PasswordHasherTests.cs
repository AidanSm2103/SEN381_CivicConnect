namespace CivicConnect.Tests
{
    /// <summary>OWNER: Aidan Smith. Remove the Skip argument when you implement a test. Keep the Trait lines - they feed the RTM/evidence table.</summary>
    public class PasswordHasherTests
    {

        [Fact(Skip = "TODO")]
        [Trait("Category", "Unit"), Trait("Req", "NFR-004"), Trait("Priority", "High"), Trait("Type", "Positive")]
        public void Hash_IsNotThePassword_AndVerifies()
        {
            // TODO: var h = new IdentityPasswordHasher(); hash != password; Verify(hash, password) true; Verify(hash, "wrong") false.
        }
    }
}

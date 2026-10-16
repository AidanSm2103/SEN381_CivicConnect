namespace CivicConnect.Services
{
    // The framework hasher uses PBKDF2 (HMAC-SHA512, 128-bit random per-password salt, Identity v3 format) (NFR-004).
    public class IdentityPasswordHasher : IPasswordHasher
    {
        private const int IterationCount = 220_000;

        private static readonly Microsoft.AspNetCore.Identity.PasswordHasher<User> Inner = new(
            Microsoft.Extensions.Options.Options.Create(new Microsoft.AspNetCore.Identity.PasswordHasherOptions { IterationCount = IterationCount }));

        public string Hash(string password)
        {
            ArgumentException.ThrowIfNullOrEmpty(password);
            return Inner.HashPassword(new User(), password);
        }

        public bool Verify(string passwordHash, string providedPassword)
        {
            if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(providedPassword)) return false;
            try
            {
                var result = Inner.VerifyHashedPassword(new User(), passwordHash, providedPassword);
                return result != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed;
            }
            catch (FormatException)
            {
                return false; // corrupted/legacy hash string: treat as a failed login, never crash
            }
        }
    }
}

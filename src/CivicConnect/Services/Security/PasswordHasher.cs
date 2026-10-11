namespace CivicConnect.Services
{
    /// <summary>
    /// OWNER: Aidan Smith. Wraps Microsoft.AspNetCore.Identity.PasswordHasher&lt;User&gt; (ships inside the ASP.NET Core
    /// shared framework - NO extra NuGet package needed). Class name differs from the framework's to avoid ambiguity.
    /// </summary>
    public class IdentityPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            // TODO: new Microsoft.AspNetCore.Identity.PasswordHasher<User>().HashPassword(new User(), password)
            throw new NotImplementedException();
        }

        public bool Verify(string passwordHash, string providedPassword)
        {
            // TODO: VerifyHashedPassword(new User(), passwordHash, providedPassword) and return true for Success or SuccessRehashNeeded.
            throw new NotImplementedException();
        }
    }
}

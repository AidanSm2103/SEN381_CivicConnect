namespace CivicConnect.Services
{
    /// <summary>Password hashing abstraction (D-006, NFR-004). Only the hash is ever stored.</summary>
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string passwordHash, string providedPassword);
    }
}

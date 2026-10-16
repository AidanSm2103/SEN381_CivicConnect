namespace CivicConnect.Services
{
    // Password hashing abstraction (D-006, NFR-004) only the hash is ever stored
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string passwordHash, string providedPassword);
    }
}


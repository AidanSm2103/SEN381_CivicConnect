namespace CivicConnect.Services
{
    // Login and registration rules (NFR-002)
    public interface IAuthService
    {
        // Success(user) for a correct password on an active account Failure(InvalidCredentials) otherwise 
        Task<Result<User>> ValidateCredentialsAsync(string email, string password);

        // Creates a new user with Role = Requester. Failure(DuplicateEmail) / Failure(ValidationFailed) as needed
        Task<Result<User>> RegisterRequesterAsync(RegisterCommand command);
    }
}


namespace CivicConnect.Services
{
    /// <summary>Login and registration rules (NFR-002). Cookie sign-in itself happens in AccountController.</summary>
    public interface IAuthService
    {
        /// <summary>Success(user) for a correct password on an ACTIVE account. Failure(InvalidCredentials) otherwise - same message for unknown email and wrong password.</summary>
        Task<Result<User>> ValidateCredentialsAsync(string email, string password);

        /// <summary>Creates a new user with Role = Requester. Failure(DuplicateEmail) / Failure(ValidationFailed) as needed. Self-registration NEVER creates Staff or Management.</summary>
        Task<Result<User>> RegisterRequesterAsync(RegisterCommand command);
    }
}

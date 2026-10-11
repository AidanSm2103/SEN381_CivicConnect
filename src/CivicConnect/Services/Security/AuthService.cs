namespace CivicConnect.Services
{
    /// <summary>OWNER: Aidan Smith.</summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository users, IPasswordHasher hasher, IUnitOfWork unitOfWork, IClock clock, ILogger<AuthService> logger)
        {
            _users = users;
            _hasher = hasher;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _logger = logger;
        }

        public Task<Result<User>> ValidateCredentialsAsync(string email, string password)
        {
            // TODO: load by email; unknown/inactive/wrong password -> Failure(InvalidCredentials, "Invalid email or password.")
            // and _logger.LogWarning(...) WITHOUT logging the password (NFR-002: failed access is logged).
            throw new NotImplementedException();
        }

        public Task<Result<User>> RegisterRequesterAsync(RegisterCommand command)
        {
            // TODO: validate (email format, DisplayName not blank, password min length 8) -> ValidationFailed;
            // EmailExistsAsync -> DuplicateEmail; hash password; add User{Role=Requester, CreatedAt=_clock.UtcNow}; SaveChangesAsync; return Success(user).
            throw new NotImplementedException();
        }
    }
}

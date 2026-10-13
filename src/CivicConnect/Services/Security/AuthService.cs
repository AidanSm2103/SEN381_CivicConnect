namespace CivicConnect.Services
{
    public class AuthService : IAuthService
    {
        private const string GenericLoginError = "Invalid email or password.";
        private const int MinPasswordLength = 8;

        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly ILogger<AuthService> _logger;

        // Hash verified when the email is unknown so "unknown email" and "wrong password" take similar time
        private string? _dummyHash;

        public AuthService(IUserRepository users, IPasswordHasher hasher, IUnitOfWork unitOfWork, IClock clock, ILogger<AuthService> logger)
        {
            _users = users;
            _hasher = hasher;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _logger = logger;
        }

        public async Task<Result<User>> ValidateCredentialsAsync(string email, string password)
        {
            var normalised = Normalise(email);
            if (normalised.Length == 0 || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Login rejected: empty email or password supplied.");
                return Result<User>.Failure(ErrorCodes.InvalidCredentials, GenericLoginError);
            }

            var user = await _users.GetByEmailAsync(normalised);
            if (user == null)
            {
                _dummyHash ??= _hasher.Hash("civicconnect-dummy-password");
                _hasher.Verify(_dummyHash, password);
                _logger.LogWarning("Login rejected: unknown account {Email}.", SafeLog(normalised));
                return Result<User>.Failure(ErrorCodes.InvalidCredentials, GenericLoginError);
            }

            if (!user.IsActive || !_hasher.Verify(user.PasswordHash, password))
            {
                _logger.LogWarning("Login rejected for {Email} (inactive account or wrong password).", SafeLog(normalised));
                return Result<User>.Failure(ErrorCodes.InvalidCredentials, GenericLoginError);
            }

            return Result<User>.Success(user);
        }

        public async Task<Result<User>> RegisterRequesterAsync(RegisterCommand command)
        {
            var email = Normalise(command.Email);
            var displayName = (command.DisplayName ?? string.Empty).Trim();
            var password = command.Password ?? string.Empty;

            if (email.Length == 0 || email.Length > 256 || !new EmailAddressAttribute().IsValid(email))
                return Result<User>.Failure(ErrorCodes.ValidationFailed, "Please enter a valid email address.");
            if (displayName.Length == 0 || displayName.Length > 100)
                return Result<User>.Failure(ErrorCodes.ValidationFailed, "Please enter a display name of up to 100 characters.");
            if (password.Length < MinPasswordLength || password.Length > 100)
                return Result<User>.Failure(ErrorCodes.ValidationFailed, $"Password must be {MinPasswordLength}-100 characters long.");

            if (await _users.EmailExistsAsync(email))
                return Result<User>.Failure(ErrorCodes.DuplicateEmail, "An account with this email already exists.");

            var user = new User
            {
                Email = email,
                DisplayName = displayName,
                PasswordHash = _hasher.Hash(password),
                Role = UserRole.Requester,      // self registration can never create Staff or Management
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };

            try
            {
                await _users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (await _users.EmailExistsAsync(email))
                    return Result<User>.Failure(ErrorCodes.DuplicateEmail, "An account with this email already exists.");
                throw;
            }

            return Result<User>.Success(user);
        }

        private static string Normalise(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

        // Strips line breaks so a crafted email cannot forge extra log lines
        private static string SafeLog(string value) => value.Replace("\r", " ").Replace("\n", " ");
    }
}


namespace CivicConnect.Composition
{
    /// <summary>OWNER: Aidan Smith. Cookie authentication + policies + security services. Already complete.</summary>
    public static class SecurityServiceExtensions
    {
        public static IServiceCollection AddCivicConnectSecurity(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.Cookie.HttpOnly = true;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    // TODO(Aidan): options.Cookie.SecurePolicy = CookieSecurePolicy.Always; (HTTPS only) once HTTPS profile confirmed.
                });

            services.AddAuthorization(AuthorizationPolicies.Register);

            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
            services.AddScoped<IAuthorisationService, AuthorisationService>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}

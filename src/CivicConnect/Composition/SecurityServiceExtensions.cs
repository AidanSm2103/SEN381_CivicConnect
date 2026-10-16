namespace CivicConnect.Composition
{
    // Cookie authentication + policies + security services
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
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Secure whenever the request is HTTPS

                    // NFR-002: log every rejected request (who, where) and then keep the framework's normal redirect.
                    var defaults = new CookieAuthenticationEvents();
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            LogRejected(context.HttpContext, "Unauthenticated request rejected");
                            return defaults.RedirectToLogin(context);
                        },
                        OnRedirectToAccessDenied = context =>
                        {
                            LogRejected(context.HttpContext, "Forbidden request rejected");
                            return defaults.RedirectToAccessDenied(context);
                        }
                    };
                });

            services.AddAuthorization(AuthorizationPolicies.Register);

            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
            services.AddScoped<IAuthorisationService, AuthorisationService>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }

        // Writes who was refused and for which path. Line breaks are stripped so a crafted URL cannot forge log lines.
        private static void LogRejected(HttpContext http, string message)
        {
            var logger = http.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("CivicConnect.Security");
            var userId = http.User.FindFirst(AppClaimTypes.UserId)?.Value ?? "anonymous";
            var path = (http.Request.Path.Value ?? string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
            logger.LogWarning("{Message}: user {UserId}, path {Path}", message, userId, path);
        }
    }
}


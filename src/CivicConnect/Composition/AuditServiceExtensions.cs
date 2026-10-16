namespace CivicConnect.Composition
{
    /// <summary>OWNER: Aidan Smith. Audit + notification services. Already complete.</summary>
    public static class AuditServiceExtensions
    {
        public static IServiceCollection AddCivicConnectAudit(this IServiceCollection services)
        {
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<INotificationService, InProcessNotificationService>();
            return services;
        }
    }
}

namespace CivicConnect.Composition
{
    /// <summary>OWNER: Michael-John Robinson. Reporting module. Already complete.</summary>
    public static class ReportingServiceExtensions
    {
        public static IServiceCollection AddCivicConnectReporting(this IServiceCollection services)
        {
            services.AddScoped<IReportingService, ReportingService>();
            return services;
        }
    }
}

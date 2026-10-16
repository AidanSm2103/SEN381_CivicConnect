namespace CivicConnect.Composition
{
    /// <summary>OWNER: Michael-John Robinson. Registers EF Core + repositories + unit of work + clock. Already complete; add a line ONLY when you add a new repository.</summary>
    public static class DataServiceExtensions
    {
        public static IServiceCollection AddCivicConnectData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IClock, SystemUtcClock>();
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IReportingRepository, ReportingRepository>();
            return services;
        }
    }
}

namespace CivicConnect.Composition
{
    /// <summary>OWNER: Enrique de Sousa. Requests + Workflow modules. Already complete; add a line ONLY when you add a new service.</summary>
    public static class RequestsServiceExtensions
    {
        public static IServiceCollection AddCivicConnectRequests(this IServiceCollection services)
        {
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IRequestWorkflowService, RequestWorkflowService>();
            services.AddScoped<TransitionGuard>();
            return services;
        }
    }
}

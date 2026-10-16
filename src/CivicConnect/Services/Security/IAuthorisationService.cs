namespace CivicConnect.Services
{
    // Every service method that reads or changes a request calls this before doing anything else
    public interface IAuthorisationService
    {
        // Failure(Unauthenticated) if nobody is logged in
        Result EnsureAuthenticated();

        // Failure(Unauthenticated) or Failure(Forbidden) unless the current user has one of the roles
        Result EnsureInRole(params UserRole[] roles);

        // Requester: only own requests. Staff: only categories in their scope. Management: any. Else Failure(Forbidden)
        Task<Result> EnsureCanViewAsync(ServiceRequest request);

        // Staff only (Management is read-only), and the request's category must be in the staff member's scope. Else Failure(Forbidden)
        Task<Result> EnsureCanWorkAsync(ServiceRequest request);

        // null = unrestricted (Management). Staff = their scoped CategoryIds. Requester = empty list.
        Task<IReadOnlyCollection<int>?> GetVisibleCategoryIdsAsync();
    }
}

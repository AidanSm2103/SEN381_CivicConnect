namespace CivicConnect.Services
{
    /// <summary>
    /// THE single enforcement point for role + category access (NFR-003, architecture decision 7).
    /// Every service method that reads or changes a request calls this BEFORE doing anything else.
    /// Controllers' [Authorize(Policy=...)] is the outer fence; this is the real check.
    /// </summary>
    public interface IAuthorisationService
    {
        /// <summary>Failure(Unauthenticated) if nobody is logged in.</summary>
        Result EnsureAuthenticated();

        /// <summary>Failure(Unauthenticated) or Failure(Forbidden) unless the current user has one of the roles.</summary>
        Result EnsureInRole(params UserRole[] roles);

        /// <summary>Requester: only own requests. Staff: only categories in their scope. Management: any. Else Failure(Forbidden).</summary>
        Task<Result> EnsureCanViewAsync(ServiceRequest request);

        /// <summary>Staff only (Management is read-only), and the request's category must be in the staff member's scope. Else Failure(Forbidden).</summary>
        Task<Result> EnsureCanWorkAsync(ServiceRequest request);

        /// <summary>null = unrestricted (Management). Staff = their scoped CategoryIds. Requester = empty list.</summary>
        Task<IReadOnlyCollection<int>?> GetVisibleCategoryIdsAsync();
    }
}

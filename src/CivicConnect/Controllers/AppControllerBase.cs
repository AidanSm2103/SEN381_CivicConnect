namespace CivicConnect.Controllers
{
    // Base class for every controller so error handling and post-login navigation are uniform
    public abstract class AppControllerBase : Controller
    {
        // Maps a failed Result to an HTTP outcome
        protected IActionResult MapFailure(Result result)
        {
            return result.ErrorCode switch
            {
                ErrorCodes.NotFound => NotFound(),
                ErrorCodes.Forbidden => Forbid(),
                ErrorCodes.Unauthenticated => Challenge(),
                ErrorCodes.ConcurrencyConflict or ErrorCodes.AlreadyAssigned or ErrorCodes.InvalidTransition => Conflict(result.ErrorMessage),
                _ => BadRequest(result.ErrorMessage)
            };
        }

        // True for failures that should become a plain HTTP outcome rather than a message on the page
        protected static bool IsAccessFailure(Result result)
        {
            return result.ErrorCode == ErrorCodes.NotFound
                || result.ErrorCode == ErrorCodes.Forbidden
                || result.ErrorCode == ErrorCodes.Unauthenticated;
        }

        // Sends a user to the landing page of their role 
        protected IActionResult RedirectToRoleHome(UserRole role)
        {
            return role switch
            {
                UserRole.Staff => RedirectToAction("Index", "StaffRequests"),
                UserRole.Management => RedirectToAction("Dashboard", "Management"),
                _ => RedirectToAction("MyRequests", "ServiceRequests")
            };
        }

        // Same as RedirectToRoleHome but reads the role from the signed-in principal
        protected IActionResult? RedirectToRoleHomeIfSignedIn()
        {
            if (User.Identity?.IsAuthenticated != true) return null;
            if (User.IsInRole(RoleNames.Management)) return RedirectToRoleHome(UserRole.Management);
            if (User.IsInRole(RoleNames.Staff)) return RedirectToRoleHome(UserRole.Staff);
            if (User.IsInRole(RoleNames.Requester)) return RedirectToRoleHome(UserRole.Requester);
            return null;
        }
    }
}

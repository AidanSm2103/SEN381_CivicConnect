namespace CivicConnect.Controllers
{
    /// <summary>
    /// OWNER: Aidan Smith. COMPLETE - do not change. Base class for every NEW controller so error handling is uniform.
    /// Controllers stay thin: bind -> call ONE service method -> map result to a View/Redirect. No business rules, no DbContext.
    /// </summary>
    public abstract class AppControllerBase : Controller
    {
        /// <summary>Maps a failed Result to an HTTP outcome. Use for NotFound/Forbidden/Unauthenticated; for form errors use ModelState or TempData.</summary>
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

        /// <summary>True for failures that should become a plain HTTP outcome (404/403/401) rather than a message on the page.</summary>
        protected static bool IsAccessFailure(Result result)
        {
            return result.ErrorCode == ErrorCodes.NotFound
                || result.ErrorCode == ErrorCodes.Forbidden
                || result.ErrorCode == ErrorCodes.Unauthenticated;
        }
    }
}

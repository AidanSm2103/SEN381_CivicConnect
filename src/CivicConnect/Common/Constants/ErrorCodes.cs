namespace CivicConnect.Common
{
    /// <summary>Machine-readable failure codes carried by Result. Controllers map these to HTTP outcomes.</summary>
    public static class ErrorCodes
    {
        public const string NotFound = "not_found";                    // -> 404
        public const string Forbidden = "forbidden";                   // -> 403 / AccessDenied
        public const string Unauthenticated = "unauthenticated";       // -> 401 / redirect to login
        public const string ValidationFailed = "validation_failed";    // -> re-display form with errors
        public const string InvalidTransition = "invalid_transition";  // -> 409 / message to staff
        public const string AlreadyAssigned = "already_assigned";      // -> 409 (FR-009 double assignment)
        public const string ConcurrencyConflict = "concurrency_conflict"; // -> 409 (RowVersion mismatch)
        public const string InvalidCredentials = "invalid_credentials";
        public const string DuplicateEmail = "duplicate_email";
    }
}

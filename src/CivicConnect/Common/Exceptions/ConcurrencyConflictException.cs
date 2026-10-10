namespace CivicConnect.Common
{
    /// <summary>
    /// Thrown by IUnitOfWork when EF reports a RowVersion mismatch (DbUpdateConcurrencyException).
    /// Services catch THIS type (never the EF type) and convert it to Result.Failure(ErrorCodes.ConcurrencyConflict, ...).
    /// This keeps EF types out of the services layer. COMPLETE - do not change.
    /// </summary>
    public class ConcurrencyConflictException : Exception
    {
        public ConcurrencyConflictException(string message, Exception? inner = null) : base(message, inner) { }
    }
}

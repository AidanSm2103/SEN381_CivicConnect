namespace CivicConnect.Data
{
    /// <summary>
    /// Transaction boundary owned by the SERVICE layer (D-004, D-006). Repositories never call SaveChanges themselves.
    /// A status change = one ExecuteInTransactionAsync call that updates the request, writes StatusHistory and writes the Feedback comment.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>Persist pending changes. Must translate DbUpdateConcurrencyException into ConcurrencyConflictException.</summary>
        Task<int> SaveChangesAsync();

        /// <summary>Run <paramref name="work"/> inside one DB transaction; commit on success, roll back on any exception.</summary>
        Task ExecuteInTransactionAsync(Func<Task> work);
    }
}

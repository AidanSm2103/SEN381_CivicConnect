namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson. One transaction per business operation (D-004).</summary>
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public EfUnitOfWork(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Services only know ConcurrencyConflictException, never the EF type (layer rule).
                throw new ConcurrencyConflictException("The record was changed by someone else.", ex);
            }
        }

        // The EF Core in-memory provider (used only by the automated integration tests) has no transactions and throws if asked for one.
        private bool SupportsTransactions =>
            _db.Database.ProviderName?.Contains("InMemory", StringComparison.OrdinalIgnoreCase) != true;

        public async Task ExecuteInTransactionAsync(Func<Task> work)
        {
            if (!SupportsTransactions)
            {
                await work();
                await SaveChangesAsync();   // a single SaveChanges is still atomic for the in-memory store
                return;
            }

            // Already inside a transaction: join it rather than nesting.
            if (_db.Database.CurrentTransaction != null)
            {
                await work();
                await SaveChangesAsync();
                return;
            }

            // The SQL Server retry strategy is not enabled, so a user-initiated transaction is allowed.
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                await work();
                await SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}

namespace CivicConnect.Data
{
    /// <summary>OWNER: Michael-John Robinson.</summary>
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public EfUnitOfWork(AppDbContext db)
        {
            _db = db;
        }

        public Task<int> SaveChangesAsync()
        {
            // TODO(Michael-John): call _db.SaveChangesAsync(); catch DbUpdateConcurrencyException and rethrow as
            // ConcurrencyConflictException (keep the original as inner exception). Return the affected row count.
            throw new NotImplementedException();
        }

        public Task ExecuteInTransactionAsync(Func<Task> work)
        {
            // TODO(Michael-John): begin a transaction with _db.Database.BeginTransactionAsync(), await work(),
            // SaveChangesAsync() (via this.SaveChangesAsync so concurrency is translated), commit.
            // On any exception: roll back and rethrow. If a transaction is already active, just run work() (no nesting).
            throw new NotImplementedException();
        }
    }
}

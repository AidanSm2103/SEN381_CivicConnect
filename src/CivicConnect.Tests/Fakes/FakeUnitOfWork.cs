namespace CivicConnect.Tests.Fakes
{
    /// <summary>COMPLETE. Counts saves and can simulate a concurrency conflict. OWNER: Aidan Smith.</summary>
    public class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }
        public bool ThrowConcurrencyConflict { get; set; }

        public Task<int> SaveChangesAsync()
        {
            if (ThrowConcurrencyConflict) throw new ConcurrencyConflictException("simulated");
            SaveCount++;
            return Task.FromResult(1);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> work)
        {
            await work();
            await SaveChangesAsync();
        }
    }
}

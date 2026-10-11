namespace CivicConnect.Common
{
    /// <summary>Abstraction over the system clock so tests can control time (D-006).</summary>
    public interface IClock
    {
        DateTime UtcNow { get; }
    }

    /// <summary>Production clock. COMPLETE - do not change.</summary>
    public class SystemUtcClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

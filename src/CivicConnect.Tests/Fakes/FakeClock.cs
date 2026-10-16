namespace CivicConnect.Tests.Fakes
{
    // A clock tests can set
    public class FakeClock : IClock
    {
        public DateTime UtcNow { get; set; } = new DateTime(2026, 10, 19, 8, 0, 0, DateTimeKind.Utc);
    }
}

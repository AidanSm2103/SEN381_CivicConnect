namespace CivicConnect.Tests.Fakes
{
    //Lets a test "log in" as anyone
    public class FakeCurrentUser : ICurrentUser
    {
        public bool IsAuthenticated { get; set; } = true;
        public int? UserId { get; set; } = 1;
        public UserRole? Role { get; set; } = UserRole.Requester;
        public string DisplayName { get; set; } = "Test User";

        public static FakeCurrentUser AsRequester(int id = 1) => new() { UserId = id, Role = UserRole.Requester };
        public static FakeCurrentUser AsStaff(int id = 2) => new() { UserId = id, Role = UserRole.Staff };
        public static FakeCurrentUser AsManager(int id = 3) => new() { UserId = id, Role = UserRole.Management };
        public static FakeCurrentUser Anonymous() => new() { IsAuthenticated = false, UserId = null, Role = null };
    }
}

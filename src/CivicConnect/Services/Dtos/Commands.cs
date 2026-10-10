namespace CivicConnect.Services
{
    // Input objects for the service layer. Plain data holders - COMPLETE, do not change.
    // They deliberately do NOT carry the acting user: services read it from ICurrentUser (prevents spoofing).

    public class SubmitRequestCommand
    {
        public int CategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class AssignRequestCommand
    {
        public int RequestId { get; set; }
        public int AssigneeStaffId { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }

    public class AcceptRequestCommand
    {
        public int RequestId { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }

    public class ChangeStatusCommand
    {
        public int RequestId { get; set; }
        public RequestStatus TargetStatus { get; set; }
        public string? Note { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }

    public class ResolveRequestCommand
    {
        public int RequestId { get; set; }
        public string ResolutionNote { get; set; } = string.Empty;
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }

    public class CloseRequestCommand
    {
        public int RequestId { get; set; }
        public string? Note { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }

    public class AddNoteCommand
    {
        public int RequestId { get; set; }
        public string Body { get; set; } = string.Empty;
    }

    public class RegisterCommand
    {
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

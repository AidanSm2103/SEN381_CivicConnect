namespace CivicConnect.Services
{
    /// <summary>State: Closed (terminal). No further moves.</summary>
    public class ClosedState : RequestState
    {
        public override RequestStatus Status => RequestStatus.Closed;

        public override IReadOnlyCollection<RequestStatus> AllowedTargets => Array.Empty<RequestStatus>();

        // Terminal state: no overrides. Every action inherits the base "invalid transition" failure. The request is never deleted (FR-012).
    }
}

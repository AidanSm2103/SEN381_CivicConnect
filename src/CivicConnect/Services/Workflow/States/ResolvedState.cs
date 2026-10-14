namespace CivicConnect.Services
{
    /// <summary>State: Resolved. Only Close is allowed.</summary>
    public class ResolvedState : RequestState
    {
        public override RequestStatus Status => RequestStatus.Resolved;

        public override IReadOnlyCollection<RequestStatus> AllowedTargets => new[] { RequestStatus.Closed };

        public override Result Close(ServiceRequest request, DateTime now)
        {
            request.Status = RequestStatus.Closed;
            request.ClosedAt = now;
            return Result.Success();
        }
    }
}

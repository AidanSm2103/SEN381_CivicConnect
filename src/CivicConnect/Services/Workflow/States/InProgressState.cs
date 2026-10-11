namespace CivicConnect.Services
{
    /// <summary>State: InProgress. Only Resolve is allowed.</summary>
    public class InProgressState : RequestState
    {
        public override RequestStatus Status => RequestStatus.InProgress;

        public override IReadOnlyCollection<RequestStatus> AllowedTargets => new[] { RequestStatus.Resolved };

        public override Result Resolve(ServiceRequest request, DateTime now)
        {
            // TODO: set Status = RequestStatus.Resolved, ResolvedAt = now; return Result.Success().
            throw new NotImplementedException();
        }
    }
}

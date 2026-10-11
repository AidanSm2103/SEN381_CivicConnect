namespace CivicConnect.Services
{
    /// <summary>State: Assigned. Only Start is allowed.</summary>
    public class AssignedState : RequestState
    {
        public override RequestStatus Status => RequestStatus.Assigned;

        public override IReadOnlyCollection<RequestStatus> AllowedTargets => new[] { RequestStatus.InProgress };

        public override Result Start(ServiceRequest request)
        {
            // TODO: set Status = RequestStatus.InProgress; return Result.Success().
            throw new NotImplementedException();
        }
    }
}

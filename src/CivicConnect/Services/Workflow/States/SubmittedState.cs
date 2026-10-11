namespace CivicConnect.Services
{
    /// <summary>State: Submitted. Only Assign is allowed.</summary>
    public class SubmittedState : RequestState
    {
        public override RequestStatus Status => RequestStatus.Submitted;

        public override IReadOnlyCollection<RequestStatus> AllowedTargets => new[] { RequestStatus.Assigned };

        public override Result Assign(ServiceRequest request, int staffId, DateTime now)
        {
            // TODO: if request.AssignedStaffId != null -> Failure(AlreadyAssigned). Otherwise set AssignedStaffId = staffId,
            // AssignedAt = now, Status = RequestStatus.Assigned; return Result.Success().
            throw new NotImplementedException();
        }
    }
}

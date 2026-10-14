namespace CivicConnect.Services
{
    /// <summary>State: Submitted. Only Assign is allowed.</summary>
    public class SubmittedState : RequestState
    {
        public override RequestStatus Status => RequestStatus.Submitted;

        public override IReadOnlyCollection<RequestStatus> AllowedTargets => new[] { RequestStatus.Assigned };

        public override Result Assign(ServiceRequest request, int staffId, DateTime now)
        {
            if (request.AssignedStaffId != null)
                return Result.Failure(ErrorCodes.AlreadyAssigned, "This request has already been assigned.");

            request.AssignedStaffId = staffId;
            request.AssignedAt = now;
            request.Status = RequestStatus.Assigned;
            return Result.Success();
        }
    }
}

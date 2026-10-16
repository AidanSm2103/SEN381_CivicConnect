namespace CivicConnect.Services
{
    /// <summary>
    /// State pattern base (D-005). One subclass per RequestStatus. Each state knows which moves are legal and applies them to the entity.
    /// The base class's action methods default to "invalid transition"; a subclass overrides ONLY the actions it allows.
    /// Allowed flow (M2): Submitted -> Assigned -> InProgress -> Resolved -> Closed. No skipping, no going back.
    /// OWNER: Enrique de Sousa (all files in Services/Workflow/States).
    /// </summary>
    public abstract class RequestState
    {
        public abstract RequestStatus Status { get; }

        /// <summary>The statuses reachable from this state.</summary>
        public abstract IReadOnlyCollection<RequestStatus> AllowedTargets { get; }

        public virtual bool CanMoveTo(RequestStatus target)
        {
            return AllowedTargets.Contains(target);
        }

        /// <summary>Submitted -> Assigned. Sets AssignedStaffId, AssignedAt, Status.</summary>
        public virtual Result Assign(ServiceRequest request, int staffId, DateTime now)
        {
            return Invalid("assigned");
        }

        /// <summary>Assigned -> InProgress. Sets Status.</summary>
        public virtual Result Start(ServiceRequest request)
        {
            return Invalid("started");
        }

        /// <summary>InProgress -> Resolved. Sets Status and ResolvedAt.</summary>
        public virtual Result Resolve(ServiceRequest request, DateTime now)
        {
            return Invalid("resolved");
        }

        /// <summary>Resolved -> Closed. Sets Status and ClosedAt.</summary>
        public virtual Result Close(ServiceRequest request, DateTime now)
        {
            return Invalid("closed");
        }

        private Result Invalid(string action)
        {
            return Result.Failure(ErrorCodes.InvalidTransition, $"A {Status} request cannot be {action}.");
        }
    }
}

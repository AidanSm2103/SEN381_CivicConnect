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
            // TODO: return AllowedTargets.Contains(target).
            throw new NotImplementedException();
        }

        /// <summary>Submitted -> Assigned. Sets AssignedStaffId, AssignedAt, Status.</summary>
        public virtual Result Assign(ServiceRequest request, int staffId, DateTime now)
        {
            // TODO: default = Failure(ErrorCodes.InvalidTransition, $"A {Status} request cannot be assigned.")
            throw new NotImplementedException();
        }

        /// <summary>Assigned -> InProgress. Sets Status.</summary>
        public virtual Result Start(ServiceRequest request)
        {
            // TODO: default = Failure(ErrorCodes.InvalidTransition, ...)
            throw new NotImplementedException();
        }

        /// <summary>InProgress -> Resolved. Sets Status and ResolvedAt.</summary>
        public virtual Result Resolve(ServiceRequest request, DateTime now)
        {
            // TODO: default = Failure(ErrorCodes.InvalidTransition, ...)
            throw new NotImplementedException();
        }

        /// <summary>Resolved -> Closed. Sets Status and ClosedAt.</summary>
        public virtual Result Close(ServiceRequest request, DateTime now)
        {
            // TODO: default = Failure(ErrorCodes.InvalidTransition, ...)
            throw new NotImplementedException();
        }
    }
}

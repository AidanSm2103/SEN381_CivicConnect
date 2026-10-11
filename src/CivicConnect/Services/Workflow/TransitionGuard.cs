namespace CivicConnect.Services
{
    /// <summary>OWNER: Enrique de Sousa. Decides whether a status transition is permitted BEFORE any change is made (D-005).</summary>
    public class TransitionGuard
    {
        private readonly IAuthorisationService _authorisation;

        public TransitionGuard(IAuthorisationService authorisation)
        {
            _authorisation = authorisation;
        }

        public Task<Result> CheckAsync(ServiceRequest request, RequestStatus target)
        {
            // TODO: 1) _authorisation.EnsureCanWorkAsync(request) -> return failure if not ok (Forbidden / Unauthenticated).
            //       2) RequestStateFactory.For(request.Status).CanMoveTo(target) false -> Failure(InvalidTransition,
            //          $"Cannot move a request from {request.Status} to {target}.").
            //       3) otherwise Success.
            throw new NotImplementedException();
        }
    }
}

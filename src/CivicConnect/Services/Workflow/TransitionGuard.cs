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

        public async Task<Result> CheckAsync(ServiceRequest request, RequestStatus target)
        {
            // 1) Who: staff in scope only (Management is read-only, requesters cannot work requests).
            var work = await _authorisation.EnsureCanWorkAsync(request);
            if (work.Failed) return work;

            // 2) What: the state object says whether this move is legal.
            if (!RequestStateFactory.For(request.Status).CanMoveTo(target))
            {
                return Result.Failure(ErrorCodes.InvalidTransition,
                    $"Cannot move a request from {request.Status} to {target}.");
            }

            return Result.Success();
        }
    }
}

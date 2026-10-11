namespace CivicConnect.Services
{
    /// <summary>OWNER: Enrique de Sousa. Maps a RequestStatus to its state object.</summary>
    public static class RequestStateFactory
    {
        public static RequestState For(RequestStatus status)
        {
            // TODO: switch expression returning a (cached/static) instance of SubmittedState, AssignedState, InProgressState,
            // ResolvedState or ClosedState. Unknown value -> throw ArgumentOutOfRangeException.
            throw new NotImplementedException();
        }
    }
}

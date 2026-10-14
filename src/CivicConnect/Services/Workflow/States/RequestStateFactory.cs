namespace CivicConnect.Services
{
    /// <summary>OWNER: Enrique de Sousa. Maps a RequestStatus to its state object.</summary>
    public static class RequestStateFactory
    {
        // States hold no data, so one shared instance per status is safe.
        private static readonly SubmittedState Submitted = new();
        private static readonly AssignedState Assigned = new();
        private static readonly InProgressState InProgress = new();
        private static readonly ResolvedState Resolved = new();
        private static readonly ClosedState Closed = new();

        public static RequestState For(RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Submitted => Submitted,
                RequestStatus.Assigned => Assigned,
                RequestStatus.InProgress => InProgress,
                RequestStatus.Resolved => Resolved,
                RequestStatus.Closed => Closed,
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown request status.")
            };
        }
    }
}

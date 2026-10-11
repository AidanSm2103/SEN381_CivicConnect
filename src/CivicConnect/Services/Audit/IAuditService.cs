namespace CivicConnect.Services
{
    /// <summary>Writes and reads the append-only audit trail (NFR-010). Writes are staged and saved by the caller's transaction.</summary>
    public interface IAuditService
    {
        /// <summary>
        /// Stage a StatusHistory row for a status change. Actor = ICurrentUser, time = IClock.
        /// Does NOT save - the calling workflow/service saves inside its own transaction so the change and its audit row commit together.
        /// </summary>
        Task RecordStatusChangeAsync(ServiceRequest request, RequestStatus? fromStatus, RequestStatus toStatus, string? note);

        /// <summary>Full history for a request, oldest first. Caller must already have passed IAuthorisationService.EnsureCanViewAsync.</summary>
        Task<IReadOnlyList<StatusHistory>> GetHistoryAsync(int requestId);
    }
}

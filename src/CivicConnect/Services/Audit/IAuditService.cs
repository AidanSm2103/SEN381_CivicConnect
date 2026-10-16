namespace CivicConnect.Services
{
    // Writes and reads the append-only audit trail (NFR-010)
    public interface IAuditService
    {
        /// Stage a StatusHistory row for a status change
        Task RecordStatusChangeAsync(ServiceRequest request, RequestStatus? fromStatus, RequestStatus toStatus, string? note);

        // Full history for a request, oldest first
        Task<IReadOnlyList<StatusHistory>> GetHistoryAsync(int requestId);
    }
}

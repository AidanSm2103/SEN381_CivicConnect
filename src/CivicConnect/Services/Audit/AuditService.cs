namespace CivicConnect.Services
{
    // Append-only: this class only ever adds StatusHistory rows (NFR-010)
    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _auditLog;
        private readonly ICurrentUser _currentUser;
        private readonly IClock _clock;

        public AuditService(IAuditLogRepository auditLog, ICurrentUser currentUser, IClock clock)
        {
            _auditLog = auditLog;
            _currentUser = currentUser;
            _clock = clock;
        }

        public Task RecordStatusChangeAsync(ServiceRequest request, RequestStatus? fromStatus, RequestStatus toStatus, string? note)
        {
            if (!_currentUser.UserId.HasValue)
                throw new InvalidOperationException("A status change can only be audited for an authenticated user.");

            var entry = new StatusHistory
            {
                FromStatus = fromStatus,
                ToStatus = toStatus,
                ChangedByUserId = _currentUser.UserId.Value,
                ChangedAt = _clock.UtcNow,
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim()
            };

            // A brand-new request has no Id yet
            if (request.Id == 0) entry.ServiceRequest = request;
            else entry.ServiceRequestId = request.Id;

            return _auditLog.AddStatusHistoryAsync(entry);   // staged only, the caller's transaction saves it with the change
        }

        public Task<IReadOnlyList<StatusHistory>> GetHistoryAsync(int requestId)
        {
            return _auditLog.GetHistoryAsync(requestId);
        }
    }
}


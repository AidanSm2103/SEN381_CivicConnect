namespace CivicConnect.Services
{
    /// <summary>OWNER: Aidan Smith.</summary>
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
            // TODO: build StatusHistory { ServiceRequestId or ServiceRequest = request (needed for brand-new requests that have no Id yet),
            // FromStatus, ToStatus, ChangedByUserId = _currentUser.UserId!.Value, ChangedAt = _clock.UtcNow, Note }
            // and call _auditLog.AddStatusHistoryAsync. DO NOT save.
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<StatusHistory>> GetHistoryAsync(int requestId)
        {
            // TODO: return _auditLog.GetHistoryAsync(requestId).
            throw new NotImplementedException();
        }
    }
}

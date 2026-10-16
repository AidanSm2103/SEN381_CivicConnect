namespace CivicConnect.Models
{
    /// <summary>
    /// The five lifecycle statuses (M2 data baseline 8.1 / 8.2).
    /// Stored in the DB as the enum NAME string (HasConversion&lt;string&gt;() in ServiceRequestConfiguration),
    /// so existing rows with "Submitted" stay valid.
    /// </summary>
    public enum RequestStatus
    {
        Submitted = 0,
        Assigned = 1,
        InProgress = 2,
        Resolved = 3,
        Closed = 4
    }
}

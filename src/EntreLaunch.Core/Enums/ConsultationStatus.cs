namespace EntreLaunch.Enums;

/// <summary>
/// Consultation Status (enum).
/// </summary>
public enum ConsultationStatus
{
    [Description("Scheduled")]
    Scheduled = 0,

    [Description("in-progress")]
    InProgress = 1,

    [Description("completed")]
    Complete = 2,

    [Description("cancelled")]
    Cancelled = 3,
}

/// <summary>
/// Consultation Ticket Status (enum).
/// </summary>
public enum ConsultationTicketStatus
{
    Open = 0,
    Closed = 1,
}

/// <summary>
/// Counselor Request Status (enum).
/// </summary>
public enum CounselorRequesttStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2,
}

/// <summary>
/// Counselor Request Status (enum).
/// </summary>
public enum ConsultationType
{
    Online = 0,
    text = 1
}

namespace EntreLaunch.Enums;

public enum CourseStatus
{
    [Description("Unknown")]
    Unknown = 0,

    [Description("Draft")]
    Draft = 1,

    [Description("Scheduled")]
    Scheduled = 2,

    [Description("Published")]
    Published = 3,

    [Description("Archived")]
    Archived = 4,

    [Description("EntreLaunchcoming")]
    EntreLaunchcoming = 5,

    [Description("Ongoing")]
    Ongoing = 6,

    [Description("Completed")]
    Completed = 7,

    [Description("Cancelled")]
    Cancelled = 8,
}

namespace EntreLaunch.Enums;
public enum SimulationStatus
{
    [Description("Pending")]
    Pending = 0,

    [Description("Accepted")]
    Accepted = 1,

    [Description("Rejected")]
    Rejected = 2,
}

public enum Category
{
    [Description("Positive")]
    Positive = 1,

    [Description("Negative")]
    Negative = -1
}

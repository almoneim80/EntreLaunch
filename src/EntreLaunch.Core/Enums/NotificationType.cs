namespace EntreLaunch.Enums;

public enum NotificationType
{
    [Description("General")]
    General = 0,

    [Description("Payment")]
    Payment = 1,

    [Description("Message")]
    Message = 2,

    [Description("Alert")]
    Alert = 3,

    [Description("System")]
    System = 4,

    [Description("Error")]
    Error = 5,

    [Description("Warning")]
    Warning = 6,
}

using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class NotificationCreateDto
{
    public string SenderId { get; set; } = null!;
    public string ReciverId { get; set; } = null!;
    public string? Message { get; set; }
    public NotificationType? Type { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTimeOffset? ReadAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class NotificationEntreLaunchdateDto
{
    public string SenderId { get; set; } = null!;
    public string ReciverId { get; set; } = null!;
    public string? Message { get; set; }
    public NotificationType? Type { get; set; }
    public bool? IsRead { get; set; } = false;
    [JsonIgnore]
    public DateTimeOffset? ReadAt { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdaedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class NotificationDetailsDto : NotificationCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdaedAt { get; set; }
}

public class NotificationExportDto
{
    public string? SenderId { get; set; }
    public string? ReciverId { get; set; }
    public string? Message { get; set; }
    public NotificationType? Type { get; set; }
    public bool? IsRead { get; set; } = false;
    public DateTimeOffset? ReadAt { get; set; }
}

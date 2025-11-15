using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class WheelPlayerCreateDto
{
    public string PlayerId { get; set; } = null!;
    public DateTimeOffset? PlayedAt { get; set; }
    public int AwardId { get; set; }
    public bool IsFree { get; set; } = true;

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class WheelPlayerEntreLaunchdateDto
{
    public string? PlayerId { get; set; }
    public DateTimeOffset? PlayedAt { get; set; }
    public int? AwardId { get; set; }
    public bool IsFree { get; set; } = true;
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class WheelPlayerDetailsDto : WheelPlayerCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class WheelPlayerExportDto
{
    public string PlayerId { get; set; } = null!;
    public DateTimeOffset? PlayedAt { get; set; }
    public int AwardId { get; set; }
    public bool IsFree { get; set; } = true;
}

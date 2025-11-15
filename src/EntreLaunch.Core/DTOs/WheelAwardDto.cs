using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class WheelAwardCreateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Probability { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class WheelAwardEntreLaunchdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Probability { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class WheelAwardDetailsDto : WheelAwardCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class WheelAwardExportDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Probability { get; set; }
}

public class WheelAwardImportDto : BaseEntityWithId
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Probability { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

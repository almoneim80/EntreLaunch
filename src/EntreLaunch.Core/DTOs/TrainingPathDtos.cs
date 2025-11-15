using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class TrainingPathCreateDto
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class TrainingPathEntreLaunchdateDto
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class TrainingPathDetailsDto : TrainingPathCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class TrainingPathExportDto
{
    public string? Name { get; set; }

    public int? CoursesNumber { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }
}

public class TrainingPathImportDto : BaseEntityWithId
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

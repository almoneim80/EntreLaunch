namespace EntreLaunch.DTOs;

public class CourseFieldCreateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class CourseFieldEntreLaunchdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    [JsonIgnore]
    public DateTimeOffset EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class CourseFieldDetailsDto : CourseFieldCreateDto
{
    public int? Id { get; set; }
    public DateTimeOffset EntreLaunchdatedAt { get; set; }
}

public class CourseFieldExportDto 
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

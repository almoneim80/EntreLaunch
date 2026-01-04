namespace EntreLaunch.DTOs;

public class CourseFieldCreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class CourseFieldUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    [JsonIgnore]
    public DateTimeOffset UpdatedAt { get; set; } = DateHelper.UtcNow;
}

public class CourseFieldDetailsDto
{
#nullable disable
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

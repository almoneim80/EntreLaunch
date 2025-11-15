using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class CourseInstructorCreateDto
{
    public int CourseId { get; set; }
    public string UserId { get; set; } = null!;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class CourseInstructorEntreLaunchdateDto
{
    public int CourseId { get; set; }
    public string UserId { get; set; } = null!;
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class CourseInstructorDetailsDto : CourseInstructorCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class CourseInstructorExportDto
{
    public int? CourseId { get; set; } 
    public string? UserId { get; set; }
}

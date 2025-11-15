using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class CourseEnrollmentCreateDto
{
    public int CourseId { get; set; }

    [JsonIgnore]
    public string UserId { get; set; } = null!;

    [JsonIgnore]
    public DateTimeOffset? EnrolledAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class CourseEnrollmentEntreLaunchdateDto
{
    public int CourseId { get; set; }

    public string UserId { get; set; } = null!;
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class CourseEnrollmentDetailsDto : CourseEnrollmentCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class CourseEnrollmentExportDto
{
    public int? CourseId { get; set; }
    public string? UserId { get; set; }
    public DateTimeOffset? EnrolledAt { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

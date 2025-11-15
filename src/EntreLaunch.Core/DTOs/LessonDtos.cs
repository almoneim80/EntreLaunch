using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

/// <summary>
/// Request for creating a lesson.
/// </summary>
public class LessonCreateDto
{
    public string? Name { get; set; }
    public string? VideoUrl { get; set; }
    public int? Order { get; set; }
    public int? DurationInMinutes { get; set; }
    public string? Description { get; set; }
    public int CourseId { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Request for EntreLaunchdating a lesson.
/// </summary>
public class LessonEntreLaunchdateDto
{
    public string? Name { get; set; }
    public string? VideoUrl { get; set; }
    public int? DurationInMinutes { get; set; }
    public string? Description { get; set; }
    public int CourseId { get; set; }
    public int? Order { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Request for EntreLaunchdating a lesson.
/// </summary>
public class LessonDetailsDto : LessonCreateDto
{
    public int Id { get; set; }
    public int? OrderIndex { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

/// <summary>
/// Request for exporting lessons.
/// </summary>
public class LessonExportDto
{
    public string? Name { get; set; }
    public string? VideoUrl { get; set; }
    public int? DurationInMinutes { get; set; }
    public string? Description { get; set; }
    public int? Order { get; set; }
}

/// <summary>
/// Request for creating a lesson with children.
/// </summary>
public class LessonWithChildrenDto : LessonCreateDto
{
    public List<LessonAttachmentFullAddDto>? LessonAttachmentFullAddDtos { get; set; }
}

/// <summary>
/// Request for importing lessons.
/// </summary>
public class LessonImportDto : BaseEntityWithId
{
    public string? Name { get; set; }
    public string? VideoUrl { get; set; }
    public int? DurationInMinutes { get; set; }
    public string? Description { get; set; }
    public int CourseId { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Request for EntreLaunchdating the progress of a lesson.
/// </summary>
public class EntreLaunchdateProgressRequest
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    public string? UserId { get; set; }

    [Required]
    public int LessonId { get; set; }

    [Required]
    public TimeSpan TimeSpent { get; set; }
}

/// <summary>
/// Request for recalculating the progress of a course for a specific user.
/// </summary>
public class RecalculateProgressRequest
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    public string? UserId { get; set; }
}


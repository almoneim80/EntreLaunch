using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class LessonAttachmentCreateDto
{
    public int LessonId { get; set; }
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class LessonAttachmentEntreLaunchdateDto
{
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
    public int? LessonId { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class LessonAttachmentDetailsDto : LessonAttachmentCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class LessonAttachmentExportDto
{
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
    public int? LessonId { get; set; }
}

public class LessonAttachmentFullAddDto
{
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
}

public class LessonAttachmentImportDto : BaseEntityWithId
{
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
    public int LessonId { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Get all LessonAttachments.
/// </summary>
public class AttachmentStatsDto
{
    public int AttachmentId { get; set; }
    public string FileName { get; set; } = null!;
    public int OpenCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}


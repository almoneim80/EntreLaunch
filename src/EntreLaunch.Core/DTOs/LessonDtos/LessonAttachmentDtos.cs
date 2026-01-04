namespace EntreLaunch.DTOs.LessonDtos;

public class LessonAttachmentCreateDto
{
    public int LessonId { get; set; }
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class LessonAttachmentUpdateDto
{
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
    public int? LessonId { get; set; }

    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class LessonAttachmentDetailsDto : LessonAttachmentCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class LessonAttachmentExportDto
{
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
    public int? LessonId { get; set; }
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


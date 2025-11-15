namespace EntreLaunch.DTOs;

public class ExamCreateDto
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }
    public int? DurationInMinutes { get; set; }
    public ExamParentEntityType? ParentEntityType { get; set; }

    public int? CourseId { get; set; }
    public int? LessonId { get; set; }
    public int? PathId { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class ExamEntreLaunchdateDto
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }
    public int? DurationInMinutes { get; set; }
    public ExamParentEntityType? ParentEntityType { get; set; }
    public int CourseId { get; set; }
    public int LessonId { get; set; }
    public int PathId { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class ExamDetailsDto : ExamCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class ExamExportDto
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }
    public int? DurationInMinutes { get; set; }

    public int? CourseId { get; set; }
    public int? LessonId { get; set; }
    public int? PathId { get; set; }
}

public class ExamWithChildrenDto : ExamCreateDto
{
    public List<QuestionCreateDtoWithChildren>? Questions { get; set; }
}

public class ExamImportDto : BaseEntityWithId
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }
    public int? DurationInMinutes { get; set; }
    public ExamParentEntityType? ParentEntityType { get; set; }

    public int? CourseId { get; set; }
    public int? LessonId { get; set; }
    public int? PathId { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

// ALL EXAM DATA WITH RELATIONSHIPS
public class ExamFullDetailsDto
{
    public int ExamId { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }
    public int? DurationInMinutes { get; set; }
    public int? MaxAttempts { get; set; }
    public ExamStatus Status { get; set; }

    public string? ParentEntityName { get; set; }

    public List<QuestionDetailsData>? Questions { get; set; }
}

public class QuestionDetailsData
{
    public int QuestionId { get; set; }
    public string? Text { get; set; }
    public List<AnswerDetailsData>? Answers { get; set; }
}

public class AnswerDetailsData
{
    public int AnswerId { get; set; }
    public string? Text { get; set; }
    public bool IsCorrect { get; set; }
}

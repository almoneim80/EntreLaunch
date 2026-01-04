namespace EntreLaunch.DTOs.ExamDtos;

#region CREATE Exam With Full Data
public class FullLessonExamDto
{
#nullable disable
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }
    public int? DurationInMinutes { get; set; }
    public int LessonId { get; set; }

    [JsonIgnore]
    public ExamParentEntityType ParentEntityType { get; set; } = ExamParentEntityType.Lesson;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;

    public List<QuestionCreateDtoWithChildren> Questions { get; set; }
}

public class UpdateLessonExamDto
{
#nullable enable
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }
    public int? DurationInMinutes { get; set; }
    public int? LessonId { get; set; }

    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}

public class FullCourseExamDto
{
#nullable disable
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }
    public int? DurationInMinutes { get; set; }
    public int CourseId { get; set; }

    [JsonIgnore]
    public ExamParentEntityType ParentEntityType { get; set; } = ExamParentEntityType.Course;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;

    public List<QuestionCreateDtoWithChildren> Questions { get; set; }
}

public class UpdateCourseExamDto
{
#nullable enable
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }
    public int? DurationInMinutes { get; set; }
    public int? CourseId { get; set; }

    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}

public class FullPathExamDto
{
#nullable disable
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }

    [JsonIgnore]
    public ExamParentEntityType ParentEntityType { get; set; } = ExamParentEntityType.Path;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;

    public List<QuestionCreateDtoWithChildren> Questions { get; set; }
}

public class UpdatePathExamDto
{
#nullable enable
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? MinMark { get; set; }
    public decimal? MaxMark { get; set; }
    public int? DurationInMinutes { get; set; }

    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}
#endregion

#region IMPORT Exam Data
public class ImportFullLessonExamDto : BaseEntityWithId
{
#nullable disable
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal MinMark { get; set; }
    public decimal MaxMark { get; set; }
    public int DurationInMinutes { get; set; }
    public int LessonId { get; set; }

    [JsonIgnore]
    public ExamParentEntityType ParentEntityType { get; set; } = ExamParentEntityType.Lesson;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<QuestionCreateDtoWithChildren> Questions { get; set; }
}

public class ImportFullCourseExamDto : BaseEntityWithId
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal MinMark { get; set; }
    public decimal MaxMark { get; set; }
    public int DurationInMinutes { get; set; }
    public int CourseId { get; set; }

    [JsonIgnore]
    public ExamParentEntityType ParentEntityType { get; set; } = ExamParentEntityType.Course;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<QuestionCreateDtoWithChildren> Questions { get; set; }
}
#endregion

#region ALL EXAM DATA WITH RELATIONSHIPS
public class ExamFullDetailsDto
{
#nullable disable
    public int ExamId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public decimal MinMark { get; set; }
    public decimal MaxMark { get; set; }
    public int DurationInMinutes { get; set; }
    public int MaxAttempts { get; set; }
    public ExamStatus Status { get; set; }

    public string ParentEntityName { get; set; }

    public List<QuestionDetailsData> Questions { get; set; }
}

public class QuestionDetailsData
{
    public int QuestionId { get; set; }
    public string Text { get; set; }
    public decimal Mark { get; set; }
    public List<AnswerDetailsData> Answers { get; set; }
}

public class AnswerDetailsData
{
    public int AnswerId { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}
#endregion

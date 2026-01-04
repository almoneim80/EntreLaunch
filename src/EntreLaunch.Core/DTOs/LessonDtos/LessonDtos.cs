using EntreLaunch.DTOs.ExamDtos;

namespace EntreLaunch.DTOs.LessonDtos;

/// <summary>
/// Request for creating a lesson.
/// </summary>
public class LessonCreateDto
{
#nullable disable
    public string Name { get; set; }
    public string VideoUrl { get; set; }
    public int Order { get; set; }
    public int? DurationInMinutes { get; set; }
    public string Description { get; set; }
    public int CourseId { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;

    public List<AttachmentOfLessonCreateDto> Attachments { get; set; }
}

/// <summary>
/// Request for updating a lesson.
/// </summary>
public class LessonUpdateDto
{
#nullable enable
    public string? Name { get; set; }
    public string? VideoUrl { get; set; }
    public int? DurationInMinutes { get; set; }
    public string? Description { get; set; }
    public int CourseId { get; set; }
    public int? Order { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}

#nullable enable
// IMPORT MULIPLE LESSONS WITH ITS RELATIONSHIPS
public class LessonWithRelatedContent : BaseEntityWithId, IHasNestedImports, IMapsTo<Lesson>
{
    public string? Name { get; set; }
    public string? VideoUrl { get; set; }
    public int? Order { get; set; }
    public int? DurationInMinutes { get; set; }
    public string? Description { get; set; }
    public int CourseId { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;

    public List<AttachmentOfLesson>? Attachments { get; set; }
    public List<LessonExcerise>? Exercises { get; set; }

    // interface implementation.
    public IEnumerable<object?> GetNestedImports()
    {
        foreach (var a in Attachments ?? Enumerable.Empty<AttachmentOfLesson>()) yield return a;
        foreach (var ex in Exercises ?? Enumerable.Empty<LessonExcerise>()) yield return ex;
    }
}

public class AttachmentOfLesson : IMapsTo<LessonAttachment>
{
    public string? FileUrl { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class LessonExcerise : IHasNestedImports, IMapsTo<Exam>
{
#nullable disable
    public string Name { get; set; }
    public string Type { get; set; } // final , midterm, homework
    public string Description { get; set; }
    public decimal MinMark { get; set; }
    public decimal MaxMark { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;

    public List<ExcersiseQuestions> Questions { get; set; }

    public IEnumerable<object> GetNestedImports()
    {
        foreach (var q in Questions ?? Enumerable.Empty<ExcersiseQuestions>()) yield return q;
    }
}

public class ExcersiseQuestions : IHasNestedImports, IMapsTo<Question>
{
    public string Text { get; set; }
    public decimal Mark { get; set; }
    [JsonIgnore]
    public DateTimeOffset UpdatedAt { get; set; } = DateHelper.UtcNow;

    public List<QuestionChoise> Choices { get; set; }

    public IEnumerable<object> GetNestedImports()
    {
        foreach (var c in Choices ?? Enumerable.Empty<QuestionChoise>()) yield return c;
    }
}

public class QuestionChoise : IMapsTo<Answer>
{
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}

/// <summary>
/// lesson full details dto.
/// </summary>
public class LessonFullDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string Name { get; set; }
    public string VideoUrl { get; set; }
    public int DurationInMinutes { get; set; }
    public string Description { get; set; }
    public int OldOrder { get; set; }
    public int NewOrder { get; set; }

    public List<LessonAttachmentDto> Attachments { get; set; }

    public LessonCourseDto lessonCourse { get; set; }
    public ExamFullDetailsDto LessonExam { get; set; }
}

/// <summary>
/// lesson attachment dto.
/// </summary>
public class LessonAttachmentDto
{
    public int Id { get; set; }
    public string FileUrl { get; set; }
}

public class LessonCourseDto
{
#nullable enable
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public CourseFieldDto? Field { get; set; }
    public CoursePathDto? Path { get; set; }
    public decimal Price { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public string? StudyWay { get; set; }
    public int DurationInDays { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool? CertificateExists { get; set; } = true;
    public bool IsFree { get; set; } = false;
    public string? Logo { get; set; }
    public CourseStatus? Status { get; set; }
    public CourseType? Type { get; set; }
    public int? MaxEnrollment { get; set; } = 0;
    public List<string>? Audience { get; set; }
    public List<string>? Requirements { get; set; }
    public List<string>? Topics { get; set; }
    public List<string>? Goals { get; set; }
    public List<string>? Outcomes { get; set; }
}

public class CourseFieldDto
{
#nullable disable
    public string Name { get; set; }
    public string Description { get; set; }
}

public class CoursePathDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; } = 0;
    public string Description { get; set; }
    public bool CertificateExists { get; set; }
    public int MaxEnrollment { get; set; } = 0;
    public bool IsFree { get; set; } = false;
}

public class AttachmentOfLessonCreateDto
{
    public string FileName { get; set; }
    public string FileUrl { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

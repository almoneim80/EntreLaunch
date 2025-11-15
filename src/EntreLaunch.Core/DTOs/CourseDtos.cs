using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class CourseCreateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? FieldId { get; set; }
    public int? PathId { get; set; }
    public decimal? Price { get; set; } = 0;
    public decimal? Discount { get; set; } = 0;
    public string? StudyWay { get; set; }
    public int? DurationInDays { get; set; }
    
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool? CertificateExists { get; set; } = true;
    public bool IsFree { get; set; } = false;
    public string? CertificateUrl { get; set; }
    public string? Logo { get; set; }
    public CourseStatus? Status { get; set; }
    public int? MaxEnrollment { get; set; } = 0;
    public List<string>? Audience { get; set; } 
    public List<string>? Requirements { get; set; } 
    public List<string>? Topics { get; set; } 
    public List<string>? Goals { get; set; } 
    public List<string>? Outcomes { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class CourseEntreLaunchdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? FieldId { get; set; }
    public decimal? Price { get; set; } = 0;
    public decimal? Discount { get; set; } = 0;
    public string? StudyWay { get; set; }
    public int? DurationInDays { get; set; }
    public int? PathId { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public CourseStatus? Status { get; set; }
    public bool CertificateExists { get; set; } = true;
    public bool IsFree { get; set; } = false;
    public string? CertificateUrl { get; set; }
    public string? Logo { get; set; }
    public int? MaxEnrollment { get; set; } = 0;
    public List<string>? Audience { get; set; }
    public List<string>? Requirements { get; set; }
    public List<string>? Topics { get; set; } 
    public List<string>? Goals { get; set; } 
    public List<string>? Outcomes { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class CourseDetailsDto : CourseCreateDto
{
    public int Id { get; set; }
    public decimal? FinalPrice => Price - (Price * (Discount / 100));
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class CourseExportDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? FieldId { get; set; }
    public decimal? Price { get; set; } = 0;
    public decimal? Discount { get; set; } = 0;
    public string? StudyWay { get; set; }
    public int? DurationInDays { get; set; }
    public int? LessonNumber { get; set; } = 0;
    public int? PathId { get; set; }
    public bool IsFree { get; set; }
    public int? MaxEnrollment { get; set; } = 0;
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public CourseStatus? Status { get; set; }
    public bool CertificateExists { get; set; } 
    public List<string>? Audience { get; set; }
    public List<string>? Requirements { get; set; } 
    public List<string>? Topics { get; set; } 
    public List<string>? Goals { get; set; } 
    public List<string>? Outcomes { get; set; } 
}

public class CourseWithChildrenDto : CourseCreateDto
{
    public List<LessonWithChildrenDto>? Lessons { get; set; }
}

public class CourseImportDto : BaseEntityWithId
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? FieldId { get; set; }
    public decimal? Price { get; set; } = 0;
    public decimal? Discount { get; set; } = 0;
    public int? MaxEnrollment { get; set; } = 0;
    public string? StudyWay { get; set; }
    public int? DurationInDays { get; set; }
    public int PathId { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool CertificateExists { get; set; } = true;
    public bool IsFree { get; set; } = false;
    public string? CertificateUrl { get; set; }
    public string? Logo { get; set; }
    public CourseStatus? Status { get; set; }

    public List<string>? Audience { get; set; } 
    public List<string>? Requirements { get; set; }
    public List<string>? Topics { get; set; } 
    public List<string>? Goals { get; set; }
    public List<string>? Outcomes { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

// ALL COURSE DATA WITH ATTACHED DATA
public class CourseFullDetailsDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public decimal? Discount { get; set; }
    public decimal? FinalPrice => Price - (Price * (Discount / 100));
    public string? StudyWay { get; set; }
    public int? DurationInDays { get; set; }
    public bool IsFree { get; set; }
    public CourseStatus Status { get; set; }
    public string? Logo { get; set; }
    public bool CertificateExists { get; set; }
    public string? CertificateUrl { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public int? MaxEnrollment { get; set; }
    public int? CurrentEnrollmentCount { get; set; }
    public List<string>? Audience { get; set; }
    public List<string>? Requirements { get; set; }
    public List<string>? Topics { get; set; }
    public List<string>? Goals { get; set; }
    public List<string>? Outcomes { get; set; }
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }

    // بدل Id اجلب التفاصيل:
    public CourseFieldData? Field { get; set; }
    public TrainingPathData? Path { get; set; }

    // القوائم المرتبطة:
    public List<CourseInstructorData>? Instructors { get; set; }
    public List<CourseEnrollmentData>? Enrollments { get; set; }
    public List<LessonData>? Lessons { get; set; }
    public List<ExamData>? Exams { get; set; }
    public List<CourseRatingData>? Ratings { get; set; }
    public List<CourseTagData>? Tags { get; set; }
}

public class CourseFieldData
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class TrainingPathData
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class CourseInstructorData
{
    public string? InstructorId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Specialization { get; set; }
    public string? AvatarUrl { get; set; }
}

public class CourseEnrollmentData
{
    public string? StudentId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public DateTimeOffset EnrolledAt { get; set; }
}

public class LessonData
{
    public int LessonId { get; set; }
    public string? Title { get; set; }
    public int OrderIndex { get; set; }
    public List<LessonAttachmentData>? Attachments { get; set; }
}

public class LessonAttachmentData
{
    public int AttachmentId { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public int OpenCount { get; set; }
}

public class ExamData
{
    public int ExamId { get; set; }
    public string? ExamName { get; set; }
    public int? DurationInMinutes { get; set; }
    public List<QuestionData>? Questions { get; set; }
}

public class QuestionData
{
    public int QuestionId { get; set; }
    public string? Text { get; set; }
    public List<AnswerData>? Answers { get; set; }
}

public class AnswerData
{
    public int AnswerId { get; set; }
    public string? Text { get; set; }
    public bool IsCorrect { get; set; }
}

public class CourseRatingData
{
    public int RatingId { get; set; }
    public decimal Rating { get; set; }
    public string? ReviewerName { get; set; }
    public string? ReviewComment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CourseTagData
{
    public int TagId { get; set; }
    public string? Name { get; set; }
}

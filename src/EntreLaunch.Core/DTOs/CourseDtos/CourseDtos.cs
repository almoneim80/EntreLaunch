namespace EntreLaunch.DTOs.TrainingDtos;

/***************************************************** PATH COURSES DTOS ****************************************************/
/***************************************************** PATH COURSES DTOS ****************************************************/
public class PathCourseCreateDto
{
#nullable disable
    public string Name { get; set; }
    public string Description { get; set; }
    public int PathId { get; set; }
    public string Logo { get; set; }
    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; }
    public List<string> Audience { get; set; }
    public List<string> Requirements { get; set; }
    public List<string> Topics { get; set; }
    public List<string> Goals { get; set; }
    public List<string> Outcomes { get; set; }
    public List<LessonsCreateDto> Lessons { get; set; }

    [JsonIgnore]
    public CourseType Type { get; set; } = CourseType.PathCourse;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class PathCourseUpdateDto
{
#nullable enable
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? PathId { get; set; }
    public string? Logo { get; set; }
    public bool? CertificateExists { get; set; }
    public int? CertificateValidityInDays { get; set; }
    public List<string>? Audience { get; set; }
    public List<string>? Requirements { get; set; }
    public List<string>? Topics { get; set; }
    public List<string>? Goals { get; set; }
    public List<string>? Outcomes { get; set; }

    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class PathCourseDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int PathId { get; set; }
    public string Logo { get; set; }
    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; }
    public List<string> Audience { get; set; }
    public List<string> Requirements { get; set; }
    public List<string> Topics { get; set; }
    public List<string> Goals { get; set; }
    public List<string> Outcomes { get; set; }

    public List<LessonData> Lessons { get; set; }
    public List<ExamData> Exams { get; set; }
    public List<CourseTagData> Tags { get; set; }
}

/***************************************************** ONLINE COURSES DTOS ****************************************************/
/***************************************************** ONLINE COURSES DTOS ****************************************************/

public class OnlineCourseCreateDto
{
#nullable disable
    public string Name { get; set; }
    public string Description { get; set; }
    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public string StudyWay { get; set; }
    public CourseStatus Status { get; set; }
    public bool IsFree { get; set; }

    [JsonIgnore]
    public CourseType Type { get; set; } = CourseType.OnlineCourse;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class OnlineCourseUpdateDto
{
#nullable enable
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? CertificateExists { get; set; }
    public int? CertificateValidityInDays { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public decimal? Price { get; set; }
    public decimal? Discount { get; set; }
    public string? StudyWay { get; set; }
    public CourseStatus? Status { get; set; }
    public bool? IsFree { get; set; }
    [JsonIgnore]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class OnlineCourseDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public string StudyWay { get; set; }
    public CourseStatus Status { get; set; }
    public bool IsFree { get; set; }

    public List<CourseInstructorData> Instructors { get; set; }
    public List<CourseEnrollmentData> Enrollments { get; set; }
}

/***************************************************** SKILL COURSES DTOS ****************************************************/
/***************************************************** SKILL COURSES DTOS ****************************************************/

public class SkillCourseCreateDto
{
#nullable disable
    public string Name { get; set; }
    public string Description { get; set; }
    public int FieldId { get; set; }
    public string Logo { get; set; }
    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; }
    public bool IsFree { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public List<LessonsCreateDto> Lessons { get; set; }

    [JsonIgnore]
    public CourseType Type { get; set; } = CourseType.SkillsLibCourse;
    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class SkillCourseUpdateDto
{
#nullable enable
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? FieldId { get; set; }
    public string? Logo { get; set; }
    public bool? CertificateExists { get; set; }
    public int? CertificateValidityInDays { get; set; }
    public bool? IsFree { get; set; }
    public decimal? Price { get; set; }
    public decimal? Discount { get; set; }
    [JsonIgnore]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class SkillCourseDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string FieldName { get; set; }
    public string Logo { get; set; }
    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; }
    public bool IsFree { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }

    public List<LessonData> Lessons { get; set; }
    public List<ExamData> Exams { get; set; }
    public List<CourseTagData> Tags { get; set; }
    public List<CourseRatingData> Ratings { get; set; }
    public List<CourseInstructorData> Instructors { get; set; }
    public List<CourseEnrollmentData> Enrollments { get; set; }
}

/******************************************************* HELPERS ******************************************************************/
/******************************************************* HELPERS ******************************************************************/

public class LessonData
{
    public int LessonId { get; set; }
    public string Title { get; set; }
    public int OrderIndex { get; set; }
    public List<LessonAttachmentData> Attachments { get; set; }
}

public class LessonAttachmentData
{
    public int AttachmentId { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public int OpenCount { get; set; }
}

public class ExamData
{
    public int ExamId { get; set; }
    public string ExamName { get; set; }
    public int DurationInMinutes { get; set; }
    public List<QuestionData> Questions { get; set; }
}

public class QuestionData
{
    public int QuestionId { get; set; }
    public string Text { get; set; }
    public List<AnswerData> Answers { get; set; }
}

public class AnswerData
{
    public int AnswerId { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}

public class CourseRatingData
{
    public int RatingId { get; set; }
    public decimal Rating { get; set; }
    public string ReviewerName { get; set; }
    public string ReviewComment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CourseTagData
{
    public int TagId { get; set; }
    public string Name { get; set; }
}

public class CourseInstructorData
{
    public int InstructorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
    public string Description { get; set; }
}

public class CourseEnrollmentData
{
    public int EnrollmentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTimeOffset EnrolledAt { get; set; }
}

// Create lesson
public class LessonsCreateDto
{
#nullable disable
    public string Name { get; set; }
    public string VideoUrl { get; set; }
    public int Order { get; set; }
    public int DurationInMinutes { get; set; }
    public string Description { get; set; }

    public List<LessonsAttachmentsCreateDto> Attachments { get; set; }
}

public class LessonsAttachmentsCreateDto
{
#nullable disable
    public string FileName { get; set; }
    public string FileUrl { get; set; }
}

public class CoursesRegisterDto
{
#nullable disable
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTimeOffset EnrolledAt { get; set; }
}

using PhoneNumbers;

namespace EntreLaunch.DTOs;

/// <summary>
/// used to return users data that enrolled in Course.
/// </summary>
public class EnrollmentWithUserData
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public double? NationalId { get; set; }
    public string? Email { get; set; }
    public DateTimeOffset? EnrolledAt { get; set; }
}

/// <summary>
/// General result class for API responses for Training Section.
/// </summary>
public class GeneralResult
{
    public bool? IsSuccess { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public Enums.ErrorType? ErrorType { get; set; }

    public GeneralResult(bool success, string message, object? data = null)
    {
        IsSuccess = success;
        Message = message;
        Data = data;
    }

    public GeneralResult()
    {
        // empty constructor
    }
}

public class GeneralResult<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public Enums.ErrorType? ErrorType { get; set; }

    public GeneralResult(bool isSuccess, string? message = null, T? data = default)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }
}

/// <summary>
/// deto class for reordering lesson.
/// </summary>
public class LessonReorderDto
{
    public int LessonId { get; set; }
    public int OrderIndex { get; set; }
}

/// <summary>
/// used to return user subscription data.
/// </summary>
public class UserSubscriptionDto
{
    public int SubscriptionId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public string IsActive { get; set; } = null!;
    public DateTimeOffset? EnrolledAt { get; set; }
    public DateTimeOffset? LastEntreLaunchdatedAt { get; set; }
}

/// <summary>
/// used to assign tags to a course.
/// </summary>
public class AssignTagsDto
{
    public int CourseId { get; set; }
    public List<int> TagIds { get; set; } = new List<int>();
}

/// <summary>
/// used to return instructor data.
/// </summary>
public class InstructorDetails
{
    public string? InstructorId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Specialization { get; set; }
    public string? ProfilePicture { get; set; }
}

/// <summary>
/// used to return trainer performance data.
/// </summary>
public class TrainerPerformanceDto
{
    public int TotalCourses { get; set; }
    public List<CourseStatisticsDto> Courses { get; set; } = new();
}

/// <summary>
/// used to return course statistics data.
/// </summary>
public class CourseStatisticsDto
{
    public int CourseId { get; set; }
    public string? CourseName { get; set; }
    public int StudentCount { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
}

/// <summary>
/// used to return course rating summary data.
/// </summary>
public class CourseRatingSummaryDto
{
    public int CourseId { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
}

/// <summary>
/// used to return course rating summary data.
/// </summary>
public class CourseRatingsDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string? CourseName { get; set; }
    public string? ReviewerName { get; set; }
    public string? UserName { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// used to return course rating summary data.
/// </summary>
public class CourseRatingsRequest
{
    [Required]
    public int CourseId { get; set; }

    [FromQuery]
    [RegularExpression("^(rating|date)$", ErrorMessage = "SortBy must be 'rating' or 'date'.")]
    public string? SortBy { get; set; }

    [FromQuery]
    public bool IsDescending { get; set; } = false;

    [FromQuery]
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
    public int Page { get; set; } = 1;

    [FromQuery]
    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// used to return student progress data.
/// </summary>
public class StudentProgressDto
{
    public string? UserId { get; set; }
    public int CourseId { get; set; }
    public int LastLessonId { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
    public double CompletionPercentage { get; set; }
}

/// <summary>
/// used to return file category settings data.
/// </summary>
public class FileCategorySettings
{
    public List<string> Extensions { get; set; } = new();
    public Dictionary<string, string> MaxSizePerExtension { get; set; } = new();
}

/// <summary>
/// used to return exam result with student data.
/// </summary>
public class ExamResultWithStudentDto
{
    public int ExamId { get; set; }
    public string StudentId { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public decimal? Mark { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// used to return user answer data.
/// </summary>
public class UserAnswerDto
{
    public int QuestionId { get; set; }
    public int AnswerId { get; set; }
}

/// <summary>
/// used to return exam result data.
/// </summary>
public class ExamResultDto
{
    public string ExamName { get; set; } = string.Empty;
    public int OriginalDuration { get; set; }
    public int TimeTakenInSeconds { get; set; }
    public decimal CompletionPercentage { get; set; }
    public decimal MaxMark { get; set; }
    public decimal ObtainedMark { get; set; }
    public int AttemptNumber { get; set; }
}

/// <summary>
/// used to return exam submission data.
/// </summary>
public class ExamSubmissionDto
{
    public string UserId { get; set; } = string.Empty;
    public List<UserAnswerDto> Answers { get; set; } = new();
    public int TimeTakenInSeconds { get; set; }
}

/// <summary>
/// used to return student comparison data.
/// </summary>
public class StudentComparisonDto
{
    public int ExamId { get; set; }
    public string? UserId { get; set; }
    public decimal StudentMark { get; set; }
    public decimal BatchAverageMark { get; set; }
    public string ComparisonStatus { get; set; } = string.Empty; // "Above Average" or "Below Average"
}

/// <summary>
/// used to return exam statistics data.
/// </summary>
public class ExamStatisticsDto
{
    public int ExamId { get; set; }
    public decimal AverageMark { get; set; }
    public decimal MinimumMark { get; set; }
    public decimal MaximumMark { get; set; }
    public int TotalParticipants { get; set; }
}

/// <summary>
/// used to return top student data.
/// </summary>
public class TopStudentDto
{
    public string UserId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public decimal Mark { get; set; }
    public int Rank { get; set; }
}

/// <summary>
/// used to return student attempt data.
/// </summary>
public class StudentAttemptDto
{
    public int AttemptNumber { get; set; }
    public decimal Mark { get; set; }
    public string? Status { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset AttemptDate { get; set; }
}

/// <summary>
/// used to return student attempts with best attempt data.
/// </summary>
public class StudentAttemptsWithBestDto
{
    public List<StudentAttemptDto> Attempts { get; set; } = new();
    public StudentAttemptDto? BestAttempt { get; set; }
}

public class CertificateResult<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

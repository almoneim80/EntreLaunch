namespace EntreLaunch.DTOs.ProgressDtos;

public class TrainingPathProgressDetailsDto
{
#nullable disable
    public int PathId { get; set; }
    public string PathName { get; set; }

    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; }

    public int CompletedCoursesCount { get; set; }
    public int TotalCoursesCount { get; set; }

    public bool IsCompleted { get; set; }
    public double CompletionPercentage { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public TimeSpan TotalTimeSpent { get; set; }

    public List<CourseProgressSummaryDto> Courses { get; set; } = new();
}

public class CourseProgressSummaryDto
{
#nullable disable
    public int CourseId { get; set; }
    public string CourseName { get; set; }

    public bool IsCompleted { get; set; }
    public double CompletionPercentage { get; set; }
}

// Progress details for a specific course
public class CourseProgressDetailsDto
{
#nullable disable
    public int CourseId { get; set; }
    public string CourseName { get; set; }

    public int? PathId { get; set; }
    public string RelatedPathName { get; set; } = null;

    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; }

    public bool IsCompleted { get; set; }
    public double CompletionPercentage { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public TimeSpan TotalTimeSpent { get; set; }

    public List<LessonProgressSummaryDto> Lessons { get; set; } = new();
}

public class LessonProgressSummaryDto
{
#nullable disable
    public int LessonId { get; set; }
    public string LessonName { get; set; }

    public bool IsCompleted { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public TimeSpan TimeSpent { get; set; }
}

// Progress details for a specific lesson
public class LessonProgressDetailsDto
{
#nullable disable
    public int LessonId { get; set; }
    public string LessonName { get; set; }

    public int CourseId { get; set; }
    public string CourseName { get; set; }

    public int? PathId { get; set; }
    public string RelatedPathName { get; set; } = null;

    public bool IsCompleted { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public TimeSpan TimeSpent { get; set; }
}

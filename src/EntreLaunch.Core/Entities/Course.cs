using EntreLaunch.Interfaces.BaseIntf;

namespace EntreLaunch.Entities;

[SupportsElastic]
[SupportsChangeLog]
[Table("courses")]
public class Course : SharedData, IBaseEntity
{
    [Searchable]
    public string? Name { get; set; }
    [Searchable]
    public string? Description { get; set; }

    public int? PathId { get; set; }
    public virtual TrainingPath? TrainingPath { get; set; } = null;

    public int? FieldId { get; set; }
    public virtual CourseField? CourseField { get; set; } = null;

    [Searchable]
    public decimal? Price { get; set; } = 0;
    public decimal? Discount { get; set; } = 0;
    [Searchable]
    public string? StudyWay { get; set; }
    public int? DurationInDays { get; set; }
    public int? InstructorCount { get; set; } = 0;

    [Searchable]
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    [Searchable]
    public bool CertificateExists { get; set; }
    public int CertificateValidityInDays { get; set; } = 365; 
    public string? Logo { get; set; }
    public CourseStatus? Status { get; set; }
    public bool IsFree { get; set; }
    public CourseType? Type { get; set; }
    public DateTimeOffset? CompletionDate { get; set; }

    // University students , Private students, working professionals, beginners, children, etc.
    [Searchable]
    public List<string>? Audience { get; set; } = new();
    [Searchable]
    public List<string>? Requirements { get; set; } = new();
    public List<string>? Topics { get; set; } = new();
    public List<string>? Goals { get; set; } = new();
    public List<string>? Outcomes { get; set; } = new();

    public virtual ICollection<CourseRating>? CourseRatings { get; set; }
    public virtual ICollection<CourseInstructor>? CourseInstructors { get; set; }
    public virtual ICollection<Lesson>? Lessons { get; set; }
    public virtual ICollection<Exam>? Exams { get; set; }
    public virtual ICollection<Certificate>? StudentCertificates { get; set; }
    public virtual ICollection<StudentProgress>? StudentProgresses { get; set; }
    public virtual ICollection<CourseTag>? CourseTags { get; set; }

    // methods
    public bool IsCourseActive() => Status == CourseStatus.Published && StartDate <= DateTime.UtcNow && EndDate >= DateTime.UtcNow;
}

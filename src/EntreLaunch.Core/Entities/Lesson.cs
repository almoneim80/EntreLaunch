using EntreLaunch.Interfaces.BaseIntf;

namespace EntreLaunch.Entities;

[SupportsElastic]
[SupportsChangeLog]
[Table("lessons")]
public class Lesson : SharedData, IBaseEntity
{
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    [Searchable]
    public string? Name { get; set; }
    [Searchable]
    public string? VideoUrl { get; set; }
    public int? Order { get; set; } // orginal order
    public int? DurationInMinutes { get; set; }
    [Searchable]
    public string? Description { get; set; }
    public int? OrderIndex { get; set; } // order after sorting

    [Searchable]
    public virtual ICollection<LessonAttachment>? LessonAttachments { get; set; }
    public virtual ICollection<Exam>? Exams { get; set; }
}

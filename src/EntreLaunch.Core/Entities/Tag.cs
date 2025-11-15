namespace EntreLaunch.Entities;

public class Tag : SharedData
{
    public string Name { get; set; } = null!;

    public virtual ICollection<CourseTag>? CourseTags { get; set; }
}

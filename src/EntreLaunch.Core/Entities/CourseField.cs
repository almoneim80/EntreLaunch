namespace EntreLaunch.Entities;

public class CourseField : SharedData
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Course>? Courses { get; set; }
}

namespace EntreLaunch.Entities;

public class CourseTag : SharedData
{
    public int CourseId { get; set; }
    public virtual Course? Course { get; set; }

    public int TagId { get; set; }
    public virtual Tag? Tag { get; set; }
}

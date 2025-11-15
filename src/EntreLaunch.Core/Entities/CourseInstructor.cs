namespace EntreLaunch.Entities;

public class CourseInstructor : SharedData
{
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

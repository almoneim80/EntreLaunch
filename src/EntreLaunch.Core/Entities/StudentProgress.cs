namespace EntreLaunch.Entities;

public class StudentProgress : SharedData
{
    public string? UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    public int LastLessonId { get; set; }
    public virtual Lesson Lesson { get; set; } = null!;

    public bool IsCompleted { get; set; }
    public double CompletionPercentage { get; set; }
    public TimeSpan TotalTimeSpent { get; set; }
}

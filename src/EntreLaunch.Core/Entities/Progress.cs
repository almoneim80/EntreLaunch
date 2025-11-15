namespace EntreLaunch.Entities;

public class Progress : SharedData
{
    public string UserId { get; set; } = null!;
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    public int? CourseId { get; set; }
    [ForeignKey(nameof(CourseId))]
    public virtual Course? Course { get; set; } 

    public int? LessonId { get; set; }
    [ForeignKey(nameof(LessonId))]
    public virtual Lesson? Lesson { get; set; } 

    public int? PathId { get; set; }
    [ForeignKey(nameof(PathId))]
    public virtual TrainingPath? Path { get; set; }

    public bool IsCompleted { get; set; } = false;
    public decimal ProgressPercentage { get; set; } = 0;

    public DateTimeOffset? CompletedAt { get; set; }
}

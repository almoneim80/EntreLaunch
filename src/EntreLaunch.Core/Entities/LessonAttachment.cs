namespace EntreLaunch.Entities;

public class LessonAttachment : SharedData
{
    public int LessonId { get; set; }
    public virtual Lesson Lesson { get; set; } = null!;

    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public int OpenCount { get; set; }
}

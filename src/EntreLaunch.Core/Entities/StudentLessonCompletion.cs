namespace EntreLaunch.Entities
{
    public class StudentLessonCompletion : SharedData
    {
        public string UserId { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; } = null!;

        public DateTimeOffset CompletedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}

namespace EntreLaunch.Entities
{
    [SupportsElastic]
    [SupportsChangeLog]
    [Table("exams")]
    public class Exam : SharedData
    {
        [Searchable]
        public string? Name { get; set; }
        [Searchable]
        public string? Description { get; set; }
        public decimal? MinMark { get; set; }
        public decimal? MaxMark { get; set; }
        public int? DurationInMinutes { get; set; }
        public int? MaxAttempts { get; set; } = 1;
        public ExamParentEntityType ParentEntityType { get; set; }

        public int? CourseId { get; set; }
        public virtual Course? Course { get; set; }

        public int? LessonId { get; set; }
        public virtual Lesson? Lesson { get; set; }

        public ExamStatus Status { get; set; }

        public virtual ICollection<Question>? Questions { get; set; }
        public virtual ICollection<ExamResult>? ExamResults { get; set; }
    }
}

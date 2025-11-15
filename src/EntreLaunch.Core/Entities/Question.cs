namespace EntreLaunch.Entities;

public class Question : SharedData
{
    public int ExamId { get; set; }
    public virtual Exam Exam { get; set; } = null!;

    public string? Text { get; set; }
    public decimal? Mark { get; set; }

    public virtual ICollection<Answer>? Answers { get; set; }
}

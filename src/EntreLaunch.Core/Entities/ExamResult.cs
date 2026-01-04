namespace EntreLaunch.Entities;

public class ExamResult : SharedData
{
    public int ExamId { get; set; }
    public virtual Exam Exam { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    public decimal? Mark { get; set; }
    public string? Status { get; set; }
    public int AttemptNumber { get; set; }
    public bool IsActive { get; set; }
}

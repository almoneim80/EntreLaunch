namespace EntreLaunch.Entities;

public class Answer : SharedData
{
    public int QuestionId { get; set; }
    public virtual Question Question { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
}

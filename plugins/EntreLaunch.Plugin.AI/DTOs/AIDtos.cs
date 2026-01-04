namespace EntreLaunch.Plugin.AI.DTOs;

public class QuizRequest
{
    public string? Topic { get; set; }
    public string? UserId { get; set; }
    public int QuestionNumber { get; set; }
    public int MaxMark { get; set; }
    public int MinMark { get; set; }
}

public class CorrectionRequest
{
    // نص السؤال المُرسل إلى dify.ai للتصحيح
    public string QuestionText { get; set; } = string.Empty;

    // إجابة المستخدم على السؤال
    public string UserAnswer { get; set; } = string.Empty;
}

public class ExamAttemptRequest
{
    public int ExamId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<ExamAnswerRequest> Answers { get; set; } = new List<ExamAnswerRequest>();
}

public class ExamAnswerRequest
{
    public int QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
}

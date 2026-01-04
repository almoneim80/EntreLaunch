#nullable disable
using System.Text.Json.Serialization;

namespace EntreLaunch.Plugin.SmartExam.Dtos;
public class AIRequestDataDto
{
    public string ExamTopic { get; set; }
    public int QuestionsNumber { get; set; }
    public int MinMark { get; set; }
    public int MaxMark { get; set; }
    public string language { get; set; }
}

public class AIFileRequestDataDto
{
    public List<string> fileIds { get; set; }
    public int QuestionsNumber { get; set; }
    public int MinMark { get; set; }
    public int MaxMark { get; set; }
    public string Language { get; set; }
}

public class ExamDtoWithQuestions
{
    public string ExamTitle { get; set; }
    public string Description { get; set; }
    public List<QuestionDtoWithAnswers> Questions { get; set; }
}

public class QuestionDtoWithAnswers
{
    public int No { get; set; }
    public string Text { get; set; }
    public int Mark { get; set; }
    public List<AnswerDto> Answers { get; set; }
}

//public class AnswerDto
//{
//    public string Text { get; set; }
//    public bool IsCorrect { get; set; }
//}

public class GenerateQuestionsRequestDto
{
    [FromForm]
    public int QuestionsNumber { get; set; }

    [FromForm]
    public int MinMark { get; set; }

    [FromForm]
    public int MaxMark { get; set; }

    [FromForm]
    public string Language { get; set; }
}

public class QuestionAnswerDto
{
    public int QuestionId { get; set; }
    public string Answer { get; set; }
}

public class ExamGradeResultDto
{
    public int ExamId { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public double TotalScore { get; set; }

    // يمكن إضافة تفاصيل أكثر:
    public List<QuestionGradeDetailDto> QuestionsDetail { get; set; }
}

public class QuestionGradeDetailDto
{
    public int QuestionId { get; set; }
    public string UserAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public double QuestionMark { get; set; }
    public double AwardedMark { get; set; }
    public string CorrectAnswer { get; set; } // إن كان سؤال متعدد الاختيارات
    public string Feedback { get; set; } // ملاحظات من الـ AI أو أي شرح إن لزم
}

public class ExamResponseDto
{
    [JsonPropertyName("Exam-title")]
    public string ExamTitle { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("questions")]
    public List<QuestionResponseDto> Questions { get; set; }
}

public class QuestionResponseDto
{
    [JsonPropertyName("No.")]
    public int Number { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("mark")]
    public int Mark { get; set; }

    [JsonPropertyName("answers")]
    public List<AnswerDto> Answers { get; set; }
}

public class AnswerDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("isCorrect")]
    public bool IsCorrect { get; set; }
}

namespace EntreLaunch.DTOs;

public class QuestionCreateDto
{
    public int ExamId { get; set; }
    public string? Text { get; set; }
    public decimal? Mark { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class QuestionEntreLaunchdateDto
{
    public int ExamId { get; set; }
    public string? Text { get; set; }
    public decimal? Mark { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class QuestionDetailsDto : QuestionCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class QuestionExportDto
{
    public int ExamId { get; set; }
    public string? Text { get; set; }
    public decimal? Mark { get; set; }
}

public class QuestionCreateDtoWithChildren
{
    public string? Text { get; set; }
    public decimal? Mark { get; set; }

    public List<AnswerCreateDtoWithChildren>? Answers { get; set; }
}

public class QuestionWithAnswers : QuestionCreateDtoWithChildren
{
    public int ExamId { get; set; }
}

// ALL Question DATA WITH RELATIONSHIPS
public class QuestionWithAnswersFullData
{
    public int QuestionId { get; set; }
    public string? Text { get; set; }
    public decimal? Mark { get; set; }
    public List<AnswerFullData> Answers { get; set; } = new();
}

public class AnswerFullData
{
    public int AnswerId { get; set; }
    public string? Text { get; set; }
    public bool IsCorrect { get; set; }
}

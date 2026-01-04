namespace EntreLaunch.DTOs.ExamDtos;

public class AnswerCreateDto
{
    public int QuestionId { get; set; }
    public string? Text { get; set; }
    public bool? IsCorrect { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class AnswerUpdateDto
{
    public int QuestionId { get; set; }
    public string? Text { get; set; }
    public bool? IsCorrect { get; set; }

    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class AnswerDetailsDto : AnswerCreateDto
{
    public int Id { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class AnswerExportDto
{
    public int QuestionId { get; set; }
    public string? Text { get; set; }
    public bool? IsCorrect { get; set; }
}

public class AnswerCreateDtoWithChildren
{
    public string? Text { get; set; }
    public bool? IsCorrect { get; set; }
}

public class AnswerImportDto : BaseEntityWithId
{
    public int QuestionId { get; set; }
    public string? Text { get; set; }
    public bool? IsCorrect { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

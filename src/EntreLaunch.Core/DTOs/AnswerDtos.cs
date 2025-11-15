using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class AnswerCreateDto
{
    public int QuestionId { get; set; }
    public string? Text { get; set; }
    public bool? IsCorrect { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class AnswerEntreLaunchdateDto
{
    public int QuestionId { get; set; }
    public string? Text { get; set; }
    public bool? IsCorrect { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class AnswerDetailsDto : AnswerCreateDto
{
    public int Id { get; set; }
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
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

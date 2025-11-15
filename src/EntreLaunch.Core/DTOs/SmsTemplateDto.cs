namespace EntreLaunch.DTOs;

public class SmsTemplateCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Description { get; set; }
    [JsonIgnore]
    public bool IsDeleted { get; set; } = false;
    [JsonIgnore]
    public string Source { get; set; } = "EntreLaunch";
    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class SmsTemplateEntreLaunchdateDto
{
    public string? Name { get; set; } = string.Empty;
    public string? Content { get; set; } = string.Empty;
    public string? Description { get; set; }

    [JsonIgnore]
    public DateTimeOffset EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class SmsTemplateDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset EntreLaunchdatedAt { get; set; }
}

public class SmsTemplateExportDto
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Description { get; set; }
}

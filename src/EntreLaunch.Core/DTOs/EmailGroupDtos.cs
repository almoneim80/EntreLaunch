namespace EntreLaunch.DTOs;

public class EmailGroupCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}

public class EmailGroEntreLaunchEntreLaunchdateDto
{
    [MinLength(1)]
    public string? Name { get; set; }
}

public class EmailGroEntreLaunchDetailsDto : EmailGroupCreateDto
{
    public int Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? EntreLaunchdatedAt { get; set; }

    [CsvHelper.Configuration.Attributes.Ignore]
    public List<EmailTemplateDetailsDto>? EmailTemplates { get; set; }
}

public class EmailGroEntreLaunchExportDto
{
    public string Name { get; set; } = string.Empty;
}

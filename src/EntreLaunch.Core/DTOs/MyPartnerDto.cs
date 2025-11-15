namespace EntreLaunch.DTOs;

/// <summary>
/// Data of MyPartner create.
/// </summary>
public class MyPartnerCreateDto
{
    public string? Activity { get; set; }
    public string? City { get; set; }
    public string? Sector { get; set; }
    public decimal? Cost { get; set; }
    public string? Idea { get; set; }
    public List<string>? AcceptRequirements { get; set; }
    public decimal? CapitalFrom { get; set; }
    public decimal? CapitalTo { get; set; }
    [JsonIgnore]
    public string? UserId { get; set; }
    public string? Contact { get; set; }
    public MyPartnerStatus Status { get; set; } = MyPartnerStatus.Pending;
    public List<MyPartnerAttachmentCreateDto>? Attachments { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of MyPartner EntreLaunchdate.
/// </summary>
public class MyPartnerEntreLaunchdateDto
{
    public string? Activity { get; set; }
    public string? City { get; set; }
    public string? Sector { get; set; }
    public decimal? Cost { get; set; }
    public string? Idea { get; set; }
    public List<string>? AcceptRequirements { get; set; }
    public decimal? CapitalFrom { get; set; }
    public decimal? CapitalTo { get; set; }
    public string? UserId { get; set; } = null!;
    public string? Contact { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of MyPartner details.
/// </summary>
public class MyPartnerDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string Activity { get; set; }
    public string City { get; set; }
    public string Sector { get; set; }
    public decimal Cost { get; set; }
    public string Idea { get; set; }
    public List<string> AcceptRequirements { get; set; }
    public decimal CapitalFrom { get; set; }
    public decimal CapitalTo { get; set; }
    public string UserId { get; set; } = null!;
    public string Contact { get; set; }
    public MyPartnerStatus Status { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset EntreLaunchdatedAt { get; set; }

    public List<ProjectAttachmentDetailsDto> Attachments { get; set; }
}

/// <summary>
/// Data of MyPartnerAttachment create.
/// </summary>
public class MyPartnerAttachmentCreateDto
{
#nullable disable
    public string Url { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of MyPartnerAttachment EntreLaunchdate.
/// </summary>
public class ProjectAttachmentEntreLaunchdateDto
{
    public int? ProjectId { get; set; }
    public string Url { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of MyPartnerAttachment details.
/// </summary>
#nullable disable
public class ProjectAttachmentDetailsDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset EntreLaunchdatedAt { get; set; }
}

#nullable disable
public class FilterProjectsDto
{
    public string Activity { get; set; }
    public string City { get; set; }
    public decimal CapitalFrom { get; set; }
    public decimal CapitalTo { get; set; }
}

public class ProcessProjectsDto
{
    public int ProjectId { get; set; }
    public MyPartnerStatus Status { get; set; }
}

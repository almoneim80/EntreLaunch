namespace EntreLaunch.DTOs;

/// <summary>
/// Data of Employee create.
/// </summary>
public class EmployeeCreateDto
{
#nullable disable
    [JsonIgnore]
    public string UserId { get; set; }
    public string WorkField { get; set; }
    public string JobTitle { get; set; }
    public string EmployeeDefinition { get; set; }
    public List<string> Skills { get; set; }

    public List<EmployeePortfolioCreateDto> EmployeePortfolio { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Data of Employee EntreLaunchdate.
/// </summary>
public class EmployeeEntreLaunchdateDto
{
#nullable enable
    [JsonIgnore]
    public string? UserId { get; set; }
    public string? WorkField { get; set; }
    public string? JobTitle { get; set; }
    public string? EmployeeDefinition { get; set; }
    public List<string>? Skills { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Data of Employee details.
/// </summary>
public class EmployeeDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string UserId { get; set; }
    public string WorkField { get; set; }
    public string JobTitle { get; set; }
    public string EmployeeDefinition { get; set; }
    public List<string> Skills { get; set; }
    public EmployeeStaus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset EntreLaunchdatedAt { get; set; }

    public List<EmployeePortfolioDetailsDto> Portfolios { get; set; }
}

/// <summary>
/// Data of Employee Portfolio create.
/// </summary>
public class EmployeePortfolioCreateDto
{
#nullable disable
    public string ProjectTitle { get; set; }
    public decimal CostFrom { get; set; }
    public decimal CostTo { get; set; }
    public string Description { get; set; }
    public string About { get; set; }
    public string Logo { get; set; }
    public List<PortfolioAttachmentCreateDto> PortfolioAttachments { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Data of Employee Portfolio EntreLaunchdate.
/// </summary>
public class EmployeePortfolioEntreLaunchdateDto
{
#nullable enable
    public string? ProjectTitle { get; set; }
    public decimal? CostFrom { get; set; }
    public decimal? CostTo { get; set; }
    public string? Description { get; set; }
    public int? EmployeeId { get; set; }
    [JsonIgnore]
    public DateTimeOffset EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Data of Employee Portfolio details.
/// </summary>
public class EmployeePortfolioDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string ProjectTitle { get; set; }
    public decimal CostFrom { get; set; }
    public decimal CostTo { get; set; }
    public string About { get; set; }
    public string Logo { get; set; }
    public string Description { get; set; }

    public List<PortfolioAttachmentDetailsDto> PortfolioAttachments { get; set; }
}

/// <summary>
/// Data of Portfolio Attachment create.
/// </summary>
public class PortfolioAttachmentCreateDto
{
#nullable disable
    public string Url { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Data of Portfolio Attachment EntreLaunchdate.
/// </summary>
public class PortfolioAttachmentEntreLaunchdateDto
{
#nullable enable
    public int? PortfolioId { get; set; }
    public string? Url { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Data of Portfolio Attachment details.
/// </summary>
public class PortfolioAttachmentDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public string Url { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset EntreLaunchdatedAt { get; set; }
}

public class EmployeeRequestDto
{
    public int ProjectId { get; set; }
    public EmployeeStaus Status { get; set; }
}

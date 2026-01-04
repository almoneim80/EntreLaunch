namespace EntreLaunch.DTOs.MyOpportunityDtos;

/// <summary>
/// Data of Investment Opportunity create.
/// </summary>
public class OpportunityCreateDto
{
    public string? CompanyName { get; set; }
    public string? Logo { get; set; }
    public string? Description { get; set; }
    public string? Sector { get; set; }
    public decimal? Costs { get; set; }
    public int? ContractDurationInDay { get; set; }
    public List<string>? AcceptRequirements { get; set; }
    public Country? BrandCountry { get; set; }
    public OpportunityType Type { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Data of Investment Opportunity update.
/// </summary>
public class OpportunityUpdateDto
{
    public string? CompanyName { get; set; }
    public string? Logo { get; set; }
    public string? Description { get; set; }
    public string? Sector { get; set; }
    public decimal? Costs { get; set; }
    public int? ContractDurationInDay { get; set; }
    public List<string>? AcceptRequirements { get; set; }
    public Country BrandCountry { get; set; }
    public OpportunityType Type { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Data of Investment Opportunity details.
/// </summary>
public class OpportunityDetailsDto : OpportunityCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class OpportunityExportDto
{
    public string? CompanyName { get; set; }
    public string? Logo { get; set; }
    public string? Description { get; set; }
    public string? Sector { get; set; }
    public decimal? Costs { get; set; }
    public int? ContractDurationInDay { get; set; }
    public List<string>? AcceptRequirements { get; set; }
    public Country BrandCountry { get; set; }
    public OpportunityType Type { get; set; }
}

/// <summary>
/// Data of Investment Opportunity import.
/// </summary>
public class OpportunityImportDto : BaseEntityWithId
{
    public string? CompanyName { get; set; }
    public string? Logo { get; set; }
    public string? Description { get; set; }
    public string? Sector { get; set; }
    public decimal? Costs { get; set; }
    public int? ContractDurationInDay { get; set; }
    public List<string>? AcceptRequirements { get; set; }
    public Country? BrandCountry { get; set; }
    public OpportunityType Type { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

/// <summary>
/// Filter of Investment Opportunies.
/// </summary>
public class OpportunityFilterDto
{
    public string? Sector { get; set; }
    public decimal? Costs { get; set; }
    public Country? BrandCountry { get; set; }
}

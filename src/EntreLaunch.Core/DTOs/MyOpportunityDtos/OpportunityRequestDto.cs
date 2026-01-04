namespace EntreLaunch.DTOs.MyOpportunityDtos;

public class CreateOpportunityRequestDto
{
    public int OpportunityId { get; set; }
    [JsonIgnore]
    public string? userId { get; set; }

    public string? City { get; set; }
    public double? ShareCapital { get; set; }
    public decimal? LoanRatio { get; set; }
    public int ManagementExperince { get; set; }
    public bool HaveFranchiseProjects { get; set; }
    public int FranchiseExperince { get; set; }
    public bool FeasibillityStudyBring { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
public class OpportunityRequestDetailsDto
{
    public int Id { get; set; }
    public OpportunityRequestStatus Status { get; }
    public DateTimeOffset? CreatedAt { get; }
    public DateTimeOffset? UpdatedAt { get; }

    public OpportunitySummaryDto? OpportunityData => Opportunity == null ? null : new OpportunitySummaryDto
    {
        CompanyName = Opportunity.CompanyName,
        Description = Opportunity.Description,
        Sector = Opportunity.Sector,
        Costs = Opportunity.Costs,
        ContractDurationInDay = Opportunity.ContractDurationInDay,
        AcceptRequirements = Opportunity.AcceptRequirements,
        BrandCountry = Opportunity.BrandCountry ?? Country.ZZ,
        Type = Opportunity.Type,
    };
    public UserSummaryDto? UserData => User == null ? null : new UserSummaryDto
    {
        FullName = User.FirstName + " " + User.LastName,
        NationalId = User.NationalId,
        Email = User.Email,
        PhoneNumber = User.PhoneNumber,
        DateOfbirth = User.DOB,
        Specialization = User.Specialization,
        CountryCode = User.CountryCode
    };

    [JsonIgnore]
    public User? User { get; set; }
    [JsonIgnore]
    public Opportunity? Opportunity { get; set; }
}
public class ProcessOpportunityRequestDto
{
    public int Id { get; set; }
    public OpportunityRequestStatus Status { get; set; }
}
public class UserSummaryDto
{
    public string? FullName { get; set; }
    public double? NationalId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset? DateOfbirth { get; set; }
    public string? Specialization { get; set; }
    public Country? CountryCode { get; set; }
}
public class OpportunitySummaryDto
{
    public string? CompanyName { get; set; }
    public string? Description { get; set; }
    public string? Sector { get; set; }
    public decimal? Costs { get; set; }
    public int? ContractDurationInDay { get; set; }
    public List<string>? AcceptRequirements { get; set; }
    public Country BrandCountry { get; set; }
    public OpportunityType Type { get; set; }
}

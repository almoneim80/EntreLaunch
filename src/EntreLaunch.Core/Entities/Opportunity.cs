namespace EntreLaunch.Entities;

[SupportsElastic]
[SupportsChangeLog]
[Table("Opportunities")]
public class Opportunity : SharedData
{
    [Searchable]
    public string? CompanyName { get; set; }
    [Searchable]
    public string? Logo { get; set; }
    public string? Description { get; set; }
    [Searchable]
    public string? Sector { get; set; }
    [Searchable]
    public decimal? Costs { get; set; }
    public int? ContractDurationInDay { get; set; }
    public List<string>? AcceptRequirements { get; set; }
    [Searchable]
    public Country? BrandCountry { get; set; }

    public OpportunityType Type { get; set; }
}

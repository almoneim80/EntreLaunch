using EntreLaunch.Interfaces.BaseIntf;

namespace EntreLaunch.Entities;

[SupportsElastic]
[SupportsChangeLog]
[Table("simulations")]
public class Simulation : SharedData, IBaseEntity
{
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    [Searchable]
    public string? ProjectField { get; set; } // Commercial, Electronic, Service, etc.
    [Searchable]
    public string? ProjectType { get; set; } // Furniture, Haberdashery, Clothing, Groceries, etc.
    public SimulationStatus ProjectStatus { get; set; } // In progress, Completed, Discontinued, etc.

    public double? IdeaPowerhValue { get; set; } 
    public double? TotalPurchaseValue { get; set; }
    public double? TotalCampaignValue { get; set; }
    public string? AdvertisementLink { get; set; }
    public DateTimeOffset? AdvertisementEndDate { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    [Searchable]
    public virtual ICollection<SimulationIdeaPower>? SimulationIdeaPowers { get; set; }
    [Searchable]
    public virtual ICollection<SimulationBusinessPlan>? SimulationBusinessPlans { get; set; }
    [Searchable]
    public virtual ICollection<SimulationFeasibilityStudy>? SimulationFeasibilityStudies { get; set; }
    [Searchable]
    public virtual ICollection<SimulationPurchase>? SimulationPurchases { get; set; }
    [Searchable]
    public virtual ICollection<SimulationMarketing>? SimulationMarketings { get; set; }

    public virtual ICollection<SimulationAdvertisement>? SimulationAdvertisements { get; set; }
    [Searchable]
    public virtual ICollection<SimulationCampaign>? SimulationCampaigns { get; set; }
}

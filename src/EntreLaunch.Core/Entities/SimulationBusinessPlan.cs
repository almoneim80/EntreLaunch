using EntreLaunch.Interfaces.BaseIntf;

namespace EntreLaunch.Entities;

public class SimulationBusinessPlan : SharedData, IBaseEntity
{
    public int SimulationId { get; set; }
    public virtual Simulation Simulation { get; set; } = null!;

    public string? BusinessPlanFileUrl { get; set; }

    public List<string>? BusinessPartners { get; set; }

    public List<string>? ProjectActivities { get; set; }

    public List<string>? ValueProposition { get; set; }

    public List<string>? CustomerRelationships { get; set; }

    public List<string>? CustomerSegments { get; set; }

    public List<string>? RequiredResources { get; set; }

    public List<string>? DistributionChannels { get; set; }

    public List<string>? RevenueStreams { get; set; }

    public List<string>? CostStructure { get; set; }
}

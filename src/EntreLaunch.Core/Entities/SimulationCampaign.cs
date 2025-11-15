namespace EntreLaunch.Entities;

public class SimulationCampaign : SharedData
{
    public int SimulationId { get; set; }
    public virtual Simulation Simulation { get; set; } = null!;

    public string? Name { get; set; }
    public double? Cost { get; set; }
    public DateTimeOffset? EndAt { get; set; }
}

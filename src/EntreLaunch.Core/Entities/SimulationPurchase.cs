namespace EntreLaunch.Entities;

public class SimulationPurchase : SharedData
{
    public int SimulationId { get; set; }
    public virtual Simulation Simulation { get; set; } = null!;

    public string? ItemName { get; set; }
    public double? ItemCost { get; set; }
    public string? Description { get; set; }
}

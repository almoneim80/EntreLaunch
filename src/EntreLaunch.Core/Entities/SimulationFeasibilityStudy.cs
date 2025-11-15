namespace EntreLaunch.Entities;

public class SimulationFeasibilityStudy : SharedData
{
    public int SimulationId { get; set; }
    public virtual Simulation Simulation { get; set; } = null!;

    public string? ProjectName { get; set; }
    public double? MinCapital { get; set; }
    public double? MaxCapital { get; set; }
    public bool IsInterest { get; set; } = false;
    public double? InterestRate { get; set; }
    public double? MarketingCost { get; set; }
    public double? RentCost { get; set; }
    public double? DecorationCost { get; set; }
    public double? EquipmentCost { get; set; }
    public double? GovFees { get; set; }
    public double? InventoryCost { get; set; }
}

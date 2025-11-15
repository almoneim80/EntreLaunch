namespace EntreLaunch.Entities;

public class SimulationIdeaPower : SharedData
{
    public int SimulationId { get; set; }
    public virtual Simulation Simulation { get; set; } = null!;

    public Category? CategoryType { get; set; } // Positive, Negative.
    public string? CategoryName { get; set; } // Strengths, Weaknesses, etc.
    public string? StrengthFactor { get; set; } // Strength Name, Weakness Name, etc.
    public int? FactorScore { get; set; } // 1..5
}

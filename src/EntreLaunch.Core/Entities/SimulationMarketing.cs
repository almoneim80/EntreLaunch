using EntreLaunch.Interfaces.BaseIntf;

namespace EntreLaunch.Entities;

public class SimulationMarketing : SharedData, IBaseEntity
{
    public int SimulationId { get; set; }
    public virtual Simulation Simulation { get; set; } = null!;

    public string? ProductName { get; set; }
    public int? Quantity { get; set; }
    public double? UnitPrice { get; set; }

    public virtual ICollection<SimulationAdvertisement>? Advertisements { get; set; }
}

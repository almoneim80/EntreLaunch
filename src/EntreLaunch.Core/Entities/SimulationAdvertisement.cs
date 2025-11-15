namespace EntreLaunch.Entities;

public class SimulationAdvertisement : SharedData
{
    public int MarketingId { get; set; }
    public virtual SimulationMarketing Marketing { get; set; } = null!;
    public string? AdUrl { get; set; }
    public string? AdType { get; set; } // Image, Video
    public DateTimeOffset? EndAt { get; set; }
}

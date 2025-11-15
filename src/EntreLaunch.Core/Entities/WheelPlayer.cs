namespace EntreLaunch.Entities;

public class WheelPlayer : SharedData
{
    public string PlayerId { get; set; } = null!;
    public virtual User Player { get; set; } = null!;

    public DateTimeOffset? PlayedAt { get; set; }

    public int AwardId { get; set; }
    public virtual WheelAward Award { get; set; } = null!;

    public bool IsFree { get; set; } = true;
}

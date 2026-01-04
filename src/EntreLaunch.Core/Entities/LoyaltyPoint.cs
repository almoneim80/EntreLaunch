namespace EntreLaunch.Entities;

public class LoyaltyPoint : SharedData
{
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    public int? PaymentId { get; set; }
    public virtual Payment? Payment { get; set; } = null;

    public int PointsChanged { get; set; }

    public string? Reason { get; set; }
}

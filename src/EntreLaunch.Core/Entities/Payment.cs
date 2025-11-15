namespace EntreLaunch.Entities;
public class Payment : SharedData
{
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    public decimal? Amount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? NetAmount { get; set; }
    public string? Currency { get; set; }
    public string? DiscountCode { get; set; }
    public string Status { get; set; } = "Pending";

    public string? PaymentPurpose { get; set; }

    public int? TargetId { get; set; }

    public string? TargetType { get; set; }

    public DateTimeOffset? PaymentDate { get; set; }

    public virtual ICollection<Refund>? Refunds { get; set; }
    public virtual ICollection<PaymentTransaction>? PaymentTransactions { get; set; }

    public virtual ICollection<LoyaltyPoint>? LoyaltyPoints { get; set; }
}

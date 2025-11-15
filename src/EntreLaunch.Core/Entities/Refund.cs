namespace EntreLaunch.Entities;

public class Refund : SharedData
{
    public int PaymentId { get; set; }
    public virtual Payment Payment { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public DateTimeOffset RefundDate { get; set; } = DateTimeOffset.UtcNow;
    public ProcessStatus Status { get; set; } = ProcessStatus.Pending;
}

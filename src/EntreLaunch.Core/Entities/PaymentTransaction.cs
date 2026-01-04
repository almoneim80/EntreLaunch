namespace EntreLaunch.Entities;

public class PaymentTransaction : SharedData
{
    public int PaymentId { get; set; }
    public virtual Payment Payment { get; set; } = null!;

    public int? PaymentMethodId { get; set; } 
    public virtual PaymentMethod? PaymentMethod { get; set; }

    public string? ExternalTransactionId { get; set; }
    public string Status { get; set; } = "Pending";

    public List<string>? ResponseData { get; set; }
}

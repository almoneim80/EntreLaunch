namespace EntreLaunch.Entities;

public class PaymentMethod : SharedData
{
    public string? Name { get; set; }
    public int GatewayId { get; set; }
    public virtual PaymentGateway PaymentGateway { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public virtual ICollection<PaymentTransaction>? PaymentTransactions { get; set; }
}

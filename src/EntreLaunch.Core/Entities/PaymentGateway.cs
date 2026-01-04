using Newtonsoft.Json;

namespace EntreLaunch.Entities;

public class PaymentGateway : SharedData
{
    public string? Name { get; set; }
    public List<string>? ConfigurationData { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual ICollection<PaymentMethod>? PaymentMethods { get; set; }
}

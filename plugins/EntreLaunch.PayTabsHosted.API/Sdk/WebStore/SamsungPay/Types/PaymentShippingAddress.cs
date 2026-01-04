using System.Text.Json.Serialization;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.SamsungPay.Types
{
    /// <summary>
    /// Represents the details of the customer's shipping address and email address.
    /// </summary>
#nullable disable
    public class PaymentShippingAddress
    {
        [JsonPropertyName("shipping")]
        public ShippingDetails Shipping { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}

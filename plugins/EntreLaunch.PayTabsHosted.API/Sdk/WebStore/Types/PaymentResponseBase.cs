using System.Text.Json.Serialization;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// is a response to a payment.
    /// </summary>
#nullable disable
    public class PaymentResponseBase : PayTabsResponse
    {
        [JsonPropertyName("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonPropertyName("tran_type")]
        public string TransactionType { get; set; }

        [JsonPropertyName("cart_id")]
        public string CartId { get; set; }

        [JsonPropertyName("cart_description")]
        public string CartDescription { get; set; }

        [JsonPropertyName("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonPropertyName("cart_amount")]
        public string CartAmount { get; set; }

        [JsonPropertyName("return")]
        public string Return { get; set; }

        [JsonPropertyName("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonPropertyName("serviceId")]
        public int ServiceId { get; set; }

        [JsonPropertyName("profileId")]
        public int ProfileId { get; set; }

        [JsonPropertyName("merchantId")]
        public int MerchantId { get; set; }

        [JsonPropertyName("trace")]
        public string Trace { get; set; }
    }
}

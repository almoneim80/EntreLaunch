using EntreLaunch.PayTabsHosted.API.Types;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Refund
{
    /// <summary>
    /// Represents the response to a refund request.
    /// </summary>
#nullable disable
    public class RefundResponse : PayTabsResponse
    {
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public decimal CartAmount { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonPropertyName("payment_result")]
        public PayTabsPaymentResult PaymentResult { get; set; }
    }
}

using System.Text.Json.Serialization;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.QueryTransaction
{
    /// <summary>
    /// Represents a response to a transaction query using the basket ID.
    /// </summary>
#nullable disable
    public class QueryTransactionByCartIdResponse : PayTabsResponse
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

        [JsonPropertyName("customer_details")]
        public CustomerDetails CustomerDetails { get; set; }

        [JsonPropertyName("payment_result")]
        public PayTabsPaymentResult PaymentResult { get; set; }

        [JsonPropertyName("payment_info")]
        public PaymentInfo PaymentInfo { get; set; }

        [JsonPropertyName("serviceId")]
        public int ServiceId { get; set; }
    }
}

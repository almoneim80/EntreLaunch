using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Refund
{
    /// <summary>
    /// A Refund request is for an existing transaction.
    /// </summary>
#nullable disable
    public class CreateRefundRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonRequired]
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonRequired]
        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonRequired]
        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [JsonRequired]
        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonRequired]
        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonRequired]
        [JsonProperty("cart_amount")]
        public decimal CartAmount { get; set; }

        [JsonRequired]
        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("refund_reason")]
        public string RefundReason { get; set; }
    }
}

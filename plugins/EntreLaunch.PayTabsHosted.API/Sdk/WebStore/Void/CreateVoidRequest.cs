using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Void
{
    /// <summary>
    /// This is a request to cancel an existing transaction.
    /// </summary>
#nullable disable
    public class CreateVoidRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

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

        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("void_reason")]
        public string VoidReason { get; set; }
    }
}

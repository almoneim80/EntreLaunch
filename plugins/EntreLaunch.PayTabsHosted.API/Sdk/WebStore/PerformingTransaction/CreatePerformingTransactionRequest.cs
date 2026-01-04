using EntreLaunch.PayTabsHosted.API.Types;
using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Capture
{
#nullable disable
    /// <summary>
    /// https://{{domain}}/payment/request.
    /// A request to execute a payment or transaction using the requested data.
    /// </summary>
    public class CreatePerformingTransactionRequest : ApiRequestBody
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

        /// <summary>
        /// Gets or sets transaction Reference (tran_ref) is the parameter that Indicates the Transaction Reference on PayTabs side.
        /// Note that, this is the reference to all PayTabs transactions, also this can be used between the merchant 
        /// and customer communications or between merchant and PayTabs team communications.
        /// </summary>
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}

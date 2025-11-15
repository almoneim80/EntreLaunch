using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// is the basis for payment requests.
    /// </summary>
#nullable disable
    public class PaymentRequestBase : ApiRequestBody
    {
        /// <summary>
        /// Gets or sets the merchant Profile ID you can get from your PayTabs dashboard.
        /// </summary>
        [System.Text.Json.Serialization.JsonRequired]
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [System.Text.Json.Serialization.JsonRequired]
        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [System.Text.Json.Serialization.JsonRequired]
        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [System.Text.Json.Serialization.JsonRequired]
        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [System.Text.Json.Serialization.JsonRequired]
        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [System.Text.Json.Serialization.JsonRequired]
        [JsonProperty("cart_amount")]
        public decimal CartAmount { get; set; }

        [System.Text.Json.Serialization.JsonRequired]
        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }
    }
}

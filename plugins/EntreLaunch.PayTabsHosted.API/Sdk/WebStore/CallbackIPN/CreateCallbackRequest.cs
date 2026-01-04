using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.APi.WebStore.Callback
{
    /// <summary>
    /// Represents a request to create a Callback to receive notifications after a payment.
    /// </summary>
#nullable disable
    public class CreateCallbackRequest : ApiRequestBody
    {
        //[JsonProperty("tran_ref")]
        //public string TransactionReference { get; set; }

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
        public int CartAmount { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonProperty("callback")]
        public string CallbackUrl { get; set; }

        [JsonProperty("customer_details")]
        public CustomerDetails CustomerDetails { get; set; }

        [JsonProperty("shipping_details")]
        public ShippingDetails ShippingDetails { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}

using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.OwnForm
{
    /// <summary>
    /// Represents the response to a request to create a payment page.
    /// </summary>
#nullable disable
    public class PaymentOwnFormResponse : PayTabsResponse
    {
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public string CartAmount { get; set; }

        [JsonProperty("return")]
        public string Return { get; set; }

        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonProperty("customer_details")]
        public CustomerDetails CustomerDetails { get; set; }

        [JsonProperty("shipping_details")]
        public ShippingDetails ShippingDetails { get; set; }

        [JsonProperty("payment_info")]
        public PaymentInfo PaymentInfo { get; set; }
    }
}

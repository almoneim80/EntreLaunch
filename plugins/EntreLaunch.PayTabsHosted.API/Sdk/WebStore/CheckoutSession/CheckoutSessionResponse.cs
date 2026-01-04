using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;
using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.CheckoutSession
{
    /// <summary>
    /// A response to a payment session, containing details such as transaction reference, amount, links, and customer information.
    /// </summary>
#nullable disable
    public class CheckoutSessionResponse : PayTabsResponse
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

        [JsonProperty("tran_currency")]
        public string TransactionCurrency { get; set; }

        [JsonProperty("tran_total")]
        public string TransactionTotal { get; set; }

        [JsonProperty("callback")]
        public string Callback { get; set; }

        [JsonProperty("return")]
        public string Return { get; set; }

        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonProperty("customer_details")]
        public CustomerDetails CustomerDetails { get; set; }

        [JsonProperty("shipping_details")]
        public ShippingDetails ShippingDetails { get; set; }

        [JsonProperty("payment_result")]
        public PayTabsPaymentResult PaymentResult { get; set; }

        [JsonProperty("payment_info")]
        public PaymentInfo PaymentInfo { get; set; }

        [JsonProperty("serviceId")]
        public int ServiceId { get; set; }

        [JsonProperty("profileId")]
        public int ProfileId { get; set; }

        [JsonProperty("merchantId")]
        public int MerchantId { get; set; }

        [JsonProperty("trace")]
        public string Trace { get; set; }
    }
}

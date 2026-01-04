using Newtonsoft.Json;
using System.Text.Json.Serialization;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.SamsungPay
{
    /// <summary>
    /// Represents the response of the Samsung Pay payment process, including payment status and shipping information.
    /// </summary>
#nullable disable
    public class SamsungPayResponse : PayTabsResponse
    {
        [JsonPropertyName("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("paypage_lang")]
        public string PayPageLang { get; set; }

        [JsonProperty("callback")]
        public string Callback { get; set; }

        [JsonProperty("return")]
        public string ReturnUrl { get; set; }

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
    }
}

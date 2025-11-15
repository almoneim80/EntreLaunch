using EntreLaunch.PayTabsHosted.API.Types;
using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.CheckoutSession
{
    /// <summary>
    /// Request to register an invoice payment with details such as amount and payment method.
    /// </summary>
#nullable disable
    public class InvoicePaidRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("invoice_id")]
        public string InvoiceId { get; set; }

        [JsonProperty("invoice_currency")]
        public string InvoiceCurrency { get; set; }

        [JsonProperty("invoice_total")]
        public double InvoiceTotal { get; set; }

        [JsonProperty("pay_method")]
        public string PayMethod { get; set; }

        [JsonProperty("pay_description")]
        public string PayDescription { get; set; }
    }
}

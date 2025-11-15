using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Invoice
{
#nullable disable
    /// <summary>
    /// https://{{domain}}/payment/invoice/cancel.
    /// Request to cancel an invoice using the invoice ID.
    /// </summary>
    public class InvoiceCancelRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("invoice_id")]
        public string InvoiceId { get; set; }
    }
}

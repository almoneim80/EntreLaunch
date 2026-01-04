using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Invoice
{
#nullable disable
    /// <summary>
    /// https://{{domain}}/payment/invoice/{{invoice_id}}/status.
    /// A request to check the status of an invoice.
    /// </summary>
    public class InvoiceStatusRequest : ApiRequestBody
    {
        [JsonProperty("invoice_id")]
        public string InvoiceId { get; set; }
    }
}



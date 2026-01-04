using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Invoice
{
    /// <summary>
    /// A response to the invoice status, including the process reference and status.
    /// </summary>
#nullable disable
    public class InvoiceStatusResponse : PayTabsResponse
    {
        [JsonProperty("invoice_status")]
        public string InvoiceStatus { get; set; }

        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("tran_status")]
        public string TranStatus { get; set; }

        [JsonProperty("tran_status_msg")]
        public string TranStatusMsg { get; set; }
    }
}

using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Invoice
{
    /// <summary>
    /// Bill payment logging response, including status and messages.
    /// </summary>
#nullable disable
    public class InvoicePaidResponse : PayTabsResponse
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

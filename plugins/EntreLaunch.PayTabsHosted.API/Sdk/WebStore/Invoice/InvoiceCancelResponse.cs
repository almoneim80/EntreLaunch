using EntreLaunch.PayTabsHosted.API.Types;
using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.CheckoutSession
{
    /// <summary>
    /// An invoice cancellation response, including status and related messages.
    /// </summary>
#nullable disable
    public class InvoiceCancelResponse : PayTabsResponse
    {
        [JsonProperty("invoice_id")]
        public string InvoiceId { get; set; }

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

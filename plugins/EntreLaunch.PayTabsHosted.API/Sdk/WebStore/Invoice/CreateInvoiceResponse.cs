using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Invoice
{
    /// <summary>
    /// استجابة إنشاء فاتورة، تحتوي على معرف الفاتورة ورابطها.
    /// </summary>
#nullable disable
    public class CreateInvoiceResponse : PayTabsResponse
    {
        [JsonProperty("invoice_status")]
        public string InvoiceStatus { get; set; }

        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("invoice_id")]
        public string InvoiceId { get; set; }

        [JsonProperty("invoice_link")]
        public string InvoiceLink { get; set; }
    }
}

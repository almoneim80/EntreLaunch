using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Invoice
{
    /// <summary>
    /// In response to sending an invoice via email.
    /// </summary>
#nullable disable
    public class InvoiceEmailResponse : PayTabsResponse
    {
        //[JsonProperty("traderPostalCodeMatch", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonProperty("bulk_email")]
        public List<InvoiceBulkEmail> BulkEmail { get; set; }

        [JsonProperty("invoice_id")]
        public int InvoiceId { get; set; }

        [JsonProperty("invoice_link")]
        public string InvoiceLink { get; set; }
    }
}

using System.Text.Json.Serialization;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Invoice
{
    /// <summary>
    /// A response to send the invoice via text message.
    /// </summary>
#nullable disable
    public class InvoiceSmsResponse : PayTabsResponse
    {
        [JsonPropertyName("bulk_sms")]
        public List<InvoiceBulkSm> BulkSms { get; set; }

        [JsonPropertyName("invoice_id")]
        public int InvoiceId { get; set; }

        [JsonPropertyName("invoice_link")]
        public string InvoiceLink { get; set; }
    }
}

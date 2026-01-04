using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Invoice
{
    /// <summary>
    /// A request to send the invoice via email.
    /// </summary>
#nullable disable
    public class SendInvoiceEmailRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [JsonProperty("tokenise")]
        public int Tokenise { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public string CartAmount { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonProperty("hide_shipping")]
        public bool HideShipping { get; set; }

        [JsonProperty("customer_ref")]
        public string CustomerRef { get; set; }

        [JsonProperty("invoice")]
        public InvoiceData Invoice { get; set; }
    }
}

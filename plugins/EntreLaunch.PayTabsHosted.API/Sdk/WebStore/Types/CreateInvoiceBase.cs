using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// is a request to create an invoice.
    /// </summary>
#nullable disable
    public class CreateInvoiceBase : ApiRequestBody
    {
        /// <summary>
        /// Gets or sets the merchant Profile ID you can get from your PayTabs dashboard.
        /// </summary>
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("tokenise")]
        public string Tokenise { get; set; }

        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public decimal CartAmount { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }
    }
}

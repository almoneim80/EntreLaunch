using Newtonsoft.Json;
namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents a request associated with previous transactions.
    /// </summary>
#nullable disable
    public class ParentRequest
    {
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public string CartAmount { get; set; }
    }
}

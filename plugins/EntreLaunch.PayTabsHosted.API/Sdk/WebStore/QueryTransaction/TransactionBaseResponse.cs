using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.QueryTransaction
{
    /// <summary>
    /// is a generic response to any transaction.
    /// </summary>
#nullable disable
    public class TransactionBaseResponse : PayTabsResponse
    {
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public string CartAmount { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonProperty("tran_currency")]
        public string TransactionCurrency { get; set; }

        [JsonProperty("tran_total")]
        public string TransactionTotal { get; set; }

        [JsonProperty("paypage_lang")]
        public string PayPageLang { get; set; }

        [JsonProperty("callback")]
        public string CallbackUrl { get; set; }

        [JsonProperty("return")]
        public string ReturnUrl { get; set; }

        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }
    }
}

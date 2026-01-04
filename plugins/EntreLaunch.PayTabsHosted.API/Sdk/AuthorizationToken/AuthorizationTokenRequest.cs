using System.Text.Json.Serialization;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.AuthorizationToken
{
    /// <summary>
    /// Sends a request for an authorization token to execute payments on behalf of the merchant.
    /// </summary>
#nullable disable
    public class AuthorizationTokenRequest : ApiRequestBody
    {
        [JsonPropertyName("profile_id")]
        public int ProfileId { get; set; }

        [JsonPropertyName("tran_type")]
        public string TransactionType { get; set; }

        [JsonPropertyName("tran_class")]
        public string TransactionClass { get; set; }

        [JsonPropertyName("cart_id")]
        public string CartId { get; set; }

        [JsonPropertyName("tokenise")]
        public string Tokenise { get; set; }

        [JsonPropertyName("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonPropertyName("cart_amount")]
        public int CartAmount { get; set; }

        [JsonPropertyName("cart_description")]
        public string CartDescription { get; set; }

        [JsonPropertyName("return")]
        public string Return { get; set; }

        [JsonPropertyName("callback")]
        public string Callback { get; set; }

        [JsonPropertyName("hide_shipping")]
        public bool HideShipping { get; set; }

        [JsonPropertyName("customer_details")]
        public CustomerDetails CustomerDetails { get; set; }
    }
}

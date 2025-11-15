using System.Text.Json.Serialization;
using EntreLaunch.PayTabsHosted.API.Types;
using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.AuthorizationToken
{
    /// <summary>
    /// Represents the response resulting from a token request, such as transaction details and the generated token.
    /// </summary>
#nullable disable
    public class AuthorizationTokenResponse : PayTabsResponse
    {
        [JsonPropertyName("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonPropertyName("tran_type")]
        public string TransactionType { get; set; }

        [JsonPropertyName("cart_id")]
        public string CartId { get; set; }

        [JsonPropertyName("cart_description")]
        public string CartDescription { get; set; }

        [JsonPropertyName("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonPropertyName("cart_amount")]
        public string CartAmount { get; set; }

        [JsonPropertyName("tran_currency")]
        public string TransactionCurrency { get; set; }

        [JsonPropertyName("tran_total")]
        public string TransactionTotal { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("callback")]
        public string Callback { get; set; }

        [JsonPropertyName("return")]
        public string Return { get; set; }

        [JsonPropertyName("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonPropertyName("serviceId")]
        public int ServiceId { get; set; }

        [JsonPropertyName("profileId")]
        public int ProfileId { get; set; }

        [JsonPropertyName("merchantId")]
        public int MerchantId { get; set; }

        [JsonPropertyName("payment_result")]
        public PayTabsPaymentResult PaymentResult { get; set; }

        [JsonPropertyName("trace")]
        public string Trace { get; set; }

        [JsonProperty("show_save_card")]
        public bool ShowSaveCard { get; internal set; }
    }
}

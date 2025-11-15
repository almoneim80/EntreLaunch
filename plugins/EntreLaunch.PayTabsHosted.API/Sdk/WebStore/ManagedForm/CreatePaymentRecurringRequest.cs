using System.Text.Json.Serialization;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.ManagedForm
{
    /// <summary>
    /// Represents a request to create a recurring payment on the payment page.
    /// </summary>
#nullable disable
    public class CreatePaymentRecurringRequest : ApiRequestBody
    {
        [JsonPropertyName("profile_id")]
        public int ProfileId { get; set; }

        [JsonPropertyName("tran_type")]
        public string TransactionType { get; set; }

        [JsonPropertyName("tran_class")]
        public string TransactionClass { get; set; }

        [JsonPropertyName("cart_id")]
        public string CartId { get; set; }

        [JsonPropertyName("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonPropertyName("cart_amount")]
        public int CartAmount { get; set; }

        [JsonPropertyName("cart_description")]
        public string CartDescription { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("tran_ref")]
        public string TransactionReference { get; set; }
    }
}

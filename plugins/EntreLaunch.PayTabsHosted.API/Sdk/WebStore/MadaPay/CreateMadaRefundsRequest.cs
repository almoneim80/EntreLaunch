using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.MadaPay
{
    /// <summary>
    /// Represents a request for a refund using Mada Pay.
    /// </summary>
#nullable disable
    public class CreateMadaRefundsRequest : ApiRequestBody
    {
        public CreateMadaRefundsRequest(int profileId, string tranType, string tranClass, string[] paymentMethods, string cartId, string cartCurrency, decimal cartAmount, string cartDescription, string tranRef)
        {
            ProfileId = profileId;
            TransactionType = tranType;
            TransactionClass = tranClass;
            PaymentMethods = paymentMethods;
            CartId = cartId;
            CartCurrency = cartCurrency;
            CartAmount = cartAmount;
            CartDescription = cartDescription;
            TransactionReference = tranRef;
        }

        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [JsonProperty("payment_methods")]
        public string[] PaymentMethods { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public decimal CartAmount { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }
    }
}

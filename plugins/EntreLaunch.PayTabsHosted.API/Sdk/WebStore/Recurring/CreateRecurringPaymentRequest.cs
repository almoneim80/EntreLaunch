using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Recurring
{
    /// <summary>
    /// This is a request to create a Recurring Payment.
    /// </summary>
#nullable disable
    public class CreateRecurringPaymentRequest : ApiRequestBody
    {
        public CreateRecurringPaymentRequest(int profileId, string tranType, string tranClass, string cartId, string cartCurrency, int cartAmount, string cartDescription, string token, string tranRef, bool hideShipping, string callback, string @return)
        {
            ProfileId = profileId;
            TransactionType = tranType;
            TransactionClass = tranClass;
            CartId = cartId;
            CartCurrency = cartCurrency;
            CartAmount = cartAmount;
            CartDescription = cartDescription;
            Token = token;
            TransactionReference = tranRef;
            HideShipping = hideShipping;
            Callback = callback;
            Return = @return;
        }

        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public int CartAmount { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("hide_shipping")]
        public bool HideShipping { get; set; }

        [JsonProperty("callback")]
        public string Callback { get; set; }

        [JsonProperty("return")]
        public string Return { get; set; }
    }
}

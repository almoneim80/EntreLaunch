using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Capture
{
#nullable disable
    /// <summary>
    /// https://{{domain}}/payment/request.
    /// This is a request to execute a Capture Payment for an existing transaction.
    /// </summary>
    public class CreateCapturePaymentRequest : ApiRequestBody
    {
        //public CreateCapturePaymentRequest(string profileId, string transactionType, string tranClass, string cartId, string cartCurrency, double cartAmount, string cartDescription, string transactionReference, string token)
        //{
        //    ProfileId = profileId;
        //    TransactionType = transactionType;
        //    TranClass = tranClass;
        //    CartId = cartId;
        //    CartCurrency = cartCurrency;
        //    CartAmount = cartAmount;
        //    CartDescription = cartDescription;
        //    TransactionReference = transactionReference;
        //    Token = token;
        //}

        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        /// <summary>
        /// Gets or sets transaction Reference (tran_ref) is the parameter that Indicates the Transaction Reference on PayTabs side.
        /// Note that, this is the reference to all PayTabs transactions, also this can be used between the merchant 
        /// and customer communications or between merchant and PayTabs team communications.
        /// </summary>
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

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

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}

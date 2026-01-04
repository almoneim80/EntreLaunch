using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.CheckoutSession
{
#nullable disable
    /// <summary>
    /// https://{{domain}}/payment/request.
    /// A request to query a payment session using the transaction reference.
    /// </summary>
    public class GetCheckoutSessionRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        /// <summary>
        /// Gets or sets transaction Reference (tran_ref) is the parameter that Indicates the Transaction Reference on PayTabs side.
        /// Note that, this is the reference to all PayTabs transactions, also this can be used between the merchant 
        /// and customer communications or between merchant and PayTabs team communications.
        /// </summary>
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}

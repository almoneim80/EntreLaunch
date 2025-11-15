using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.AuthorizationToken
{
    /// <summary>
    /// Represents the response to a token query request, with details such as the transaction reference and payment status.
    /// </summary>
#nullable disable
    public class QueryTokenResponse : PayTabsResponse
    {
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("payment_info")]
        public string PaymentInfo { get; set; }

        [JsonProperty("customer_details")]
        public string CustomerDetails { get; set; }

        [JsonProperty("payment_result")]
        public PayTabsPaymentResult PaymentResult { get; set; }
    }
}

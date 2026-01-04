using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.CallbackIPN
{
    /// <summary>
    /// Represents a response to the Callback IPN (push notification) service.
    /// </summary>
#nullable disable
    public class CallbackResponse : PayTabsResponse
    {
        [JsonProperty("tokenise")]
        public string Tokenise { get; internal set; }

        [JsonProperty("payment_result")]
        public PayTabsPaymentResult PaymentResult { get; set; }

        [JsonProperty("ipn_trace")]
        public string IPNTrace { get; set; }
    }
}

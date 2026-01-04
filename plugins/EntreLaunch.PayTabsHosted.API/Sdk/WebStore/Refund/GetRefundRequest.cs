using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Refund
{
    /// <summary>
    /// Represents a query request for details of a particular refund.
    /// </summary>
#nullable disable
    public class GetRefundRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("refund_reason")]
        public string RefundReason { get; set; }
    }
}

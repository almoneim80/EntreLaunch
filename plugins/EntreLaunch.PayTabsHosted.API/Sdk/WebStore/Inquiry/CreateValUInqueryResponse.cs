using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Inquiry
{
    /// <summary>
    /// represents a response to a ValU query request.
    /// </summary>
#nullable disable
    public class CreateValUInqueryResponse : PayTabsResponse
    {
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("payment_result")]
        public PayTabsPaymentResult PaymentResult { get; set; }
    }
}

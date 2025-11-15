using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace EntreLaunch.PayTabsHosted.API.Types
{
#nullable disable
    /// <summary>
    /// The payment result is represented as the status of the operation and associated messages.
    /// "payment_result": {
    ///    "response_status": "A",
    ///    "response_code": "G29717",
    ///    "response_message": "Authorised",
    ///    "acquirer_message": "SUCCESS",
    ///    "acquirer_rrn": "CL081222929169",
    ///    "cvv_result": " ",
    ///    "avs_result": " ",
    ///    "transaction_time": "2022-12-14T14:55:17Z"
    /// }.
    /// </summary>
    public class PayTabsPaymentResult
    {
        [JsonProperty("response_status")]
        public string ResponseStatus { get; set; }

        [JsonProperty("response_code")]
        public string ResponseCode { get; set; }

        [JsonProperty("response_message")]
        public string ResponseMessage { get; set; }

        [JsonProperty("acquirer_message")]
        public string AcquirerMessage { get; set; }

        [JsonProperty("acquirer_rrn")]
        public string AcquirerRrn { get; set; }

        [JsonPropertyName("cvv_result")]
        public string CvvResult { get; set; }

        [JsonPropertyName("avs_result")]
        public string AvsResult { get; set; }

        [JsonProperty("transaction_time")]
        public string TransactionTime { get; set; }
    }
}

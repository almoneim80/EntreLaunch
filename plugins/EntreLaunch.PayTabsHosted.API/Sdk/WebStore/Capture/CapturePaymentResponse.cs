using Newtonsoft.Json;
using System;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Capture
{
    /// <summary>
    /// is a response to a payment collection request.
    /// </summary>
#nullable disable
    public class CapturePaymentResponse : PayTabsResponse
    {
        [JsonRequired]
        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonRequired]
        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonRequired]
        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [JsonRequired]
        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonRequired]
        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonRequired]
        [JsonProperty("cart_amount")]
        public decimal CartAmount { get; set; }

        [JsonRequired]
        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonProperty("payment_result")]
        public PayTabsPaymentResult PaymentResult { get; set; }

        [JsonProperty("payment_info")]
        public PaymentInfo PaymentInfo { get; set; }
    }
}

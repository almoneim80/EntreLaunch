using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;
using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace EntreLaunch.PayTabsHosted.API.WebStore.MadaPay
{
    /// <summary>
    /// Represents a response to a refund request using Mada Pay.
    /// </summary>
#nullable disable
    public class MadaRefundResponse : PayTabsResponse
    {
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

        [JsonProperty("payment_result")]
        public PayTabsPaymentResult PaymentResult { get; set; }

        [JsonProperty("payment_info")]
        public PaymentInfo PaymentInfo { get; set; }
    }
}

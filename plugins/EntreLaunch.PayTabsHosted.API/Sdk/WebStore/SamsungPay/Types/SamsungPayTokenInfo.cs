using System.Text.Json.Serialization;

namespace EntreLaunch.PayTabsHosted.API.WebStore.SamsungPay.Types
{
    /// <summary>
    /// Represents Samsung Pay token details, such as the card brand and address associated with the payment.
    /// </summary>
#nullable disable
    public class SamsungPayTokenInfo
    {
        [JsonPropertyName("3DS")]
        public ThreeDSInfo _3DS { get; set; }

        [JsonPropertyName("payment_card_brand")]
        public string PaymentCardBrand { get; set; }

        [JsonPropertyName("payment_currency_type")]
        public string PaymentCurrencyType { get; set; }

        [JsonPropertyName("payment_eco_opt_in")]
        public string PaymentEcoOptIn { get; set; }

        [JsonPropertyName("payment_last4_dpan")]
        public string PaymentLast4Dpan { get; set; }

        [JsonPropertyName("payment_last4_fpan")]
        public string PaymentLast4Fpan { get; set; }

        [JsonPropertyName("merchant_ref")]
        public string MerchantRef { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("recurring_payment")]
        public bool RecurringPayment { get; set; }

        [JsonPropertyName("payment_shipping_address")]
        public PaymentShippingAddress PaymentShippingAddress { get; set; }

        [JsonPropertyName("payment_shipping_method")]
        public string PaymentShippingMethod { get; set; }
    }
}

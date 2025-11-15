using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.SamsungPay
{
    /// <summary>
    /// is a request to create a payment with Samsung Pay.
    /// </summary>
#nullable disable
    public class CreateSamsungPayRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public string CartAmount { get; set; }

        [JsonProperty("tokenise")]
        public int Tokenise { get; set; }

        [JsonProperty("return")]
        public string Return { get; set; }

        [JsonProperty("customer_details")]
        public string CustomerDetails { get; set; }

        [JsonProperty("samsung_pay_token")]
        public string SamsungPayToken { get; set; }
    }
}

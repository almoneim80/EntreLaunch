using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents a request to create a payment.
    /// </summary>
#nullable disable
    public class CreatePaymentBase : ApiRequestBody
    {
        /// <summary>
        /// Gets or sets the merchant Profile ID you can get from your PayTabs dashboard.
        /// </summary>
        [JsonRequired]
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonRequired]
        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [StringLength(128)]
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
    }
}

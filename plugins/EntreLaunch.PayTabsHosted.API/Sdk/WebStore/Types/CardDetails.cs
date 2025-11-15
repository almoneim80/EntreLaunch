using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents the card information (PAN, CVV, expiration date).
    /// </summary>
#nullable disable
    public class CardDetails
    {
        [JsonProperty("pan")]
        public string Pan { get; set; }

        [JsonProperty("cvv")]
        public string Cvv { get; set; }

        [JsonProperty("expiry_month")]
        public int ExpiryMonth { get; set; }

        [JsonProperty("expiry_year")]
        public int ExpiryYear { get; set; }
    }
}

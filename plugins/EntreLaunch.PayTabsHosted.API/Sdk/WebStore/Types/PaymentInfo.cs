using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents payment information (card type, payment description).
    /// </summary>
#nullable disable
    public class PaymentInfo
    {
        [JsonProperty("card_type")]
        public string CardType { get; set; }

        [JsonProperty("card_scheme")]
        public string CardScheme { get; set; }

        [JsonProperty("payment_description")]
        public string PaymentDescription { get; set; }
    }
}

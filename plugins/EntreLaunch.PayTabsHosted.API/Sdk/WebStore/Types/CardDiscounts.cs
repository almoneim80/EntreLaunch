using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents discounts associated with certain cards.
    /// </summary>
#nullable disable
    public class CardDiscounts
    {
        [JsonProperty("discount_cards")]
        public string DiscountCards { get; set; }

        [JsonProperty("discount_amount")]
        public decimal DiscountAmount { get; set; }

        [JsonProperty("discount_percent")]
        public string DiscountPercent { get; set; }

        [JsonProperty("discount_title")]
        public string DiscountTitle { get; set; }
    }
}

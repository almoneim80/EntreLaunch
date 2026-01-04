using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents the basket data (ID, currency, amount, description).
    /// </summary>
    public class CartData
    {
        public CartData(string cartId, string cartCurrency, decimal cartAmount, string cartDescription)
        {
            CartId = cartId;
            CartCurrency = cartCurrency;
            CartAmount = cartAmount;
            CartDescription = cartDescription;
        }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public decimal CartAmount { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }
    }
}

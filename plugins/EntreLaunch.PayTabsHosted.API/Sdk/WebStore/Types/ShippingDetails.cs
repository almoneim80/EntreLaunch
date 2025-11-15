using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents the shipping details (address, phone).
    /// </summary>
#nullable disable
    public class ShippingDetails
    {
        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("email")]
        public string Email { get; internal set; }

        [JsonProperty("phone")]
        public string PhoneNumber { get; internal set; }

        [JsonProperty("country")]
        public string Country { get; internal set; }

        [JsonProperty("state")]
        public string State { get; internal set; }

        [JsonProperty("city")]
        public string City { get; internal set; }

        [JsonProperty("street1")]
        public string Street1 { get; internal set; }

        [JsonProperty("zip")]
        public string Zip { get; internal set; }

        [JsonProperty("ip")]
        public string Ip { get; internal set; }
    }
}

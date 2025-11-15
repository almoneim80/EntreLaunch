using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents customer data (name, email, address).
    /// </summary>
#nullable disable
    public class CustomerDetails
    {
        public CustomerDetails()
        {
        }

        public CustomerDetails(string name, string email, string phoneNumber, string country, string state, string city, string street1, string zip)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Country = country;
            State = state;
            City = city;
            Street1 = street1;
            Zip = zip;
        }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("email")]
        public string Email { get; internal set; }

        [JsonProperty("phone")]
        public string PhoneNumber { get; internal set; }

        [JsonProperty("Country")]
        public string Country { get; internal set; }

        [JsonProperty("state")]
        public string State { get; internal set; }

        [JsonProperty("city")]
        public string City { get; internal set; }

        [JsonProperty("street1")]
        public string Street1 { get; internal set; }

        [JsonProperty("zip")]
        public string Zip { get; internal set; }
    }
}


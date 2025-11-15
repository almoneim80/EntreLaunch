using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents the service provider's data.
    /// </summary>
#nullable disable
    public class ProviderMetadata
    {
        /// <summary>
        /// Gets or sets payment service provider (PSP)-provided order identifier.
        /// </summary>
        [JsonProperty("providerReferenceId")]
        public string ProviderReferenceId { get; set; }
    }
}

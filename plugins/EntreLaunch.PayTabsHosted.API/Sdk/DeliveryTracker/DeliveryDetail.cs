using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.DeliveryTracker
{
    /// <summary>
    /// Represents the details of a single shipment, such as TrackingNumber and CarrierCode.
    /// </summary>
#nullable disable
    public class DeliveryDetail
    {
        /// <summary>
        /// Gets or sets the tracking number for the shipment provided by the shipping company.
        /// </summary>
        [JsonProperty("trackingNumber")]
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets the shipping company code used for delivering goods to the customer.
        /// </summary>
        [JsonProperty("carrierCode")]
        public string CarrierCode { get; set; }
    }
}

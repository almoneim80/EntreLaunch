using EntreLaunch.PayTabsHosted.API.Types;
using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.DeliveryTracker
{
    /// <summary>
    /// Represents the response to the trace request.
    /// </summary>
#nullable disable
    public class DeliveryTrackerResponse : PayTabsResponse
    {
        /// <summary>
        /// Gets the Amazon Order Reference ID associated with the order for which the shipments need to be tracked.
        /// </summary>
        [JsonProperty("amazonOrderReferenceId")]
        public string AmazonOrderReferenceId { get; internal set; }

        /// <summary>
        /// Gets the Charge Permission ID associated with the order for which the shipments need to be tracked.
        /// </summary>
        [JsonProperty("chargePermissionId")]
        public string ChargePermissionId { get; internal set; }

        /// <summary>
        /// Gets delivery details of the request.
        /// </summary>
        [JsonProperty("deliveryDetails")]
        public List<DeliveryDetail> DeliveryDetails { get; internal set; }
    }
}

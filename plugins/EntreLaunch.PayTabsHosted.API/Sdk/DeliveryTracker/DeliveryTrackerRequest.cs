using EntreLaunch.PayTabsHosted.API.Types;
using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.DeliveryTracker
{
    /// <summary>
    /// Used to send a shipment tracking request.
    /// </summary>
#nullable disable
    public class DeliveryTrackerRequest : ApiRequestBody
    {
        public DeliveryTrackerRequest(string objectId, bool objectIsChargePermission, string trackingNumber, string carrierCode)
        {
            if (objectIsChargePermission)
            {
                ChargePermissionId = objectId;
            }
            else
            {
                AmazonOrderReferenceId = objectId;
            }

            DeliveryDetails = new List<DeliveryDetail>();
            DeliveryDetails.Add(new DeliveryDetail()
            {
                TrackingNumber = trackingNumber,
                CarrierCode = carrierCode
            });
        }

        /// <summary>
        /// Gets or sets the Amazon Order Reference ID associated with the order for which the shipments need to be tracked.
        /// </summary>
        [JsonProperty("amazonOrderReferenceId")]
        public string AmazonOrderReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the Charge Permission ID associated with the order for which the shipments need to be tracked.
        /// </summary>
        [JsonProperty("chargePermissionId")]
        public string ChargePermissionId { get; set; }

        /// <summary>
        /// Gets delivery details of the request.
        /// </summary>
        [JsonProperty("deliveryDetails")]
        public List<DeliveryDetail> DeliveryDetails { get; internal set; }
    }
}

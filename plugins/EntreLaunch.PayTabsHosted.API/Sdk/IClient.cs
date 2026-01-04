using EntreLaunch.PayTabsHosted.API.AuthorizationToken;
using EntreLaunch.PayTabsHosted.API.DeliveryTracker;

namespace EntreLaunch.PayTabsHosted.API
{
    public interface IClient
    {
        /// <summary>
        /// Sends delivery tracking information that will trigger a Alexa Delivery Notification when the item is shipped or about to be delivered.
        /// </summary>
        /// <returns>DeliveryTrackerResponse response.</returns>
        DeliveryTrackerResponse SendDeliveryTrackingInformation(DeliveryTrackerRequest deliveryTrackersRequest, Dictionary<string, string>? headers = null);


        /// <summary>
        /// Retrieves a delegated authorization token used in order to make API calls on behalf of a merchant.
        /// </summary>
        AuthorizationTokenResponse GetAuthorizationToken(AuthorizationTokenRequest request, Dictionary<string, string>? headers = null);

        AuthorizationTokenResponse GetInvoiceAuthorizationToken(AuthorizationTokenRequest body, Dictionary<string, string>? headers = null);

        /// <summary>
        /// Retrieves a delegated authorization token used in order to make API calls on behalf of a merchant.
        /// </summary>
        /// <param name="tokenFormat">The MWS Auth Token that the solution provider currently uses to make V1 API calls on behalf of the merchant.</param>
        /// <param name="profileId">The PayTabs merchant ID.</param>
        /// <returns>HS256 encoded JWT Token that will be used to make V2 API calls on behalf of the merchant.</returns>
        //PaymentHostedPageResponse InitiatingPaymentHostedPage(string profileId, string tranType, string tranClass, string cartId, string tokenFormat, string cartCurrency, string cartAmount, string cartDescription);
    }
}

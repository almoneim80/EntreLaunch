using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// These are links to redirect the user during the checkout process.
    /// </summary>
#nullable disable
    public class WebCheckoutDetails
    {
        /// <summary>
        /// Gets or sets checkout review URL provided by the merchant. PayTabs will redirect to this URL after the buyer selects their preferred payment instrument and shipping address.
        /// </summary>
        [JsonProperty("checkoutReviewReturnUrl")]
        public string CheckoutReviewReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets checkout result URL provided by the merchant. PayTabs will redirect to this URL after completing the transaction.
        /// </summary>
        [JsonProperty("checkoutResultReturnUrl")]
        public string CheckoutResultReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets checkout cancel URL provided by the merchant. PayTabs will redirect to this URL when the checkout is cancelled on any of the PayTabs hosted sites.
        /// </summary>
        [JsonProperty("checkoutCancelUrl")]
        public string CheckoutCancelUrl { get; set; }

        /// <summary>
        /// Gets uRL provided by PayTabs. Merchant will redirect to this page after setting transaction details to complete checkout.
        /// </summary>
        [JsonProperty("amazonPayRedirectUrl")]
        public string PayTabsRedirectUrl { get; internal set; }

        /// <summary>
        /// Gets or sets specify whether the buyer will return to your website to review their order before completing checkout.
        /// </summary>
        [JsonProperty("checkoutMode")]
        public CheckoutMode? CheckoutMode { get; set; }
    }
}

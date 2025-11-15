using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Specify whether the buyer will return to your website to review their order before completing checkout.
    /// Selects the payment mode (e.g., direct order fulfillment).
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CheckoutMode
    {
        ProcessOrder
    }
}

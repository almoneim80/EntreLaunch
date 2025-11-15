using System.Text.Json.Serialization;

namespace EntreLaunch.PayTabsHosted.API.WebStore.SamsungPay.Types
{
    /// <summary>
    /// Stores Samsung Pay token information.
    /// </summary>
#nullable disable
    public class SamsungPayToken
    {
        [JsonPropertyName("samsung_pay_token")]
        public SamsungPayToken Token { get; set; }
    }
}

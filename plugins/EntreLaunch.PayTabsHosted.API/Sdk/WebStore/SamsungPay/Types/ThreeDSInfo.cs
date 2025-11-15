using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.SamsungPay.Types
{
    /// <summary>
    /// 3D Secure stores 3D information, such as version and type.
    /// </summary>
#nullable disable
    public class ThreeDSInfo
    {
        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}

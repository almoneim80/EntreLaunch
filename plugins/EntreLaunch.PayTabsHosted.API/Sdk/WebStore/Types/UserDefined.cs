using System.Text.Json.Serialization;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents custom fields.
    /// </summary>
#nullable disable
    public class UserDefined
    {
        [JsonPropertyName("test")]
        public string Test { get; set; }

        [JsonPropertyName("test2")]
        public string Test2 { get; set; }

        [JsonPropertyName("udf3")]
        public string Udf3 { get; set; }

        [JsonPropertyName("udf4")]
        public string Udf4 { get; set; }

        [JsonPropertyName("udf5")]
        public string Udf5 { get; set; }

        [JsonPropertyName("udf6")]
        public string Udf6 { get; set; }

        [JsonPropertyName("udf7")]
        public string Udf7 { get; set; }

        [JsonPropertyName("udf8")]
        public string Udf8 { get; set; }

        [JsonPropertyName("udf9")]
        public string Udf9 { get; set; }
    }
}

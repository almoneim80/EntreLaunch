using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.Types
{
#nullable disable
    public class TokenInfo
    {
        /// <summary>
        /// Gets hS256 encoded JWT Token.
        /// Stores token information such as token type and number of uses.
        /// </summary>
        [JsonProperty("tokenise")]
        public string Token { get; internal set; }

        [JsonProperty("tokenise")]
        public string Tokenise { get; internal set; }

        [JsonProperty("token_type")]
        public string TokenType { get; internal set; }

        [JsonProperty("counter")]
        public string Counter { get; internal set; }

        [JsonProperty("total_count")]
        public string TotalCount { get; internal set; }

        [JsonProperty("show_save_card")]
        public bool ShowSaveCard { get; internal set; }
    }
}

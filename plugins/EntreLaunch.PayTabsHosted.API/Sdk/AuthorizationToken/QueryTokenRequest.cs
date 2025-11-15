using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.AuthorizationToken
{
    /// <summary>
    /// Sends a request to query the status of an existing token.
    /// </summary>
#nullable disable
    public class QueryTokenRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}

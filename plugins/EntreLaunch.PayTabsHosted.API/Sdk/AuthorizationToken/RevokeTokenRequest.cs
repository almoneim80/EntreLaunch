using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.AuthorizationToken
{
    /// <summary>
    /// Sends a request to cancel or delete an existing token.
    /// </summary>
#nullable disable
    public class RevokeTokenRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}

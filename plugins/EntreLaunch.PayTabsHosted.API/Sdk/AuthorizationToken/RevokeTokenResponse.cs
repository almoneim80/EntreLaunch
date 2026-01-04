using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.AuthorizationToken
{
    /// <summary>
    /// Represents the response to the token cancel request, along with the status code and message.
    /// </summary>
#nullable disable
    public class RevokeTokenResponse : PayTabsResponse
    {
        [JsonProperty("code")]
        public string CodeResponse { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

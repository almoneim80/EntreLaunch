using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.QueryTransaction
{
    /// <summary>
    /// Represents a query request for a transaction using the CartId.
    /// </summary>
#nullable disable
    public class CreateQueryTransactionByCartIdRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }
    }
}

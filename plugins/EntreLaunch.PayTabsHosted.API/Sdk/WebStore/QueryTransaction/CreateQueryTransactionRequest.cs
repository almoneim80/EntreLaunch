using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.QueryTransaction
{
    /// <summary>
    /// Represents a request to query a transaction using the TransactionReference.
    /// </summary>
#nullable disable
    public class CreateQueryTransactionRequest : ApiRequestBody
    {
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }
    }
}

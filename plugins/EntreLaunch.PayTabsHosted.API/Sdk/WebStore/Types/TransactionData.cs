using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Represents the type and categorization of the operation.
    /// </summary>
#nullable disable
    public class TransactionData
    {
        public TransactionData(string transactionType, string transactionClass)
        {
            TransactionType = transactionType;
            TransactionClass = transactionClass;
        }

        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }
    }
}

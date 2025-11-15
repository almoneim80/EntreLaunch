using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
    /// <summary>
    /// Ledger Currencies available for PayTabs.
    /// Identifies sEntreLaunchported currencies in the books of accounts.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LedgerCurrency
    {
        /// <summary>
        /// British Pound
        /// </summary>
        GBP,

        /// <summary>
        /// Euro
        /// </summary>
        EUR,

        /// <summary>
        /// Japanese Yen
        /// </summary>
        JPY,

        /// <summary>
        /// United States Dollar
        /// </summary>
        USD
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EntreLaunch.PayTabsHosted.API.Types
{
    /// <summary>
    /// Currencies available for PayTabs.
    /// Specifies sEntreLaunchported currencies such as AED and USD.
    /// </summary>
    /// <remarks>
    /// Specifying a currenty other than the ledger currenc is only sEntreLaunchported in our region.
    /// </remarks>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Currency
    {
        /// <summary>
        /// United Arab Emirates Dirham
        /// </summary>
        AED,

        /// <summary>
        /// Saudi Riyal
        /// </summary>
        SAR,

        /// <summary>
        /// Egyptian Pound
        /// </summary>
        EGP,

        /// <summary>
        /// Omani Rial
        /// </summary>
        OMR,

        /// <summary>
        /// 
        /// </summary>
        PKR,

        /// <summary>
        /// 
        /// </summary>
        QAR,

        /// <summary>
        /// Jordanian Dinar
        /// </summary>
        JOD,

        /// <summary>
        /// Iraqi Dinar 
        /// </summary>
        IQD,

        /// <summary>
        /// IDR
        /// </summary>
        IDR,

        /// <summary>
        /// INR
        /// </summary>
        INR,

        /// <summary>
        /// Kuwaiti Dinar
        /// </summary>
        KWD,

        /// <summary>
        /// MAD
        /// </summary>
        MAD,

        /// <summary>
        /// British Pound
        /// </summary>
        GBP,

        /// <summary>
        /// Euro
        /// </summary>
        EUR,

        /// <summary>
        /// Hong Kong Dollar
        /// </summary>
        HKD,

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

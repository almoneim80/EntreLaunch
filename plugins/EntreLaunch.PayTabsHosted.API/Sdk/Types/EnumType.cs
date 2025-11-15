using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace EntreLaunch.PayTabsHosted.API.Types
{
    /// <summary>
    /// Defines transaction types such as Sale and Refund.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionType
    {
        Sale = 1,
        Auth = 2,
        Capture = 3,
        Register = 4,
        Refund = 5
    }

    /// <summary>
    /// Determines the categorization of transactions such as Ecom or Recurring.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionClass
    {
        Ecom = 1,
        Recurring = 2,
        Moto = 3
    }

    /// <summary>
    /// Determines the payment methods sEntreLaunchported by PayTabs.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentMethods
    {
        All = 1,
        Mada = 2,
        AmricanExpress = 3,
        Meastro = 4,
        MasterCard = 5,
        Visa = 6,
        VisaElectron = 7,
        ApplePay = 8,
        StcPay = 9,
        UrPay = 10,
        Sadad = 11,
        Tabby = 12,
        Tamara = 13,
        OmanNet = 14,
        UnionPay = 15,
        SamsungPay = 16,
        Meeza = 17,
        PayPal = 18,
        Forsa = 19,
        Knet = 30,
    }

    /// <summary>
    /// Specifies the behavior of saving data in the database.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SaveDataType
    {
        None = 0,
        OnlyIfEmpty,
        Always
    }

    /// <summary>
    /// Specifies the status of a transaction response.
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary>
        /// Authorized
        /// </summary>
        A = 1,

        /// <summary>
        /// Hold (Authorised but on hold for further anti-fraud review)
        /// </summary>
        H = 2,

        /// <summary>
        /// Pending (for refunds)
        /// </summary>
        P = 3,

        /// <summary>
        /// Voided
        /// </summary>
        V = 4,

        /// <summary>
        /// Error
        /// </summary>
        E = 5,

        /// <summary>
        /// Declined
        /// </summary>
        D = 6,

        /// <summary>
        /// Expired
        /// </summary>
        X = 7,
    }
}

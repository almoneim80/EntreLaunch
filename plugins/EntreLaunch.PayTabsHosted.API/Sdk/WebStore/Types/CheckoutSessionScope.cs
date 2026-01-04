using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types
{
#nullable disable
    /// <summary>
    /// The buyer details that you're requesting access for.
    /// Specifies the data required from the client during the session (e.g., name, email address).
    /// </summary>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum CheckoutSessionScope
    {
        /// <summary>
        /// Request access to buyer name.
        /// </summary>
        [EnumMember(Value = "name")]
        Name,

        /// <summary>
        /// Request access to buyer email address.
        /// </summary>
        [EnumMember(Value = "email")]
        Email,

        /// <summary>
        /// Request access to buyer default shipping address postal code and Country code.
        /// </summary>
        [EnumMember(Value = "postalCode")]
        PostalCode,

        /// <summary>
        /// Request access to buyer default full shipping address (including postal code and Country code).
        /// </summary>
        [EnumMember(Value = "shippingAddress")]
        ShippingAddress,

        /// <summary>
        /// Request access to buyer default phone number.
        /// </summary>
        [EnumMember(Value = "phoneNumber")]
        PhoneNumber,

        /// <summary>
        /// Request access to buyer prime membership status.
        /// </summary>
        /// <remarks>
        /// Note that Passing "primeStatus" will return value for field "primeMembershipTypes" in response, only if the customer is eligible for prime membership
        /// </remarks>
        [EnumMember(Value = "primeStatus")]
        PrimeStatus,

        /// <summary>
        /// Request access to buyer default full billing address (including postal code and Country code).
        /// </summary>
        [EnumMember(Value = "billingAddress")]
        BillingAddress
    }
}

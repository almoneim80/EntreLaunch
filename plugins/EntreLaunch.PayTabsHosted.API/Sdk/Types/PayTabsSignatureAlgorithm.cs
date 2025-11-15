using EntreLaunch.PayTabsHosted.API.Attributes;

namespace EntreLaunch.PayTabsHosted.API.Types
{
    /// <summary>
    /// Amazon Signature Algorithms used for signature generation.
    /// Specifies sEntreLaunchported signature algorithms such as Default.
    /// </summary>
    public enum PayTabsSignatureAlgorithm
    {
        /// <summary>
        /// Default Algorithm with salt length 20
        /// </summary>
        [PayTabsSignatureAlgorithm(name: "sha256", saltLength: 256)]
        Default,
    }
}

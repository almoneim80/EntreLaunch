using EntreLaunch.PayTabsHosted.API.Attributes;

namespace EntreLaunch.PayTabsHosted.API.Types
{
    /// <summary>
    /// Payment regions that paytabs is available at.
    /// Selects sEntreLaunchported regions such as SaudiArabia and Egypt.
    /// </summary>
    public enum Region
    {
        /// <summary>
        /// global and all countries in the sEntreLaunchort region.
        /// </summary>
        [Region(shortform: "GLOBAL", domain: "com")]
        Global,

        /// <summary>
        /// United Emirates (ARE)
        /// </summary>
        [Region(shortform: "ARE", domain: "secure.paytabs.com")]
        UnitedEmirates,

        /// <summary>
        /// Saudi Arabia (SAU)
        /// </summary>
        [Region(shortform: "SAU", domain: "secure.paytabs.sa")]
        SaudiArabia,

        /// <summary>
        /// Egypt (EGY)
        /// </summary>
        [Region(shortform: "EGY", domain: "secure-egypt.paytabs.com")]
        Egypt,

        /// <summary>
        /// Oman (OMN)
        /// </summary>
        [Region(shortform: "OMN", domain: "secure-oman.paytabs.com")]
        Oman,

        /// <summary>
        /// jordan (JOR)
        /// </summary>
        [Region(shortform: "JOR", domain: "secure-jordan.paytabs.com")]
        Jordan,

        /// <summary>
        /// iraq (IRQ)
        /// </summary>
        [Region(shortform: "IRQ", domain: "secure-iraq.paytabs.com")]
        Iraq
    }
}

namespace EntreLaunch.PayTabsHosted.API.Types
{
#nullable disable
    /// <summary>
    /// The PayTabs environment (live or sandbox).
    /// Specifies the operating environment (Production or Sandbox).
    /// </summary>
    /// <remarks>
    /// Sandbox mode enables you to conduct an end-to-end test of
    /// integration prior to going live.
    /// </remarks>
    public enum Environment
    {
        Live,
        Sandbox,
    }
}

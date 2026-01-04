namespace EntreLaunch.PayTabsHosted.API.WebStore.SamsungPay.Types
{
    /// <summary>
    /// Stores Samsung Pay payment data, such as signature and version.
    /// </summary>
#nullable disable
    public class SamsungPayPaymentData
    {
        public string Data { get; set; }

        public string Signature { get; set; }

        public string Header { get; set; }

        public string Version { get; set; }
    }
}

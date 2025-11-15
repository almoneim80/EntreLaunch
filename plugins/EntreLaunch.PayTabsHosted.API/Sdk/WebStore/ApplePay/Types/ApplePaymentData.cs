namespace EntreLaunch.PayTabsHosted.API.WebStore.ApplePay.Types
{
#nullable disable
    internal class ApplePaymentData
    {
        public string Data { get; set; }

        public string Signature { get; set; }

        public string Header { get; set; }

        public string Version { get; set; }
    }
}

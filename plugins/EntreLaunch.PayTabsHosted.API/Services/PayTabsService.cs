using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore;
using EntreLaunch.PayTabsHosted.API.WebStore.HostedPage;

namespace EntreLaunch.PayTabsHosted.API.Services
{
    public class PayTabsService
    {
        private readonly WebStoreClient _client;

        public PayTabsService()
        {
            var config = new ApiConfiguration(
                profileId: 12345,          // Profile ID من لوحة تحكم PayTabs
                serverKey: "your_server_key",
                clientKey: "your_client_key",
                region: Region.SaudiArabia,      // اختر المنطقة المناسبة
                currency: Currency.SAR,
                algorithm: PayTabsSignatureAlgorithm.Default);

            _client = new WebStoreClient(config);
        }

        public HostedPaymentPageResponse CreatePayment(decimal amount, string orderId)
        {
            var request = new InitiatingHostedPaymentPageRequest(
                profileId: 12345,
                transactionType: "sale",
                transactionClass: "ecom",
                cartId: orderId,
                cartCurrency: "KSA",
                cartAmount: amount,
                cartDescription: "Order Payment");

            return _client.InitiatingHostedPaymentPage(request);
        }
    }
}

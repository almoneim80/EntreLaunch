using Newtonsoft.Json;
namespace EntreLaunch.Services.PaymentSvc
{
    public class PaytabsPaymentGateway : IPaymentGateway
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaytabsPaymentGateway> _logger;
        private readonly PayTabsOptions _options;
        private readonly HttpClient _httpClient;

        public PaytabsPaymentGateway(
            IConfiguration configuration,
            ILogger<PaytabsPaymentGateway> logger,
            HttpClient httpClient,
            IOptions<PayTabsOptions> options)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
            _options = options.Value;
        }

        /// <inheritdoc />
        public async Task<PaymentResult> InitiatePaymentAsync(Payment payment, string paymentToken)
        {
            try
            {
                // (1) Setting EntreLaunch the order to PayTabs
                var paytabsRequest = new
                {
                    profile_id = _options.ProfileId,
                    tran_type = "sale",
                    tran_class = "ecom",
                    cart_id = payment.Id.ToString(),
                    cart_currency = _options.DefaultCurrency ?? "SAR",
                    cart_amount = payment.NetAmount,
                    cart_description = payment.PaymentPurpose,
                    payment_token = paymentToken,
                    customer_details = new
                    {
                        name = $"{payment.User.FirstName} {payment.User.LastName}",
                        email = payment.User.Email,
                        phone = payment.User.PhoneNumber,
                        street1 = "Street Address",
                        city = "Riyadh",
                        state = "Riyadh",
                        country = payment.User.CountryCode.ToString(),
                        zip = "12345",
                        ip = "192.168.1.1"
                    },
                    callback = _options.CallbackUrl,
                    returnUrl = _options.ReturnUrl,
                };

                var jsonBody = JsonConvert.SerializeObject(paytabsRequest);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // (2) Preparing guidance and endpoints
                var endpoint = $"{_options.BaseUrl}/payment/request";
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ServerKey}");

                // (3) Sending the request
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                // (4) Analyzing the response
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to initiate payment. Response: {Response}", responseString);
                    return new PaymentResult
                    {
                        IsSuccess = false,
                        PaymentStatus = "Failed",
                        ErrorMessage = $"Error: {response.StatusCode} - {responseString}"
                    };
                }

                // (5) Parsing the response
                var paytabsResponse = JsonConvert.DeserializeObject<PayTabsInitiateResponse>(responseString);

                if (paytabsResponse == null || !paytabsResponse.Success)
                {
                    _logger.LogError("Invalid response from PayTabs: {Response}", responseString);
                    return new PaymentResult
                    {
                        IsSuccess = false,
                        PaymentStatus = "Failed",
                        ErrorMessage = "Invalid PayTabs response."
                    };
                }

                // (6) Returning the result
                return new PaymentResult
                {
                    IsSuccess = true,
                    PaymentStatus = "PendingConfirmation",
                    TransactionId = paytabsResponse.TranRef,
                    RedirectUrl = paytabsResponse.RedirectUrl,
                    PaidAmount = payment.NetAmount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while initiating payment.");
                return new PaymentResult
                {
                    IsSuccess = false,
                    PaymentStatus = "Failed",
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}

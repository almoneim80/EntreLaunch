using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Capture;
using EntreLaunch.PayTabsHosted.API.WebStore.CheckoutSession;
using EntreLaunch.PayTabsHosted.API.WebStore.HostedPage;
using EntreLaunch.PayTabsHosted.API.WebStore.Interfaces;
using EntreLaunch.PayTabsHosted.API.WebStore.QueryTransaction;
using EntreLaunch.PayTabsHosted.API.WebStore.Refund;
using EntreLaunch.PayTabsHosted.API.WebStore.Void;

namespace EntreLaunch.PayTabsHosted.API.WebStore
{
    public class WebStoreClient : Client, IWebStoreClient
    {
        public WebStoreClient(ApiConfiguration payConfiguration)
            : base(payConfiguration)
        {
        }

        #region Hosted Payment Page

        /// <summary>
        /// Creates a new Checkout Session.
        /// </summary>
        public HostedPaymentPageResponse InitiatingHostedPaymentPage(InitiatingHostedPaymentPageRequest body, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Default);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, body, headers);

            var result = CallAPI<HostedPaymentPageResponse>(apiRequest);

            return result;
        }

        public HostedPaymentPageResponse CreateHostedPaymentPage(CreateHostedPaymentRequest body, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Default);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, body, headers);

            var result = CallAPI<HostedPaymentPageResponse>(apiRequest);

            return result;
        }

        public HostedPaymentPageResponse EntreLaunchdateHostedPaymentPage(EntreLaunchdateHostedPaymentPageRequest body, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Default);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, body, headers);

            var result = CallAPI<HostedPaymentPageResponse>(apiRequest);

            return result;
        }

        public HostedPaymentPageResponse CompleteHostedPaymentPage(CompleteHostedPaymentPageRequest body, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Default);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, body, headers);

            var result = CallAPI<HostedPaymentPageResponse>(apiRequest);

            return result;
        }

        public HostedPaymentPageResponse FinalizeHostedPaymentPage(FinalizeHostedPaymentPageRequest body, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Default);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, body, headers);

            var result = CallAPI<HostedPaymentPageResponse>(apiRequest);

            return result;
        }

        #endregion


        #region Public Checkout 

        /// <summary>
        /// Creates a new Checkout Session.
        /// </summary>
        public CheckoutSessionResponse CreateCheckoutSession(CreateCheckoutSessionRequest body, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.CheckoutSessions);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, body, headers);

            var result = CallAPI<CheckoutSessionResponse>(apiRequest);

            return result;
        }

        /// <summary>
        /// Gets a Checkout Session.
        /// </summary>
        public CheckoutSessionResponse GetCheckoutSession(GetCheckoutSessionRequest request, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.CheckoutSessions);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.GET, request, headers);

            var result = CallAPI<CheckoutSessionResponse>(apiRequest);

            return result;
        }

        /// <summary>
        /// EntreLaunchdates a Checkout Session.
        /// </summary>
        public CheckoutSessionResponse EntreLaunchdateCheckoutSession(EntreLaunchdateCheckoutSessionRequest body, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.CheckoutSessions);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.PATCH, body, headers);

            var result = CallAPI<CheckoutSessionResponse>(apiRequest);

            return result;
        }

        /// <summary>
        /// Completes a Checkout Session.
        /// </summary>
        public CheckoutSessionResponse CompleteCheckoutSession(CompleteCheckoutSessionRequest completeRequest, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.CheckoutSessions, "", Constants.Methods.Complete);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, completeRequest, headers);

            var result = CallAPI<CheckoutSessionResponse>(apiRequest);

            return result;
        }

        /// <summary>
        /// FinalizeCheckoutSession API which enables Pay to validate payment critical attributes and also EntreLaunchdate book-keeping attributes present in merchantMetadata.
        /// </summary>
        public CheckoutSessionResponse FinalizeCheckoutSession(FinalizeCheckoutSessionRequest finalizeCheckoutSessionRequest, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.CheckoutSessions, "", Constants.Methods.Finalize);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, finalizeCheckoutSessionRequest, headers);

            var result = CallAPI<CheckoutSessionResponse>(apiRequest);

            return result;
        }
        #endregion


        #region Manager Transaction

        public QueryTransactionByCartIdResponse CreateTransactionByCartId(CreateQueryTransactionByCartIdRequest body, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Default);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, body, headers);

            var result = CallAPI<QueryTransactionByCartIdResponse>(apiRequest);

            return result;
        }

        public QueryTransactionResponse CreateTransactionByReference(CreateQueryTransactionRequest body, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Default);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, body, headers);

            var result = CallAPI<QueryTransactionResponse>(apiRequest);

            return result;
        }

        /// <summary>
        /// Initiate a full or partial refund for a charge.
        /// </summary>
        public RefundResponse CreateRefund(CreateRefundRequest refundRequest, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Refunds);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, refundRequest, headers);

            var result = CallAPI<RefundResponse>(apiRequest);

            return result;
        }

        /// <summary>
        /// Get refund details.
        /// </summary>
        public RefundResponse GetRefund(GetRefundRequest refundRequest, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Refunds);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, refundRequest, headers);

            var result = CallAPI<RefundResponse>(apiRequest);

            return result;
        }


        /// <summary>
        /// Initiate a full or partial refund for a charge.
        /// </summary>
        public CapturePaymentResponse CapturePayment(CreateCapturePaymentRequest captureRequest, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Refunds);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, captureRequest, headers);

            var result = CallAPI<CapturePaymentResponse>(apiRequest);

            return result;
        }

        public VoidResponse CreateVoid(CreateVoidRequest voidRequest, Dictionary<string, string>? headers = null)
        {
            var apiPath = apiUrlBuilder.BuildFullApiPath(Constants.ApiServices.Default, Constants.Resources.WebStore.Refunds);
            var apiRequest = new ApiRequest(apiPath, PayHttpMethod.POST, voidRequest, headers);

            var result = CallAPI<VoidResponse>(apiRequest);

            return result;
        }
        #endregion

        /// <summary>
        /// Generates the signature string for the PayTabs front-end button.
        /// </summary>
        /// <param name="jsonString">The payload for generating a CheckoutSession as JSON string.</param>
        /// <returns>Signature string that can be assigned to the front-end button's "signature" parameter.</returns>
        public string GenerateButtonSignature(string jsonString)
        {
            var stringToSign = SignatureHelper.CreateStringToSign(jsonString);
            var signature = SignatureHelper.GenerateSignature(stringToSign);

            return signature;
        }
    }
}

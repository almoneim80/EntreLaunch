using System.Collections.Generic;
using EntreLaunch.PayTabsHosted.API.WebStore.Capture;
using EntreLaunch.PayTabsHosted.API.WebStore.CheckoutSession;
using EntreLaunch.PayTabsHosted.API.WebStore.HostedPage;
using EntreLaunch.PayTabsHosted.API.WebStore.QueryTransaction;
using EntreLaunch.PayTabsHosted.API.WebStore.Refund;
using EntreLaunch.PayTabsHosted.API.WebStore.Void;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Interfaces
{
#nullable disable
    public interface IWebStoreClient : IClient
    {
        #region Hosted Payment Page

        /// <summary>
        /// Initiating the payment request.
        /// </summary>
        HostedPaymentPageResponse InitiatingHostedPaymentPage(InitiatingHostedPaymentPageRequest body, Dictionary<string, string> headers = null);

        /// <summary>
        /// Creates a new hosted payment page.
        /// </summary>
        HostedPaymentPageResponse CreateHostedPaymentPage(CreateHostedPaymentRequest body, Dictionary<string, string> headers = null);

        /// <summary>
        /// EntreLaunchdates a hosted payment page.
        /// </summary>
        HostedPaymentPageResponse EntreLaunchdateHostedPaymentPage(EntreLaunchdateHostedPaymentPageRequest body, Dictionary<string, string> headers = null);

        /// <summary>
        /// Completes a hosted payment page.
        /// </summary>
        HostedPaymentPageResponse CompleteHostedPaymentPage(CompleteHostedPaymentPageRequest body, Dictionary<string, string> headers = null);

        /// <summary>
        /// FinalizeHostedPaymentPage API which enables Pay to validate payment critical attributes and also EntreLaunchdate book-keeping attributes present in merchantMetadata.
        /// </summary>
        HostedPaymentPageResponse FinalizeHostedPaymentPage(FinalizeHostedPaymentPageRequest body, Dictionary<string, string> headers = null);

        #endregion

        #region Public Checkout 
        
        /// <summary>
        /// Creates a new Checkout Session.
        /// </summary>
        CheckoutSessionResponse CreateCheckoutSession(CreateCheckoutSessionRequest body, Dictionary<string, string> headers = null);

        /// <summary>
        /// Gets a Checkout Session.
        /// </summary>
        CheckoutSessionResponse GetCheckoutSession(GetCheckoutSessionRequest request, Dictionary<string, string> headers = null);

        /// <summary>
        /// EntreLaunchdates a Checkout Session.
        /// </summary>
        CheckoutSessionResponse EntreLaunchdateCheckoutSession(EntreLaunchdateCheckoutSessionRequest body, Dictionary<string, string> headers = null);

        /// <summary>
        /// Completes a Checkout Session.
        /// </summary>
        CheckoutSessionResponse CompleteCheckoutSession(CompleteCheckoutSessionRequest completeRequest, Dictionary<string, string> headers = null);

        /// <summary>
        /// FinalizeCheckoutSession API which enables Pay to validate payment critical attributes and also EntreLaunchdate book-keeping attributes present in merchantMetadata.
        /// </summary>
        CheckoutSessionResponse FinalizeCheckoutSession(FinalizeCheckoutSessionRequest finalizeCheckoutSessionRequest, Dictionary<string, string> headers = null);

        #endregion

        #region Manager Transaction

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        QueryTransactionByCartIdResponse CreateTransactionByCartId(CreateQueryTransactionByCartIdRequest body, Dictionary<string, string> headers = null);

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        QueryTransactionResponse CreateTransactionByReference(CreateQueryTransactionRequest body, Dictionary<string, string> headers = null);

        /// <summary>
        /// Initiate a full or partial refund for a charge.
        /// </summary>
        RefundResponse CreateRefund(CreateRefundRequest refundRequest, Dictionary<string, string> headers = null);

        /// <summary>
        /// Get refund details.
        /// </summary>
        RefundResponse GetRefund(GetRefundRequest refundRequest, Dictionary<string, string> headers = null);

        /// <summary>
        /// Initiate a full or partial refund for a charge.
        /// </summary>
        CapturePaymentResponse CapturePayment(CreateCapturePaymentRequest captureRequest, Dictionary<string, string> headers = null);

        /// <summary>
        /// Initiate a full or partial refund for a charge.
        /// </summary>
        VoidResponse CreateVoid(CreateVoidRequest voidRequest, Dictionary<string, string> headers = null);

        #endregion


        /// <summary>
        /// Generates the signature string for the PayTabs front-end button.
        /// </summary>
        /// <param name="jsonString">The payload for generating a CheckoutSession as JSON string.</param>
        /// <returns>Signature string that can be assigned to the front-end button's "signature" parameter.</returns>
        string GenerateButtonSignature(string jsonString);
    }
}

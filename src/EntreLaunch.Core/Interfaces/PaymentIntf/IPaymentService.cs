namespace EntreLaunch.Interfaces.PaymentIntf;
public interface IPaymentService
{
    /// <summary>
    /// Create Payment.
    /// </summary>
    Task<PaymentDetailsDto> CreatePaymentAsync(PaymentCreateDto dto);

    /// <summary>
    /// Initiate Payment.
    /// </summary>
    Task<PaymentResult> InitiatePaymentAsync(int paymentId, string paymentToken);

    /// <summary>
    /// Process Payment Callback.
    /// </summary>
    Task<bool> ProcessCallbackAsync(Dictionary<string, string> callbackData);

    /// <summary>
    /// Process IPN.
    /// </summary>
    Task<bool> ProcessIPNAsync(Dictionary<string, string> ipnData);

    /// <summary>
    /// Get Payment By Id.
    /// </summary>
    Task<PaymentDetailsDto> GetPaymentByIdAsync(int paymentId);

   /// <summary>
   /// Check if payment is paid.
   /// </summary>
    Task<bool> IsPaid(int targetId, string userId);

    /// <summary>
    /// Cancel Payment afert refund Approved.
    /// </summary>
    Task<bool> CancelPayment(int paymentId);
}

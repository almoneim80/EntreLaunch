namespace EntreLaunch.Interfaces.PaymentIntf;
public interface IPaymentGateway
{
    /// <summary>
    /// Start the payment process with the payment gateway Get a link or Token.
    /// </summary>
    Task<PaymentResult> InitiatePaymentAsync(Payment payment, string paymentToken);
}

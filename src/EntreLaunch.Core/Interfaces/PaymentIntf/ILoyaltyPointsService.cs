namespace EntreLaunch.Interfaces.PaymentIntf;
public interface ILoyaltyPointsService
{
    /// <summary>
    /// Calculate and add points to the user based on a specific payment.
    /// </summary>
    Task<LoyaltyPointsResult> AddPointsForPaymentAsync(string userId, int paymentId);

    /// <summary>
    /// deduct points from the user (if used for payment or any other reason).
    /// </summary>
    Task<LoyaltyPointsResult> DeductPointsAsync(string userId, int points);

    /// <summary>
    /// Retrieve the user's current balance of points.
    /// </summary>
    Task<int> GetUserPointsAsync(string userId);

    /// <summary>
    /// Record a points transaction - can be called internally or publicly to record a change in the points balance.
    /// </summary>
    Task RecordPointsTransactionAsync(string userId, int pointsChanged, string reason, int? paymentId = null);

    /// <summary>
    /// Redeeming a certain number of points in a payment (deducting part of the amount against these points).
    /// </summary>
    Task<PaymentDetailsDto> RedeemPointsForPaymentAsync(string userId, int paymentId, int pointsToUse);

    /// <summary>
    /// Add points as a reward to the user.
    /// </summary>
    Task<LoyaltyPointsResult> AddBonusPointsAsync(string userId, int points, string reason);
}

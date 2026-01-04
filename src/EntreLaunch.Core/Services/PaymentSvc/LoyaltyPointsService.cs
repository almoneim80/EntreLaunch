using EntreLaunch.DTOs.PaymentDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.PaymentSvc
{
    public class LoyaltyPointsService(
        PgDbContext dbContext,
        ILogger<LoyaltyPointsService> logger,
        ILocalizationManager localizationManager,
        IHttpContextHelper httpContextHelper) : ILoyaltyPointsService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<LoyaltyPointsService> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult<LoyaltyPointsResult>> AddPointsForPaymentAsync(string userId, int paymentId)
        {
            try
            {
                if (paymentId <= 0)
                {
                    _logger.LogWarning("Invalid payment ID: {PaymentId} is not a positive integer.", paymentId);
                    return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("InvalidId"), null, ErrorType.InvalidData);
                }

                var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId && !p.IsDeleted);
                if (payment == null)
                {
                    _logger.LogError($"Payment {paymentId} for user {userId} not found or invalid.");
                    return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("PaymentNotFound"), null);
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    _logger.LogError($"User with ID {userId} not found or deleted.");
                    return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("UserNotFound"), null);
                }

                if (!payment.Status!.Equals("Paid"))
                {
                    _logger.LogWarning("Cannot add points for unpaid payment {PaymentId}", paymentId);
                    return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("PointsOnlyForPaidPayments"), null);
                }

                int pointsToAdd = CalculatePoints(payment.NetAmount ?? 0);
                await RecordPointsTransactionAsync(userId, pointsToAdd, "Points added for payment", paymentId);
                _logger.LogInformation("Added {Points} points to user {UserId} for payment {PaymentId}", pointsToAdd, userId, paymentId);
                return new GeneralResult<LoyaltyPointsResult>(true, _localizationManager.GetLocalizedString("PointsAddedSuccessfully"), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding points for payment {PaymentId}", paymentId);
                return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("ErrorAddingPoints"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<LoyaltyPointsResult>> AddBonusPointsAsync(string userId, int points, string reason)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID is required.");
                    return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("InvalidUserId"), null);
                }

                if (points <= 0)
                {
                    _logger.LogError("Points must be greater than zero.");
                    return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("PointsDeductMustBePositive"), null);
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    _logger.LogError($"User with ID {userId} not found or deleted.");
                    return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("UserNotFound"), null);
                }

                // add bonus points in the database
                var loyaltyPoint = new LoyaltyPoint
                {
                    UserId = userId,
                    PointsChanged = points,
                    Reason = reason ?? "Bonus Points",
                    IsDeleted = false,
                    PaymentId = null,
                    CreatedAt = DateTimeOffset.UtcNow,
                    ByUserAgent = httpContextHelper.UserAgent,
                    ByIp = httpContextHelper.IpAddress,
                    ById = user.Id,
                };

                _dbContext.LoyaltyPoints.Add(loyaltyPoint);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Bonus points ({points}) added for user {userId}.");
                return new GeneralResult<LoyaltyPointsResult>(true, _localizationManager.GetLocalizedString("PointsAddedSuccessfully"), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding bonus points for user {UserId}", userId);
                return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("ErrorAddingPoints"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<LoyaltyPointsResult>> DeductPointsAsync(string userId, int points)
        {
            try
            {
                if (points <= 0)
                {
                    _logger.LogWarning("Invalid points to deduct: {Points}", points);
                    return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("PointsDeductMustBePositive"), null);
                }

                int currentPoints = await GetUserPointsAsync(userId);
                if (currentPoints < points)
                {
                    _logger.LogWarning("Insufficient points for user {UserId}. Available: {Available}, Required: {Required}", userId, currentPoints, points);
                    return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("InsufficientPoints"), null);
                }

                await RecordPointsTransactionAsync(userId, -points, "Points deducted.");
                _logger.LogInformation("Deducted {Points} points from user {UserId}", points, userId);
                return new GeneralResult<LoyaltyPointsResult>(true, _localizationManager.GetLocalizedString("PointsDeductedSuccessfully"), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deducting points for user {UserId}", userId);
                return new GeneralResult<LoyaltyPointsResult>(false, _localizationManager.GetLocalizedString("ErrorDeductingPoints"), null);
            }
        }

        /// <inheritdoc />
        public async Task<int> GetUserPointsAsync(string userId)
        {
            var points = await _dbContext.LoyaltyPoints.Where(lp => lp.UserId == userId && !lp.IsDeleted).SumAsync(lp => lp.PointsChanged);
            if (points < 0) points = 0;

            return points;
        }

        /// <inheritdoc />
        public async Task RecordPointsTransactionAsync(string userId, int pointsChanged, string reason, int? paymentId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    _logger.LogWarning("Transaction reason is required.");
                    throw new ArgumentException(_localizationManager.GetLocalizedString("TransactionReasonRequired"));
                }

                var transaction = new LoyaltyPoint
                {
                    UserId = userId,
                    PointsChanged = pointsChanged,
                    Reason = reason,
                    PaymentId = paymentId ?? null,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false,
                    ByUserAgent = httpContextHelper.UserAgent,
                    ByIp = httpContextHelper.IpAddress,
                    ById = userId,
                };

                _dbContext.LoyaltyPoints.Add(transaction);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Recorded points transaction for user {UserId}: {PointsChanged} points ({Reason})", userId, pointsChanged, reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording points transaction for user {UserId}", userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaymentDetailsDto>> RedeemPointsForPaymentAsync(string userId, int paymentId, int pointsToUse)
        {
            try
            {
                if (pointsToUse <= 0)
                {
                    _logger.LogWarning("Invalid points to redeem: {Points}", pointsToUse);
                    return new GeneralResult<PaymentDetailsDto>(false, _localizationManager.GetLocalizedString("PointsToRedeemMustBeGreaterThanZero"), null);
                }

                var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId && !p.IsDeleted);

                if (payment == null)
                {
                    _logger.LogError($"Payment {paymentId} for user {userId} not found or invalid.");
                    return new GeneralResult<PaymentDetailsDto>(false, _localizationManager.GetLocalizedString("PaymentNotFound"), null);
                }

                if (payment.Status != "Pending")
                {
                    _logger.LogWarning("Cannot redeem points for payment {PaymentId} with status {Status}", paymentId, payment.Status);
                    return new GeneralResult<PaymentDetailsDto>(false, _localizationManager.GetLocalizedString("RedeemOnlyForPendingPayments"), null);
                }

                int currentPoints = await GetUserPointsAsync(userId);

                if (currentPoints < pointsToUse)
                {
                    _logger.LogWarning("Insufficient points for user {UserId}. Available: {Available}, Required: {Required}", userId, currentPoints, pointsToUse);
                    return new GeneralResult<PaymentDetailsDto>(false, _localizationManager.GetLocalizedString("InsufficientPointsToRedeem"), null);
                }

                // Calculate the monetary value of points (e.g., 1 point = 0.1 SAR)
                decimal valueOfPoints = pointsToUse * 0.1m;

                if (valueOfPoints > payment.NetAmount)
                {
                    _logger.LogWarning("Cannot redeem points exceeding payment amount. Points value: {Value}, Payment amount: {Amount}", valueOfPoints, payment.NetAmount);
                    return new GeneralResult<PaymentDetailsDto>(false, _localizationManager.GetLocalizedString("RedeemExceedsPaymentAmount"), null);
                }

                // Deduct points
                await DeductPointsAsync(userId, pointsToUse);

                // Update payment amount
                payment.NetAmount -= valueOfPoints;
                payment.UpdatedAt = DateTimeOffset.UtcNow;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Redeemed {Points} points for payment {PaymentId}. New payment amount: {NetAmount}", pointsToUse, paymentId, payment.NetAmount);

                // Return updated payment details
                var result = new PaymentDetailsDto
                {
                    Id = payment.Id,
                    UserId = payment.UserId,
                    Amount = payment.Amount,
                    DiscountAmount = payment.DiscountAmount,
                    NetAmount = payment.NetAmount,
                    Status = payment.Status,
                    PaymentDate = payment.PaymentDate,
                    PaymentPurpose = payment.PaymentPurpose,
                    TargetId = payment.TargetId,
                    TargetType = payment.TargetType
                };

                return new GeneralResult<PaymentDetailsDto>(true, _localizationManager.GetLocalizedString("PointsRedeemedSuccessfully"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redeeming points for payment {PaymentId}", paymentId);
                return new GeneralResult<PaymentDetailsDto>(false, _localizationManager.GetLocalizedString("ErrorRedeemingPoints"), null);
            }
        }

        /// <summary>
        /// Calculate points based on net amount spent (10 SAR = 1 point).
        /// </summary>
        private int CalculatePoints(decimal netAmount)
        {
            // Assume 1 point for every 10 SAR spent
            return (int)(netAmount / 10);
        }
    }
}

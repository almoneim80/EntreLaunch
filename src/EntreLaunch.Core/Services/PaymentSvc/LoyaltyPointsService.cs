namespace EntreLaunch.Services.PaymentSvc
{
    public class LoyaltyPointsService : ILoyaltyPointsService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<LoyaltyPointsService> _logger;
        public LoyaltyPointsService(PgDbContext dbContext, ILogger<LoyaltyPointsService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<LoyaltyPointsResult> AddPointsForPaymentAsync(string userId, int paymentId)
        {
            var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId && !p.IsDeleted);

            if (payment == null)
            {
                _logger.LogError($"Payment {paymentId} for user {userId} not found or invalid.");
                return new LoyaltyPointsResult
                {
                    Points = 0,
                    IsSuccess = false,
                    Message = $"Payment {paymentId} for user {userId} not found or invalid."
                };
            }

            if (!payment.Status.Equals("Paid"))
            {
                _logger.LogWarning("Cannot add points for unpaid payment {PaymentId}", paymentId);
                return new LoyaltyPointsResult
                {
                    Points = 0,
                    IsSuccess = false,
                    Message = "Points can only be added for paid payments."
                };
            }

            int pointsToAdd = CalculatePoints(payment.NetAmount ?? 0);
            await RecordPointsTransactionAsync(userId, pointsToAdd, "Points added for payment", paymentId);
            _logger.LogInformation("Added {Points} points to user {UserId} for payment {PaymentId}", pointsToAdd, userId, paymentId);

            return new LoyaltyPointsResult
            {
                Points = pointsToAdd,
                IsSuccess = true,
                Message = $"points added successfully."
            };
        }

        /// <inheritdoc />
        public async Task<LoyaltyPointsResult> AddBonusPointsAsync(string userId, int points, string reason)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("User ID is required.");
                return new LoyaltyPointsResult
                {
                    Points = 0,
                    IsSuccess = false,
                    Message = "User ID cannot be null or empty."
                };
            }

            if (points <= 0)
            {
                _logger.LogError("Points must be greater than zero.");
                return new LoyaltyPointsResult
                {
                    Points = 0,
                    IsSuccess = false,
                    Message = "Points must be greater than zero."
                };
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
            {
                _logger.LogError($"User with ID {userId} not found or deleted.");
                return new LoyaltyPointsResult
                {
                    Points = 0,
                    IsSuccess = false,
                    Message = "Invalid user ID."
                };
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
            };

            _dbContext.LoyaltyPoints.Add(loyaltyPoint);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Bonus points ({points}) added for user {userId}.");
            return new LoyaltyPointsResult
            {
                Points = points,
                IsSuccess = true,
                Message = "Points added successfully."
            };
        }

        /// <inheritdoc />
        public async Task<LoyaltyPointsResult> DeductPointsAsync(string userId, int points)
        {
            if (points <= 0)
            {
                _logger.LogWarning("Invalid points to deduct: {Points}", points);
                return new LoyaltyPointsResult
                {
                    Points = 0,
                    IsSuccess = false,
                    Message = "Points to deduct must be greater than zero."
                };
            }

            int currentPoints = await GetUserPointsAsync(userId);

            if (currentPoints < points)
            {
                _logger.LogWarning("Insufficient points for user {UserId}. Available: {Available}, Required: {Required}", userId, currentPoints, points);
                return new LoyaltyPointsResult
                {
                    Points = 0,
                    IsSuccess = false,
                    Message = "Insufficient points , available: " + currentPoints + ", required: " + points + ")"
                };
            }

            await RecordPointsTransactionAsync(userId, -points, "Points deducted.");
            _logger.LogInformation("Deducted {Points} points from user {UserId}", points, userId);

            return new LoyaltyPointsResult
            {
                Points = points,
                IsSuccess = true,
                Message = "Points deducted successfully."
            };
        }

        /// <inheritdoc />
        public async Task<int> GetUserPointsAsync(string userId)
        {
            var points = await _dbContext.LoyaltyPoints.Where(lp => lp.UserId == userId && !lp.IsDeleted).SumAsync(lp => lp.PointsChanged);

            _logger.LogInformation("User {UserId} has {Points} points", userId, points);
            return points;
        }

        /// <inheritdoc />
        public async Task RecordPointsTransactionAsync(string userId, int pointsChanged, string reason, int? paymentId = null)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                _logger.LogWarning("Transaction reason is required.");
                throw new ArgumentException("Reason is required for points transaction.");
            }

            var transaction = new LoyaltyPoint
            {
                UserId = userId,
                PointsChanged = pointsChanged,
                Reason = reason,
                PaymentId = paymentId ?? null,
                CreatedAt = DateTimeOffset.UtcNow,
                IsDeleted = false
            };

            _dbContext.LoyaltyPoints.Add(transaction);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Recorded points transaction for user {UserId}: {PointsChanged} points ({Reason})", userId, pointsChanged, reason);
        }

        /// <inheritdoc />
        public async Task<PaymentDetailsDto> RedeemPointsForPaymentAsync(string userId, int paymentId, int pointsToUse)
        {
            if (pointsToUse <= 0)
            {
                _logger.LogWarning("Invalid points to redeem: {Points}", pointsToUse);
                throw new ArgumentException("Points to redeem must be greater than zero.");
            }

            var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId && !p.IsDeleted);

            if (payment == null)
            {
                _logger.LogError($"Payment {paymentId} for user {userId} not found or invalid.");
                throw new ArgumentException($"Payment {paymentId} for user {userId} not found.");
            }

            if (payment.Status != "Pending")
            {
                _logger.LogWarning("Cannot redeem points for payment {PaymentId} with status {Status}", paymentId, payment.Status);
                throw new InvalidOperationException("Points can only be redeemed for pending payments.");
            }

            int currentPoints = await GetUserPointsAsync(userId);

            if (currentPoints < pointsToUse)
            {
                _logger.LogWarning("Insufficient points for user {UserId}. Available: {Available}, Required: {Required}", userId, currentPoints, pointsToUse);
                throw new InvalidOperationException("Insufficient points to redeem.");
            }

            // Calculate the monetary value of points (e.g., 1 point = 0.1 SAR)
            decimal valueOfPoints = pointsToUse * 0.1m;

            if (valueOfPoints > payment.NetAmount)
            {
                _logger.LogWarning("Cannot redeem points exceeding payment amount. Points value: {Value}, Payment amount: {Amount}", valueOfPoints, payment.NetAmount);
                throw new InvalidOperationException("Cannot redeem points exceeding payment amount.");
            }

            // Deduct points
            await DeductPointsAsync(userId, pointsToUse);

            // EntreLaunchdate payment amount
            payment.NetAmount -= valueOfPoints;
            payment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Redeemed {Points} points for payment {PaymentId}. New payment amount: {NetAmount}", pointsToUse, paymentId, payment.NetAmount);

            // Return EntreLaunchdated payment details
            return new PaymentDetailsDto
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

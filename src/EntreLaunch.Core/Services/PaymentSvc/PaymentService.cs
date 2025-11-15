namespace EntreLaunch.Services.PaymentSvc
{
    public class PaymentService : IPaymentService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentGateway _paymentGateway;
        private readonly PayTabsOptions _options;
        private readonly ILoyaltyPointsService _loyaltyPointsService;

        public PaymentService(
            PgDbContext dbContext,
            ILogger<PaymentService> logger,
            IPaymentGateway paymentGateway,
            IOptions<PayTabsOptions> options,
            ILoyaltyPointsService loyaltyPointsService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _paymentGateway = paymentGateway;
            _options = options.Value;
            _loyaltyPointsService = loyaltyPointsService;
        }

        /// <inheritdoc />
        public async Task<PaymentDetailsDto> CreatePaymentAsync(PaymentCreateDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1) Data validation (e.g. making sure UserId is in Users)
                User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId && !u.IsDeleted);
                if (user == null)
                {
                    _logger.LogError("User with ID {UserId} not found.", dto.UserId);
                    throw new ArgumentException("Invalid or deleted user.");
                }

                // 2) Create a Payment object within the system with a status of "Pending".
                Payment payment = new Payment
                {
                    UserId = dto.UserId,
                    Amount = dto.Amount ?? 0,
                    DiscountAmount = dto.DiscountAmount ?? 0,
                    NetAmount = dto.Amount - dto.DiscountAmount ?? 0,
                    Status = "Pending",
                    PaymentPurpose = dto.PaymentPurpose,
                    TargetId = dto.TargetId,
                    TargetType = dto.TargetType,
                    PaymentDate = null,
                };

                // 3) Save the Payment object to the database
                _dbContext.Payments.Add(payment);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // 4) Converting a Payment object to a return DTO (simplified example)
                var resultDto = new PaymentDetailsDto
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

                return resultDto;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid input: {Message}", ex.Message);
                throw;
            }
            catch (DbEntreLaunchdateException ex)
            {
                _logger.LogError(ex, "Database EntreLaunchdate error: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<PaymentResult> InitiatePaymentAsync(int paymentId, string paymentToken)
        {
            // (1) Fetching the payment from the database
            var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId && !p.IsDeleted);

            if (payment == null)
            {
                _logger.LogError($"Payment with id {paymentId} not found or deleted.");
                throw new ArgumentException($"Payment with id {paymentId} not found or deleted.");
            }

            // (2) Verification of the current state of payment
            if (payment.Status != "Pending")
            {
                _logger.LogError($"Cannot initiate payment. Current status is '{payment.Status}'.");
                throw new InvalidOperationException($"Cannot initiate payment. Current status is '{payment.Status}'.");
            }

            // (3) Call the payment gateway to finalize the initiative process (to get the payment link/token)
            var gatewayResult = await _paymentGateway.InitiatePaymentAsync(payment, paymentToken);

            if (gatewayResult == null)
            {
                _logger.LogError("Failed to initiate payment. Payment gateway returned null.");
                throw new InvalidOperationException("Failed to initiate payment. Payment gateway returned null.");
            }

            if (!gatewayResult.IsSuccess)
            {
                _logger.LogError($"Failed to initiate payment. Error message: {gatewayResult.ErrorMessage}");
                throw new InvalidOperationException($"Failed to initiate payment. Error message: {gatewayResult.ErrorMessage}");
            }


            // (4) Build `PaymentTransaction`
            var transaction = new PaymentTransaction
            {
                PaymentId = payment.Id,
                ExternalTransactionId = gatewayResult.TransactionId ?? string.Empty,
                Status = gatewayResult.IsSuccess ? "Initiated" : "Failed",
                CreatedAt = DateTimeOffset.UtcNow,
                EntreLaunchdatedAt = DateTimeOffset.UtcNow,
                ResponseData = !string.IsNullOrEmpty(gatewayResult.ErrorMessage) ? new List<string> { gatewayResult.ErrorMessage } : new List<string>()
            };

            // (5) Save data
            _dbContext.PaymentTransactions.Add(transaction);

            // EntreLaunchdate payment status based on result
            payment.Status = gatewayResult.IsSuccess ? "PendingConfirmation" : "Failed";
            payment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync();

            // (6) Constructing a PaymentResult to return to the caller
            var result = new PaymentResult
            {
                IsSuccess = gatewayResult.IsSuccess,
                PaymentStatus = gatewayResult.IsSuccess ? "PendingConfirmation" : "Failed",
                TransactionId = gatewayResult.TransactionId,
                PaidAmount = gatewayResult.PaidAmount,
                PaymentDate = gatewayResult.PaymentDate,
                ErrorMessage = gatewayResult.ErrorMessage
            };

            return result;
        }

        /// <inheritdoc />
        public async Task<bool> ProcessCallbackAsync(Dictionary<string, string> callbackData)
        {
            try
            {
                // (1) Validate the signature from PayTabs
                if (!callbackData.TryGetValue("signature", out var signature) || string.IsNullOrEmpty(signature))
                {
                    _logger.LogError("Callback received without signature.");
                    throw new InvalidOperationException("Invalid callback: Missing signature.");
                }

                if (!ValidateSignature(callbackData, _options.ServerKey))
                {
                    _logger.LogError("Callback signature validation failed.");
                    throw new InvalidOperationException("Invalid callback: Signature mismatch.");
                }

                // (2) Extract necessary fields from the callback data
                if (!callbackData.TryGetValue("tranRef", out var transactionId) || string.IsNullOrEmpty(transactionId))
                {
                    _logger.LogError("Transaction reference missing in callback.");
                    throw new InvalidOperationException("Invalid callback: Missing transaction reference.");
                }

                if (!callbackData.TryGetValue("respStatus", out var responseStatus))
                {
                    _logger.LogError("Response status missing in callback.");
                    throw new InvalidOperationException("Invalid callback: Missing response status.");
                }

                // (3) Fetch the payment record from the database
                var paymentTransaction = await _dbContext.PaymentTransactions.Include(pt => pt.Payment)
                    .FirstOrDefaultAsync(pt => pt.ExternalTransactionId == transactionId);

                if (paymentTransaction == null)
                {
                    _logger.LogError("Transaction not found for transaction ID: {TransactionId}", transactionId);
                    throw new InvalidOperationException($"Transaction not found for transaction ID: {transactionId}");
                }

                var payment = paymentTransaction.Payment;

                if (payment == null)
                {
                    _logger.LogError("Payment record not found for transaction ID: {TransactionId}", transactionId);
                    throw new InvalidOperationException($"Payment record not found for transaction ID: {transactionId}");
                }

                // (4) EntreLaunchdate payment and transaction status based on response
                payment.Status = responseStatus == "A" ? "Success" : "Failed"; // 'A' indicates Authorized/Successful
                payment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                paymentTransaction.Status = responseStatus == "A" ? "Success" : "Failed";
                paymentTransaction.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                if (callbackData.TryGetValue("respMessage", out var responseMessage))
                {
                    paymentTransaction.ResponseData!.Add(responseMessage);
                }

                // (5) Save changes to the database
                await _dbContext.SaveChangesAsync();

                // (6) Add loyalty points if successful
                await _loyaltyPointsService.AddPointsForPaymentAsync(payment.UserId, payment.Id);

                _logger.LogInformation("Callback processed successfully for transaction ID: {TransactionId}", transactionId);

                return true; // Indicates successful processing
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing callback.");
                return false; // Indicates processing failure
            }
        }

        /// <inheritdoc />
        public async Task<bool> ProcessIPNAsync(Dictionary<string, string> ipnData)
        {
            try
            {
                // (1) Extracting and removing the signature from the data
                if (!ipnData.TryGetValue("signature", out string? signature) || string.IsNullOrEmpty(signature))
                {
                    _logger.LogError("Missing or invalid signature in IPN data.");
                    return false;
                }

                ipnData.Remove("signature");

                // (2) Signature verification
                var serverKey = _options.ServerKey;
                var isValidSignature = ValidateSignature(ipnData, signature, serverKey);

                if (!isValidSignature)
                {
                    _logger.LogError("Invalid signature for IPN data.");
                    return false;
                }

                // (3) Analyzing the data
                if (!ipnData.TryGetValue("tran_ref", out string? transactionReference) || !ipnData.TryGetValue("resp_status", out string? responseStatus))
                {
                    _logger.LogError("Missing required IPN fields.");
                    return false;
                }

                // (4) Search for payment and EntreLaunchdate its status
                var paymentTransaction = await _dbContext.PaymentTransactions.FirstOrDefaultAsync(pt => pt.ExternalTransactionId == transactionReference);

                if (paymentTransaction == null)
                {
                    _logger.LogError("Payment transaction not found for tran_ref: {TranRef}", transactionReference);
                    return false;
                }

                // EntreLaunchdate payment status based on response status
                paymentTransaction.Status = responseStatus == "A" ? "Confirmed" : "Failed";
                paymentTransaction.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                // Linked Payment EntreLaunchdate
                var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentTransaction.PaymentId);

                if (payment != null)
                {
                    payment.Status = responseStatus == "A" ? "Confirmed" : "Failed";
                    payment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                }

                // Save Changes
                await _dbContext.SaveChangesAsync();

                // (6) Add loyalty points if successful
                await _loyaltyPointsService.AddPointsForPaymentAsync(payment!.UserId, payment.Id);

                _logger.LogInformation("IPN processed successfully for transaction: {TranRef}", transactionReference);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing IPN.");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<PaymentDetailsDto> GetPaymentByIdAsync(int paymentId)
        {
            if (paymentId <= 0)
            {
                _logger.LogError("Invalid payment ID: {PaymentId} is not a positive integer.", paymentId);
                throw new InvalidOperationException("Payment ID must be a positive integer.");
            }

            Payment? payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId && !p.IsDeleted);

            if (payment == null)
            {
                _logger.LogError("Payment not found: Payment with ID {PaymentId} does not exist or has been deleted.", paymentId);
                throw new InvalidOperationException($"Payment with ID {paymentId} not found.");
            }

            PaymentDetailsDto paymentDetails = new PaymentDetailsDto
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

            return paymentDetails;
        }

        /// <inheritdoc />
        public async Task<bool> IsPaid(int targetId, string userId)
        {
            try
            {
                // 4. Payment Verification
                var hasPaid = await _dbContext.Payments.AnyAsync(p => p.UserId == userId
                                   && p.TargetId == targetId
                                   && p.Status == "Paid"
                                   && !p.IsDeleted);

                if (!hasPaid)
                {
                    _logger.LogError($"User With Id {userId} dont completed payment for this target {targetId}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verifying payment.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> CancelPayment(int paymentId)
        {
            if (paymentId <= 0)
            {
                throw new ArgumentNullException(nameof(paymentId));
            }

            var payment = _dbContext.Payments.FirstOrDefault(p => p.Id == paymentId && !p.IsDeleted);

            if (payment == null)
            {
                _logger.LogError("Payment with ID {PaymentId} does not exist or has been deleted.", paymentId);
                throw new InvalidOperationException($"Payment does not exist or has been deleted.");
            }

            payment.Status = "Cancelled";
            payment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

            var result = await _dbContext.SaveChangesAsync();
            if (result > 0)
                return true;

            return false;
        }

        // helpers methods.

        /// <summary>
        /// Validate Signature.
        /// </summary>
        private bool ValidateSignature(Dictionary<string, string> data, string serverKey)
        {
            // (1) Remove the 'signature' key before validating
            data.Remove("signature");

            // (2) Sort data keys
            var sortedData = data
                .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                .OrderBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // (3) Create a query string
            var queryString = string.Join("&", sortedData.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

            // (4) Generate HMAC SHA256 hash
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(serverKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));

            // (5) Convert hash to hexadecimal
            var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            // (6) Compare computed signature with provided signature
            return string.Equals(data["signature"], computedSignature, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Validate signature.
        /// </summary>
        private bool ValidateSignature(Dictionary<string, string> data, string receivedSignature, string serverKey)
        {
            // Sort data alphabetically
            var sortedData = data.OrderBy(kvp => kvp.Key)
                                 .Select(kvp => $"{kvp.Key}={kvp.Value}");
            var concatenatedString = string.Join("&", sortedData);

            // Create a HMAC-SHA256 signature
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(serverKey));
            var computedSignature = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString))).Replace("-", "").ToLower();

            // Signature verification
            return string.Equals(computedSignature, receivedSignature, StringComparison.OrdinalIgnoreCase);
        }
    }
}

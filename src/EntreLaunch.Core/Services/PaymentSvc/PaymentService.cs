using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading;
using Twilio.TwiML.Messaging;
using EntreLaunch.DTOs.PaymentDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Interfaces.SubscriptionIntf;

namespace EntreLaunch.Services.PaymentSvc
{
    public class PaymentService(
        PgDbContext dbContext,
        ILogger<PaymentService> logger,
        IPaymentGateway paymentGateway,
        ILocalizationManager localizationManager,
        IOptions<PayTabsOptions> options,
        ILoyaltyPointsService loyaltyPointsService) : IPaymentService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<PaymentService> _logger = logger;
        private readonly IPaymentGateway _paymentGateway = paymentGateway;
        private readonly PayTabsOptions _options = options.Value;
        private readonly ILoyaltyPointsService _loyaltyPointsService = loyaltyPointsService;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult<PaymentDetailsDto>> CreatePaymentAsync(PaymentCreateDto dto, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1) التحقق من المستخدم
                var user = await _dbContext.Users.FirstOrDefaultAsync(
                    u => u.Id == dto.UserId && !u.IsDeleted, cancellationToken);

                if (user == null)
                {
                    _logger.LogError("CreatePaymentAsync: User with ID {UserId} not found.", dto.UserId);
                    return new GeneralResult<PaymentDetailsDto>(
                        false,
                        _localizationManager.GetLocalizedString("UserNotFound"),
                        null,
                        ErrorType.NotFound);
                }

                if((dto.TargetType == PaymentType.OnlineCourse || dto.TargetType == PaymentType.SkillsLibCourse) &&
                    (dto.TargetId != null || dto.TargetId < 0))
                {
                    var (isValid, errorMessage, errorType) = await ValidateTargetIdAsync(dto, cancellationToken);
                    if (!isValid)
                    {
                        return new GeneralResult<PaymentDetailsDto>(
                            false,
                            errorMessage!,
                            null,
                            errorType ?? ErrorType.Validation);
                    }
                }

                var (duplicateIsValid, duplicateErrorMessage, duplicateErrorType) = await ValidateDuplicatePaymentAsync(dto, cancellationToken);
                if (!duplicateIsValid)
                {
                    return new GeneralResult<PaymentDetailsDto>(false, duplicateErrorMessage!, null, duplicateErrorType ?? ErrorType.Duplicated);
                }

                // 4) إنشاء الدفع
                var payment = new Payment
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

                _dbContext.Payments.Add(payment);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                // 5) إعداد DTO للإرجاع
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

                return new GeneralResult<PaymentDetailsDto>(true, string.Empty, resultDto);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "CreatePaymentAsync: Database update error: {Message}", ex.Message);
                return new GeneralResult<PaymentDetailsDto>(
                    false,
                    _localizationManager.GetLocalizedString("DatabaseError"),
                    null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreatePaymentAsync: Unexpected error: {Message}", ex.Message);
                return new GeneralResult<PaymentDetailsDto>(
                    false,
                    _localizationManager.GetLocalizedString("UnexpectedError"),
                    null);
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
                UpdatedAt = DateTimeOffset.UtcNow,
                ResponseData = !string.IsNullOrEmpty(gatewayResult.ErrorMessage) ? new List<string> { gatewayResult.ErrorMessage } : new List<string>()
            };

            // (5) Save data
            _dbContext.PaymentTransactions.Add(transaction);

            // Update payment status based on result
            payment.Status = gatewayResult.IsSuccess ? "PendingConfirmation" : "Failed";
            payment.UpdatedAt = DateTimeOffset.UtcNow;

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

                // (4) Update payment and transaction status based on response
                payment.Status = responseStatus == "A" ? "Success" : "Failed"; // 'A' indicates Authorized/Successful
                payment.UpdatedAt = DateTimeOffset.UtcNow;

                paymentTransaction.Status = responseStatus == "A" ? "Success" : "Failed";
                paymentTransaction.UpdatedAt = DateTimeOffset.UtcNow;

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

                // (4) Search for payment and update its status
                var paymentTransaction = await _dbContext.PaymentTransactions.FirstOrDefaultAsync(pt => pt.ExternalTransactionId == transactionReference);

                if (paymentTransaction == null)
                {
                    _logger.LogError("Payment transaction not found for tran_ref: {TranRef}", transactionReference);
                    return false;
                }

                // Update payment status based on response status
                paymentTransaction.Status = responseStatus == "A" ? "Confirmed" : "Failed";
                paymentTransaction.UpdatedAt = DateTimeOffset.UtcNow;

                // Linked Payment Update
                var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentTransaction.PaymentId);

                if (payment != null)
                {
                    payment.Status = responseStatus == "A" ? "Confirmed" : "Failed";
                    payment.UpdatedAt = DateTimeOffset.UtcNow;
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
            payment.UpdatedAt = DateTimeOffset.UtcNow;

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

        /// <summary>
        /// Validate target id.
        /// </summary>
        private async Task<(bool IsValid, string? ErrorMessage, ErrorType? ErrorType)> ValidateTargetIdAsync(
            PaymentCreateDto dto,
            CancellationToken cancellationToken)
        {
            switch (dto.TargetType)
            {
                case PaymentType.SkillsLibCourse:
                    if (!dto.TargetId.HasValue)
                    {
                        _logger.LogWarning("ValidateTargetIdAsync: TargetId is required for SkillsLibCourse.");
                        return (false, _localizationManager.GetLocalizedString("TargetIdRequired"), ErrorType.Validation);
                    }

                    var skillsCourse = await _dbContext.Courses
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c =>
                            c.Id == dto.TargetId.Value &&
                            c.Type == CourseType.SkillsLibCourse &&
                            !c.IsDeleted,
                            cancellationToken);

                    if (skillsCourse == null)
                    {
                        _logger.LogWarning("ValidateTargetIdAsync: SkillsLibCourse with ID {TargetId} not found.", dto.TargetId);
                        return (false, _localizationManager.GetLocalizedString("SkillsLibCourseNotFound"), ErrorType.NotFound);
                    }

                    break;

                case PaymentType.OnlineCourse:
                    if (!dto.TargetId.HasValue)
                    {
                        _logger.LogWarning("ValidateTargetIdAsync: TargetId is required for SkillsLibCourse.");
                        return (false, _localizationManager.GetLocalizedString("TargetIdRequired"), ErrorType.Validation);
                    }

                    var onlineCourse = await _dbContext.Courses
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c =>
                                c.Id == dto.TargetId.Value &&
                                c.Type == CourseType.OnlineCourse &&
                                !c.IsDeleted,
                                cancellationToken);

                    if (onlineCourse == null)
                    {
                        _logger.LogWarning("ValidateTargetIdAsync: Online Course with ID {TargetId} not found.", dto.TargetId);
                        return (false, _localizationManager.GetLocalizedString("onlineCourseNotFound"), ErrorType.NotFound);
                    }

                    break;

                case PaymentType.TrainingPath:
                    if (!dto.TargetId.HasValue)
                    {
                        _logger.LogWarning("ValidateTargetIdAsync: TargetId is required for TrainingPath.");
                        return (false, _localizationManager.GetLocalizedString("TargetIdRequired"), ErrorType.Validation);
                    }

                    var trainingPath = await _dbContext.TrainingPaths
                        .AsNoTracking()
                        .FirstOrDefaultAsync(tp =>
                            tp.Id == dto.TargetId.Value &&
                            !tp.IsDeleted,
                            cancellationToken);

                    if (trainingPath == null)
                    {
                        _logger.LogWarning("ValidateTargetIdAsync: TrainingPath with ID {TargetId} not found.", dto.TargetId);
                        return (false, _localizationManager.GetLocalizedString("TrainingPathNotFound"), ErrorType.NotFound);
                    }

                    break;

                case PaymentType.CertificateShipping:
                    if (!dto.TargetId.HasValue)
                    {
                        _logger.LogWarning("ValidateTargetIdAsync: TargetId is required for CertificateShipping.");
                        return (false, _localizationManager.GetLocalizedString("TargetIdRequired"), ErrorType.Validation);
                    }

                    var certificate = await _dbContext.Certificates
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c =>
                            c.Id == dto.TargetId.Value &&
                            c.UserId == dto.UserId &&
                            !c.IsDeleted,
                            cancellationToken);

                    if (certificate == null)
                    {
                        _logger.LogWarning("ValidateTargetIdAsync: Certificate with ID {TargetId} not found for user {UserId}.",
                            dto.TargetId, dto.UserId);

                        return (false, _localizationManager.GetLocalizedString("CertificateNotFound"), ErrorType.NotFound);
                    }

                    break;
            }

            return (true, null, null);
        }

        private async Task<(bool IsValid, string? ErrorMessage, ErrorType? ErrorType)> ValidateDuplicatePaymentAsync(
    PaymentCreateDto dto,
    CancellationToken cancellationToken)
        {
            // allow duplicate payment for these payment types
            if (dto.TargetType == PaymentType.SpinWheelRetry ||
                dto.TargetType == PaymentType.TextConsultation)
            {
                return (true, null, null);
            }

            // day payment with TargetId and no re-payment with no payment for same TargetId
            var nonRepeatableCourseTypes = new[]
            {
                PaymentType.SkillsLibCourse,
                PaymentType.OnlineCourse,
                PaymentType.TrainingPath
            };

            if (nonRepeatableCourseTypes.Contains(dto.TargetType))
            {
                var hasPaidBefore = await _dbContext.Payments.AnyAsync(p =>
                    p.UserId == dto.UserId &&
                    p.TargetType == dto.TargetType &&
                    p.TargetId == dto.TargetId &&
                    p.Status != "Failed" &&
                    !p.IsDeleted,
                    cancellationToken);

                if (hasPaidBefore)
                {
                    _logger.LogWarning("ValidateDuplicatePaymentAsync: Duplicate non-repeatable course payment for user {UserId}, course {TargetId}.",
                        dto.UserId, dto.TargetId);

                    return (false, _localizationManager.GetLocalizedString("DuplicateCoursePaymentNotAllowed"), ErrorType.Duplicated);
                }
            }

            // monthly payment without TargetId 
            var monthlyTypes = new[]
            {
                PaymentType.MyTeam,
                PaymentType.MyFinance,
                PaymentType.MyPartner,
                PaymentType.MyOpportunity,
                PaymentType.Club
            };

            if (monthlyTypes.Contains(dto.TargetType))
            {
                var thirtyDaysAgo = DateHelper.UtcNow.AddDays(-30);

                var hasRecentMonthlyPayment = await _dbContext.Payments.AnyAsync(p =>
                    p.UserId == dto.UserId &&
                    p.TargetType == dto.TargetType &&
                    p.PaymentDate != null &&
                    p.PaymentDate.Value >= thirtyDaysAgo &&
                    p.Status == "Paid" &&
                    !p.IsDeleted,
                    cancellationToken);

                if (hasRecentMonthlyPayment)
                {
                    _logger.LogWarning("ValidateDuplicatePaymentAsync: Monthly payment already exists for user {UserId}, type {TargetType}.",
                        dto.UserId, dto.TargetType);

                    return (false, _localizationManager.GetLocalizedString("MonthlyPaymentAlreadyExists"), ErrorType.Duplicated);
                }
            }

            if (dto.TargetType == PaymentType.CertificateShipping)
            {
                var hasPaidBefore = await _dbContext.Payments.AnyAsync(p =>
                    p.UserId == dto.UserId &&
                    p.TargetType == PaymentType.CertificateShipping &&
                    p.TargetId == dto.TargetId &&
                    p.Status != "Failed" &&
                    !p.IsDeleted,
                    cancellationToken);

                if (hasPaidBefore)
                {
                    _logger.LogWarning("ValidateDuplicatePaymentAsync: Certificate shipping already paid for user {UserId}, target {TargetId}.",
                        dto.UserId, dto.TargetId);

                    return (false, _localizationManager.GetLocalizedString("CertificateShippingAlreadyPaid"), ErrorType.Duplicated);
                }
            }

            return (true, null, null);
        }
    }
}

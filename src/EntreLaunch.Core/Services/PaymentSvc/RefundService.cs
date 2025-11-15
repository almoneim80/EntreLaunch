namespace EntreLaunch.Services.PaymentSvc
{
    public class RefundService : IRefundService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<RefundService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;
        public RefundService(
            PgDbContext dbContext,
            ILogger<RefundService> logger,
            IConfiguration configuration,
            IMapper mapper,
            IPaymentService paymentService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
            _paymentService = paymentService;
        }

        /// <inheritdoc />
        public async Task<RefundDetailsDto> CreateRefundAsync(RefundCreateDto dto)
        {
            // (1) Checking the existence of the batch in the database
            Payment? payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == dto.PaymentId && !p.IsDeleted);

            if (payment == null)
            {
                _logger.LogWarning($"Payment {dto.PaymentId} not found or is deleted.");
                throw new ArgumentException($"Payment with ID {dto.PaymentId} not found.");
            }

            // (2) Check the payment status (often a refund may only be created if the payment status is Paid)
            if (payment.Status != "Paid")
            {
                _logger.LogWarning($"Cannot create refund request for a payment not in 'Paid' status. Current status is '{payment.Status}'.");
                throw new InvalidOperationException($"Cannot create refund request for a payment not in 'Paid' status. Current status is '{payment.Status}'.");
            }

            // (2.1) Check the date difference between now and the payment date
            // only allow refunds if no more than 7 days have passed since the payment date
            if (payment.PaymentDate.HasValue)
            {
                var daysSincePayment = (DateTimeOffset.UtcNow - payment.PaymentDate!.Value).TotalDays;
                var maxRefundDaysString = _configuration["RefundSettings:MaxRefundDays"];
                if (maxRefundDaysString == null) maxRefundDaysString = "7";

                if (daysSincePayment > double.Parse(maxRefundDaysString))
                {
                    _logger.LogWarning("Cannot create refund request after more than 7 days from the payment date.");
                    throw new InvalidOperationException("Cannot create refund request after more than 7 days from the payment date.");
                }
            }
            else
            {
                // If there is no recorded payment history (rarely if the payment is Paid)
                //  throwing an exception or allowing recovery.
                _logger.LogWarning("Payment date is not specified. Cannot determine refund eligibility.");
                throw new InvalidOperationException("Payment date is not specified. Cannot determine refund eligibility.");
            }

            // (3) Ensure that the refund amount does not exceed the payment amount
            decimal refundAmount = payment.NetAmount ?? 0;
            if (refundAmount <= 0 || refundAmount > (payment.NetAmount ?? 0))
            {
                _logger.LogWarning($"Invalid refund amount. (Requested: {refundAmount}, Payment NetAmount: {payment.NetAmount})");
                throw new InvalidOperationException($"Invalid refund amount. (Requested: {refundAmount}, Payment NetAmount: {payment.NetAmount})");
            }

            // (4) Create a new recovery record if the data is validated
            var refund = new Refund
            {
                PaymentId = payment.Id,
                Amount = refundAmount,
                Reason = dto.Reason,
                RefundDate = dto.RefundDate ?? DateTimeOffset.UtcNow,
                Status = ProcessStatus.Pending,
            };

            _dbContext.Refunds.Add(refund);
            await _dbContext.SaveChangesAsync();

            // (5) Entity conversion to RefundDetailsDto
            var result = new RefundDetailsDto
            {
                Id = refund.Id,
                PaymentId = refund.PaymentId,
                Amount = refund.Payment.NetAmount ?? 0,
                Reason = refund.Reason,
                RefundDate = refund.RefundDate,
                Status = refund.Status
            };

            return result;
        }

        /// <inheritdoc />
        public async Task<RefundDetailsDto> ApproveRefundAsync(int refundId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Fetching the recovery from the database
                var refund = await _dbContext.Refunds.Include(r => r.Payment).FirstOrDefaultAsync(r => r.Id == refundId && !r.IsDeleted);
                if (refund == null)
                {
                    _logger.LogWarning($"Refund {refundId} not found or is deleted.");
                    throw new ArgumentException($"Refund with ID {refundId} not found or is deleted.");
                }

                // Check Current Status
                // Cannot approve a refund that is not in Pending status
                if (refund.Status != ProcessStatus.Pending)
                {
                    _logger.LogWarning($"Cannot approve refund request with status '{refund.Status}'. Must be Pending.");
                    throw new InvalidOperationException($"Cannot approve refund request with status '{refund.Status}'. Must be Pending.");
                }

                // Convert status to Approved 
                refund.Status = ProcessStatus.Approved;
                refund.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                var result = await _paymentService.CancelPayment(refund.PaymentId);
                if (!result)
                {
                    _logger.LogWarning($"Failed to cancel payment {refund.PaymentId} for refund request {refundId}.");
                    throw new InvalidOperationException($"Failed to cancel payment {refund.PaymentId} for refund request {refundId}.");
                }

                // Saving changes to the database
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return Details after converted to Dto
                var refundResult = new RefundDetailsDto
                {
                    Id = refund.Id,
                    PaymentId = refund.PaymentId,
                    Amount = refund.Payment.NetAmount,
                    Reason = refund.Reason,
                    RefundDate = refund.RefundDate,
                    Status = refund.Status
                };

                return refundResult;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while creating a new entity.");
                throw new CustomException(_logger, "Error occurred while creating a new entity.", ex);
            }
        }

        /// <inheritdoc />
        public async Task<RefundDetailsDto> RejectRefundAsync(int refundId)
        {
            // (1) Fetching the recovery from the database
            var refund = await _dbContext.Refunds.Include(r => r.Payment).FirstOrDefaultAsync(r => r.Id == refundId && !r.IsDeleted);

            if (refund == null)
            {
                _logger.LogWarning($"Refund request {refundId} not found or is deleted.");
                throw new ArgumentException($"Refund request with ID {refundId} not found or is deleted.");
            }

            // (2) Checking the current state
            if (refund.Status != ProcessStatus.Pending)
            {
                _logger.LogWarning($"Cannot reject refund request with status '{refund.Status}'. Must be Pending.");
                throw new InvalidOperationException($"Cannot reject refund request with status '{refund.Status}'. Must be Pending.");
            }

            // (3) EntreLaunchdate status to Rejected
            refund.Status = ProcessStatus.Rejected;
            refund.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

            // (4) Save Changes
            await _dbContext.SaveChangesAsync();

            // (5) Return Details
            var result = new RefundDetailsDto
            {
                Id = refund.Id,
                PaymentId = refund.PaymentId,
                Amount = refund.Amount,
                Reason = refund.Reason,
                RefundDate = refund.RefundDate,
                Status = refund.Status
            };

            return result;
        }

        /// <inheritdoc />
        public async Task<RefundDetailsDto> GetRefundByIdAsync(int refundId)
        {
            // (1) Fetch Recovery
            var refund = await _dbContext.Refunds.FirstOrDefaultAsync(r => r.Id == refundId && !r.IsDeleted);

            if (refund == null)
            {
                _logger.LogWarning($"Refund request {refundId} not found or is deleted.");
                throw new ArgumentException($"Refund request with ID {refundId} not found or is deleted.");
            }

            // (2) Convert to Dto
            var result = new RefundDetailsDto
            {
                Id = refund.Id,
                PaymentId = refund.PaymentId,
                Amount = refund.Amount,
                Reason = refund.Reason,
                RefundDate = refund.RefundDate,
                Status = refund.Status
            };

            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RefundDetailsDto>> GetAllRefundsAsync()
        {
            // (1) return all orders.
            var refunds = await _dbContext.Refunds.Where(r => !r.IsDeleted).ToListAsync();

            if (refunds.Count > 0)
            {
                var result = _mapper.Map<List<RefundDetailsDto>>(refunds);
                return result;
            }

            return Enumerable.Empty<RefundDetailsDto>();
        }
    }
}

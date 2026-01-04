namespace EntreLaunch.Controllers.PaymentAPI
{
    [Authorize(Roles = AppRoles.PaymentRoles)]
    [Route("api/[controller]")]
    public class PaymentController(
        IPaymentService paymentService,
        ILogger<PaymentController> logger,
        ISubscriptionService subscriptionService,
        IPurchaseService purchaseService,
        ILocalizationManager localizationManager) : AuthenticatedController(localizationManager)
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly ILogger<PaymentController> _logger = logger;
        private readonly ISubscriptionService _subscriptionService = subscriptionService;
        private readonly IPurchaseService _purchaseService = purchaseService;

        /// <summary>
        /// Create a new payment.
        /// </summary>
        [HttpPost]
        [RequiredPermission(PaymentPermissions.Create)]
        public async Task<IActionResult> Create([FromBody] PaymentCreateDto createDto, CancellationToken cancellationToken)
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            createDto.UserId = CurrentUserId!;
            createDto.Status = "Paid";

            var result = await _paymentService.CreatePaymentAsync(createDto, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("CreatePaymentAsync failed: {Message}", result.Message);
                return BadRequest(result);
            }

            if (result.Data!.PaymentPurpose == PaymentPurpose.Subscription)
            {
                await CreateSubscriptionAfterPaymentAsync(result.Data);
            }

            if (result.Data.PaymentPurpose == PaymentPurpose.Buy)
            {
                await CreatePurchaseAfterPaymentAsync(result.Data);
            }

            return Ok(result);
        }

        [NonAction]
        [HttpPost("initiate")]
        public async Task<ActionResult<PaymentResult>> InitiatePayment([FromBody] PaymentInitiateRequest request)
        {
            var paymentResult = await _paymentService.InitiatePaymentAsync(request.PaymentId, request.PaymentToken);

            if (paymentResult == null)
            {
                _logger.LogError("Failed to initiate payment.");
                return BadRequest("Failed to initiate payment.");
            }

            return Ok(paymentResult);
        }

        [NonAction]
        [HttpPost("callback")]
        public async Task<IActionResult> HandleCallback([FromBody] PayTabsCallbackRequest request)
        {
            try
            {
                // (1) Convert the incoming object into a dictionary to pass to the processing function
                var callbackData = request.ToDictionary();

                // (2) Calling the function from PaymentService to process the data
                var isProcessed = await _paymentService.ProcessCallbackAsync(callbackData);

                if (!isProcessed)
                {
                    _logger.LogError("Callback processing failed.");
                    return BadRequest("Failed to process callback.");
                }

                // (3) Success response when the request is successfully processed
                return Ok("Callback processed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling callback.");
                return StatusCode(500, "An error occurred while processing the callback.");
            }
        }

        [NonAction]
        [HttpPost("ipn")]
        public async Task<IActionResult> HandleIPN([FromBody] PayTabsIpnRequest request)
        {
            try
            {
                // Convert the request to a dictionary
                var ipnData = request.ToDictionary();

                // Call the processing function
                var isProcessed = await _paymentService.ProcessIPNAsync(ipnData);

                if (!isProcessed)
                {
                    _logger.LogError("Failed to process IPN.");
                    return BadRequest("Failed to process IPN.");
                }

                return Ok("IPN processed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling IPN.");
                return StatusCode(500, "An error occurred while processing the IPN.");
            }
        }

        /// <summary>
        /// Create subscription after payment.
        /// </summary>
        private async Task CreateSubscriptionAfterPaymentAsync(PaymentDetailsDto payment)
        {
            var type = MapPaymentTypeToSubscriptionType(payment.TargetType);

            var dto = new SubscriptionCreateDto
            {
                UserId = payment.UserId,
                Type = type,
                ReferenceId = payment.TargetId,
                Price = payment.NetAmount ?? 0,
                PaymentId = payment.Id,
                IsAutoRenewal = false
            };

            await _subscriptionService.CreateSubscriptionAsync(dto);
        }

        private static SubscriptionType MapPaymentTypeToSubscriptionType(PaymentType paymentType)
        {
            return paymentType switch
            {
                PaymentType.MyOpportunity => SubscriptionType.MyOpportunity,
                PaymentType.MyPartner => SubscriptionType.MyPartner,
                PaymentType.MyTeam => SubscriptionType.MyTeam,
                PaymentType.MyFinance => SubscriptionType.MyFinance,
                PaymentType.TrainingPath => SubscriptionType.TrainingPath,
                PaymentType.Club => SubscriptionType.Club,
                _ => throw new InvalidOperationException($"Unsupported payment type: {paymentType}")
            };
        }


        /// <summary>
        /// Creates a purchase record after successful payment (PaymentPurpose.Buy).
        /// </summary>
        private async Task CreatePurchaseAfterPaymentAsync(PaymentDetailsDto payment)
        {
            var type = MapPaymentTypeToPurchaseItemType(payment.TargetType);

            var dto = new PurchaseCreateDto
            {
                UserId = payment.UserId,
                ItemType = type,
                ReferenceId = payment.TargetId ?? -1,
                Price = payment.NetAmount ?? 0,
                PaymentId = payment.Id,
                MetadataJson = null
            };

            await _purchaseService.CreatePurchaseAsync(dto);
        }

        private static PurchaseItemType MapPaymentTypeToPurchaseItemType(PaymentType paymentType)
        {
            return paymentType switch
            {
                PaymentType.CertificateShipping => PurchaseItemType.CertificateShipping,
                PaymentType.OnlineCourse => PurchaseItemType.OnlineCourse,
                PaymentType.SkillsLibCourse => PurchaseItemType.SkillsLibCourse,
                PaymentType.OnlineConsultation => PurchaseItemType.OnlineConsultation,
                PaymentType.TextConsultation => PurchaseItemType.TextConsultation,
                PaymentType.SpinWheelRetry => PurchaseItemType.SpinWheelRetry,

                _ => throw new InvalidOperationException($"Unsupported PaymentType: {paymentType}")
            };
        }
    }
}

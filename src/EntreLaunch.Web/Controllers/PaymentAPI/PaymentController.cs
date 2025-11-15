namespace EntreLaunch.Controllers.PaymentAPI
{
    [Authorize(Roles = "Admin, Entrepreneur")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new payment.
        /// </summary>
        [HttpPost]
        [RequiredPermission(Permissions.PaymentPermissions.Create)]
        public async Task<ActionResult<PaymentDetailsDto>> Create([FromBody] PaymentCreateDto createDto)
        {
            //  create payment
            var paymentDto = await _paymentService.CreatePaymentAsync(createDto);

            if (paymentDto == null)
            {
                _logger.LogError("Failed to create payment.");
                return BadRequest("Failed to create payment.");
            }

            return paymentDto;
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
    }
}

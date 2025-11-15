namespace EntreLaunch.Web.Controllers.PaymentAPI
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Entrepreneur")]
    public class LoyaltyPointsController : Controller
    {
        private readonly ILoyaltyPointsService _loyaltyPointsService;
        private readonly ILogger<LoyaltyPointsController> _logger;

        public LoyaltyPointsController(
            ILoyaltyPointsService loyaltyPointsService,
            ILogger<LoyaltyPointsController> logger)
        {
            _loyaltyPointsService = loyaltyPointsService;
            _logger = logger;
        }

        /// <summary>
        /// Calculate and add points to the user based on a specific payment.
        /// </summary>
        [HttpPost("add-points")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.LoyaltyPointPermissions.Create)]
        public async Task<IActionResult> AddPointsForPayment([FromQuery] string userId, [FromQuery] int paymentId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { Message = "User ID is required." });
                }

                if (paymentId <= 0)
                {
                    _logger.LogWarning("Invalid payment ID: {PaymentId} is not a positive integer.", paymentId);
                    return BadRequest(new { Message = "Payment ID is required." });
                }

                var result = await _loyaltyPointsService.AddPointsForPaymentAsync(userId, paymentId);

                return Ok(new
                {
                    result.Points,
                    result.IsSuccess,
                    result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding points for payment (UserId: {UserId}, PaymentId: {PaymentId})", userId, paymentId);
                return StatusCode(500, new { Message = "An error occurred while adding points." });
            }
        }

        /// <summary>
        /// Add reward points to the user.
        /// </summary>
        [HttpPost("add-bonus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.LoyaltyPointPermissions.CreateBonus)]
        public async Task<IActionResult> AddBonusPoints([FromBody] BonusPointsRequest request)
        {
            try
            {
                if (request.Points <= 0)
                {
                    _logger.LogWarning("Invalid points: {Points} is not a positive integer.", request.Points);
                    return BadRequest(new { Message = "Points must be a positive number." });
                }

                var result = await _loyaltyPointsService.AddBonusPointsAsync(request.UserId, request.Points, request.Reason!);
                return Ok(new
                {
                    result.Points,
                    result.IsSuccess,
                    result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add bonus points.");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Deduct points from the user.
        /// </summary>
        [HttpPost("deduct-points")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.LoyaltyPointPermissions.Deduct)]
        public async Task<IActionResult> DeductPoints([FromQuery] string userId, [FromQuery] int points)
        {
            try
            {
                if (userId == null)
                {
                    _logger.LogError("User ID is required.");
                    return BadRequest(new { Message = "User ID is required." });
                }

                LoyaltyPointsResult success = await _loyaltyPointsService.DeductPointsAsync(userId, points);
                if (success.IsSuccess)
                {
                    return Ok(new { success.Message });
                }

                return BadRequest(new { success.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deducting points (UserId: {UserId}, Points: {Points})", userId, points);
                return StatusCode(500, new { Message = "An error occurred while deducting points." });
            }
        }


        /// <summary>
        /// Retrieve the user's current balance of points.
        /// </summary>
        [HttpGet("get-points")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.LoyaltyPointPermissions.ShowOne)]
        public async Task<IActionResult> GetUserPoints(string userId)
        {
            try
            {
                int points = await _loyaltyPointsService.GetUserPointsAsync(userId);
                return Ok(new { UserId = userId, Points = points });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user points (UserId: {UserId})", userId);
                return StatusCode(500, new { Message = "An error occurred while retrieving points." });
            }
        }

        /// <summary>
        /// Redeem a certain number of points in a payment.
        /// </summary>
        [HttpPost("redeem-points")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(Permissions.LoyaltyPointPermissions.Redeem)]
        public async Task<IActionResult> RedeemPointsForPayment([FromBody] RedeemPointsRequestDto redeemRequest)
        {
            try
            {
                if (redeemRequest == null)
                {
                    _logger.LogWarning("Invalid redeem request.");
                    return BadRequest(new { Message = "Invalid redeem request." });
                }

                var paymentDetails = await _loyaltyPointsService.RedeemPointsForPaymentAsync(
                    redeemRequest.UserId,
                    redeemRequest.PaymentId,
                    redeemRequest.PointsToUse);

                return Ok(paymentDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redeeming points (UserId: {UserId}, PaymentId: {PaymentId})",
                    redeemRequest.UserId,
                    redeemRequest.PaymentId);
                return StatusCode(500, new { Message = "An error occurred while redeeming points." });
            }
        }
    }
}

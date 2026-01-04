namespace EntreLaunch.Web.Controllers.PaymentAPI
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.LoyaltyPointsRoles)]
    public class LoyaltyPointsController(
        ILocalizationManager localizationManager,
        ILoyaltyPointsService loyaltyPointsService,
        ILogger<LoyaltyPointsController> logger) : AuthenticatedController(localizationManager)
    {
        private readonly ILoyaltyPointsService _loyaltyPointsService = loyaltyPointsService;
        private readonly ILogger<LoyaltyPointsController> _logger = logger;

        /// <summary>
        /// Calculate and add points to the user based on a specific payment.
        /// </summary>
        [HttpPost("add-points/{paymentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(LoyaltyPointPermissions.Create)]
        public async Task<IActionResult> AddPointsForPayment([FromRoute] int paymentId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _loyaltyPointsService.AddPointsForPaymentAsync(CurrentUserId!, paymentId);
                if(result.IsSuccess == false)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding points for payment (UserId: {UserId}, PaymentId: {PaymentId})", CurrentUserId!, paymentId);
                return this.UnexpectedError("adding points.");
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
        [RequiredPermission(LoyaltyPointPermissions.CreateBonus)]
        public async Task<IActionResult> AddBonusPoints([FromBody] BonusPointsRequest request)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _loyaltyPointsService.AddBonusPointsAsync(CurrentUserId!, request.Points, request.Reason!);
                if (result.IsSuccess == false)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add bonus points.");
                return this.UnexpectedError("add bonus points");
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
        [RequiredPermission(LoyaltyPointPermissions.Deduct)]
        public async Task<IActionResult> DeductPoints([FromQuery] int points)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _loyaltyPointsService.DeductPointsAsync(CurrentUserId!, points);
                if (result.IsSuccess == false)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deducting points (UserId: {UserId}, Points: {Points})", CurrentUserId!, points);
                return this.UnexpectedError("deducting points.");
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
        [RequiredPermission(LoyaltyPointPermissions.GetOne)]
        public async Task<IActionResult> GetUserPoints()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                int points = await _loyaltyPointsService.GetUserPointsAsync(CurrentUserId!);
                return Ok(new { UserId = CurrentUserId!, Points = points });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user points (UserId: {UserId})", CurrentUserId!);
                return this.UnexpectedError("retrieving points.");
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
        [RequiredPermission(LoyaltyPointPermissions.Redeem)]
        public async Task<IActionResult> RedeemPointsForPayment([FromBody] RedeemPointsRequestDto redeemRequest)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var paymentDetails = await _loyaltyPointsService.RedeemPointsForPaymentAsync(
                    CurrentUserId!,
                    redeemRequest.PaymentId,
                    redeemRequest.PointsToUse);

                if(paymentDetails.IsSuccess == false)
                {
                    return BadRequest(paymentDetails);
                }

                return Ok(paymentDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error redeeming points (UserId: {UserId}, PaymentId: {PaymentId})",
                    CurrentUserId!,
                    redeemRequest.PaymentId);
                return this.UnexpectedError("redeeming points.");
            }
        }
    }
}

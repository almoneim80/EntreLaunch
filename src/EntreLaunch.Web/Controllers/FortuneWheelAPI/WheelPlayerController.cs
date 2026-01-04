namespace EntreLaunch.Controllers.FortuneWheelAPI
{
    [Authorize(Roles = AppRoles.WheelRoles)]
    [Route("api/[controller]")]
    public class WheelPlayerController(
        ILocalizationManager localization,
        ILogger<WheelPlayerController> logger,
        IWheelPlayerService wheelPlayerService) : AuthenticatedController(localization)
    {
        private readonly ILogger<WheelPlayerController> _logger = logger;
        private readonly IWheelPlayerService _wheelPlayerService = wheelPlayerService;

        /// <summary>
        /// Spins the wheel for the current authenticated user for a specified award.
        /// </summary>
        /// <param name="awardId">The identifier of the WheelAward to be spun.</param>
        /// <returns>
        /// Returns the spin result if successful; otherwise, returns HTTP 400 for validation or business errors such as ineligible spin attempts.
        /// </returns>
        [HttpPost("spin")]
        [RequiredPermission(WheelPlayerPermissions.Spin)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Spin([FromQuery] int awardId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _wheelPlayerService.SpinAsync(CurrentUserId!, awardId);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("spinning wheel");
            }
        }

        /// <summary>
        /// Checks whether a player is eligible to spin the wheel on the current day.
        /// </summary>
        /// <returns>
        /// Returns a success result if the player can spin today; otherwise, returns HTTP 400 indicating the reason for ineligibility.
        /// </returns>
        [HttpGet("can-play")]
        [RequiredPermission(WheelPlayerPermissions.CanPlay)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CanPlay()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _wheelPlayerService.CanPlayTodayAsync(CurrentUserId!);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("checking if player can play");
            }
        }

        /// <summary>
        /// Retrieves the player's spin result for the current day, if available.
        /// </summary>
        /// <returns>
        /// Returns the spin result for today if available; otherwise, returns HTTP 400 with failure details.
        /// </returns>
        [HttpGet("today-spins")]
        [RequiredPermission(WheelPlayerPermissions.TodaySpin)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodaySpin()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _wheelPlayerService.GetTodaySpinAsync(CurrentUserId!);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("getting today spin");
            }
        }

        /// <summary>
        /// Retrieves the complete spin history of a specific player.
        /// </summary>
        /// <returns>
        /// Returns the spin history if found; otherwise, returns HTTP 400 if retrieval fails.
        /// </returns>
        [HttpGet("history")]
        [RequiredPermission(WheelPlayerPermissions.History)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _wheelPlayerService.GetPlayerHistoryAsync(CurrentUserId!);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.UnexpectedError("getting player history");
            }
        }

        /// <summary>
        /// Retrieves all wheel play records with pagination for admins.
        /// </summary>
        [HttpGet("all-plays")]
        [RequiredPermission(WheelPlayerPermissions.ViewAllPlays)]
        [ProducesResponseType(typeof(GeneralResult<PaginatedResult<WheelPlayDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPlays([FromQuery] PaginationParams pagination)
        {
            try
            {
                var result = await _wheelPlayerService.GetAllUserPlaysAsync(pagination);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all wheel plays.");
                return this.UnexpectedError("loading all plays");
            }
        }

        /// <summary>
        /// Updates the delivery status of a physical item spin.
        /// </summary>
        [HttpPut("{playId}/delivery-status")]
        [RequiredPermission(WheelPlayerPermissions.ManageDelivery)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDeliveryStatus([FromRoute] int playId, [FromQuery] bool isDelivered)
        {
            try
            {
                var result = await _wheelPlayerService.MarkPlayDeliveredAsync(playId, isDelivered);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating delivery status for PlayId={PlayId}", playId);
                return this.UnexpectedError("updating delivery status");
            }
        }

        /// <summary>
        /// Retrieves wheel plays with a specific delivery status (for physical items).
        /// </summary>
        [HttpGet("plays-by-delivery")]
        [RequiredPermission(WheelPlayerPermissions.ViewDeliveryStatus)]
        [ProducesResponseType(typeof(GeneralResult<PaginatedResult<WheelPlayDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPlaysByDeliveryStatus([FromQuery] bool delivered, [FromQuery] PaginationParams pagination)
        {
            try
            {
                var result = await _wheelPlayerService.GetPlaysByDeliveryStatusAsync(delivered, pagination);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plays by delivery status.");
                return this.UnexpectedError("loading plays by delivery");
            }
        }

        /// <summary>
        /// Retrieves paginated physical item wheel plays across all users,
        /// optionally filtered by delivery status.
        /// </summary>
        /// <param name="isDelivered">true for delivered, false for undelivered, null for all.</param>
        /// <param name="pagination">Pagination parameters.</param>
        [HttpGet("physical-plays")]
        [RequiredPermission(WheelPlayerPermissions.ViewPhysicalDelivery)]
        [ProducesResponseType(typeof(GeneralResult<PaginatedResult<WheelPlayDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPhysicalItemPlays([FromQuery] bool? isDelivered, [FromQuery] PaginationParams pagination)
        {
            try
            {
                var result = await _wheelPlayerService.GetPhysicalItemPlaysByDeliveryStatusAsync(isDelivered, pagination);
                if (!result.IsSuccess) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving physical item plays.");
                return this.UnexpectedError("loading physical item plays");
            }
        }

        /// <summary>
        /// Updates the delivery status of a physical item award for a specific wheel play.
        /// </summary>
        /// <param name="playId">The play entry identifier.</param>
        /// <param name="isDelivered">true to mark as delivered, false otherwise.</param>
        [HttpPut("{playId}/physical-delivery")]
        [RequiredPermission(WheelPlayerPermissions.ManagePhysicalDelivery)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePhysicalItemDelivery([FromRoute] int playId, [FromQuery] bool isDelivered)
        {
            try
            {
                var result = await _wheelPlayerService.UpdatePhysicalItemDeliveryStatusAsync(playId, isDelivered);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating physical item delivery status. PlayId={PlayId}", playId);
                return this.UnexpectedError("updating physical item delivery");
            }
        }
    }
}


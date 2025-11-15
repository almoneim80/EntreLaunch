namespace EntreLaunch.Web.Controllers.LaunchingProjectAPI
{
    [Authorize(Roles = "Admin, Entrepreneur")]
    [Route("api/[controller]")]
    public class SimulationController : AuthenticatedController
    {
        private readonly ILogger<SimulationController> _logger;
        private readonly CascadeDeleteService _deleteService;
        private readonly ISimulationService _simulationService;
        public SimulationController(
            ILogger<SimulationController> logger,
            CascadeDeleteService deleteService,
            ILocalizationManager? localization,
            ISimulationService simulationService)
        {
            _logger = logger;
            _deleteService = deleteService;
            _simulationService = simulationService;
        }

        /// <summary>
        /// Create simulation.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SimulationPermissions.Create)]
        public async Task<IActionResult> CreateSimulation([FromBody] SimulationCreateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                dto.projectDto.UserId = CurrentUserId;
                var result = await _simulationService.CreateSimulationAsync(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateSimulation.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while creating simulation", Data = null });
            }
        }

        /// <summary>
        /// Register a new guest.
        /// </summary>
        [HttpPost("add-guest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterGuest([FromBody] GuestRegisterDto dto)
        {
            try
            {
                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _simulationService.RegisterGuestAsync(dto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in RegisterGuest.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while registering guest", Data = null });
            }
        }

        /// <summary>
        /// The guest likes an ad.
        /// </summary>
        [HttpPost("ad-like/{adId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        public async Task<IActionResult> LikeProductAd([FromRoute] int adId, string userId)
        {
            try
            {
                var result = await _simulationService.LikeProductAdAsync(adId, userId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in LikeProductAd.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while liking product ad", Data = null });
            }
        }

        /// <summary>
        /// EntreLaunchdate simulation status.
        /// </summary>
        [HttpPatch("EntreLaunchdate/{simulationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SimulationPermissions.EntreLaunchdate)]
        public async Task<IActionResult> EntreLaunchdateSimulationStatus(int simulationId, SimulationStatus newStatus)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _simulationService.EntreLaunchdateSimulationStatusAsync(simulationId, newStatus);
                if (result.IsSuccess == false)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in EntreLaunchdateSimulationStatus.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while EntreLaunchdating simulation status", Data = null });
            }
        }

        /// <summary>
        /// Fetch all simulations regardless of state.
        /// </summary>
        [HttpGet("get-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SimulationPermissions.GetAll)]
        public async Task<IActionResult> GetAllSimulations()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _simulationService.GetAllSimulationsAsync();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllSimulations.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting all simulations", Data = null });
            }
        }

        /// <summary>
        /// Fetch a specific simulation in detail based on the ID.
        /// </summary>
        [HttpGet("by-id/{simulationId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SimulationPermissions.GetOne)]
        public async Task<IActionResult> GetSimulationById([FromRoute] int simulationId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _simulationService.GetSimulationByIdAsync(simulationId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetSimulationById.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting simulation by id", Data = null });
            }
        }

        /// <summary>
        /// Get simulation ads.
        /// </summary>
        [HttpGet("by-ad/{simulationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SimulationPermissions.GetAds)]
        public async Task<IActionResult> GetSimulationAds([FromRoute] int simulationId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _simulationService.GetSimulationAdsAsync(simulationId, CurrentUserId!);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetSimulationAds.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting simulation ads", Data = null });
            }
        }

        /// <summary>
        /// Fetch all simulations by status (Pending, Accepted, Rejected).
        /// </summary>
        [HttpGet("by-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SimulationPermissions.GetByStatus)]
        public async Task<IActionResult> GetSimulationsByStatus(SimulationStatus status)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _simulationService.GetSimulationsByStatusAsync(status);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetSimulationsByStatus.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting simulations by status", Data = null });
            }
        }

        /// <summary>
        /// Get the number of likes for a particular ad.
        /// </summary>
        [HttpGet("likes-count/{adId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(SimulationPermissions.LikeCount)]
        public async Task<IActionResult> GetLikeCount([FromRoute] int adId)
        {
            try
            {
                var result = await _simulationService.GetAdLikeCountAsync(adId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetLikeCount.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting like count", Data = null });
            }
        }

        /// <summary>
        /// Delete a specific simulation based on the ID.
        /// </summary>
        [HttpDelete("delete/{simulationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SimulationPermissions.Delete)]
        public async Task<IActionResult> DeleteSimulation(int simulationId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (simulationId <= 0)
                {
                    return BadRequest(new GeneralResult { IsSuccess = false, Message = "Invalid ID sEntreLaunchplied." });
                }

                var transactionId = Guid.NewGuid();
                _logger.LogInformation("Transaction {TransactionId}: Starting soft delete for entity ID {Id}.", transactionId, simulationId);

                try
                {
                    var result = await _deleteService.SoftDeleteCascadeAsync<Simulation>(simulationId);
                    if (result.IsSuccess == false)
                    {
                        _logger.LogWarning("Transaction {TransactionId}: Entity with ID {Id} not found or already deleted.", transactionId, simulationId);
                        return NotFound(result);
                    }

                    _logger.LogInformation("Transaction {TransactionId}: Successfully soft deleted entity ID {Id}.", transactionId, simulationId);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Transaction {TransactionId}: Unexpected error occurred while deleting entity ID {Id}.", transactionId, simulationId);
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while deleting simulation", Data = null });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteSimulation.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while deleting simulation", Data = null });
            }
        }
    }
}


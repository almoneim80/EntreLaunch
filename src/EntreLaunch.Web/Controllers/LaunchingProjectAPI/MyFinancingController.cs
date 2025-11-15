namespace EntreLaunch.Web.Controllers.LaunchingProjectAPI
{
    [Authorize(Roles = "Admin, Entrepreneur")]
    [Route("api/[controller]")]
    public class MyFinancingController : AuthenticatedController
    {
        private readonly IMyFinancingService _myFinancingService;
        private readonly ILogger<MyFinancingController> _logger;
        private readonly IExtendedBaseService _extendedBaseService;
        public MyFinancingController(IMyFinancingService myFinancingService, ILogger<MyFinancingController> logger, IExtendedBaseService extendedBaseService)
        {
            _myFinancingService = myFinancingService;
            _logger = logger;
            _extendedBaseService = extendedBaseService;
        }

        /// <summary>
        /// Send financing request.
        /// </summary>
        [HttpPost("send-financing-request")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.SenDOBportunityRequest)]
        public async Task<IActionResult> SendFinancingRequest([FromBody] CreateOpportunityRequestDto request)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var invalidRefCheck = await request.OpportunityId.CheckIfEntityExistsAsync<Opportunity>(_extendedBaseService, _logger);
                if (invalidRefCheck != null) return invalidRefCheck;

                request.userId = CurrentUserId!;
                var result = await _myFinancingService.SendRequest(request);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SendFinancingRequest.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while sending financing request", Data = null });
            }
        }

        /// <summary>
        /// Progress requests state (Accepted, Rejected).
        /// </summary>
        [HttpPost("financing-requests/progress")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.ProgressRequest)]
        public async Task<IActionResult> ProgressRequest([FromBody] ProcessOpportunityRequestDto processOpportunityRequest)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _myFinancingService.ProgressRequests(processOpportunityRequest);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ProgressRequest.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while processing financing request", Data = null });
            }
        }

        /// <summary>
        /// Filtering Investment Opportunities (My Opportunity).
        /// </summary>
        [HttpPost("filtering")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.Filter)]
        public async Task<IActionResult> Filtering([FromBody] OpportunityFilterDto filterDto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var opportunity = await _myFinancingService.Filtering(filterDto);
                if (opportunity.IsSuccess == false)
                {
                    return BadRequest(opportunity);
                }

                return Ok(opportunity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Filtering.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while filtering opportunity", Data = null });
            }
        }

        /// <summary>
        /// Get all financing opportunities.
        /// </summary>
        [HttpGet("financing-opportunities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.ShowOpportunities)]
        public async Task<IActionResult> AllFinancingOpportunities()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myFinancingService.AllFinancingOpportunities();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AllFinancingOpportunities.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting financing opportunities", Data = null });
            }
        }

        /// <summary>
        /// Get all opportunity requests.
        /// </summary>
        [HttpGet("all-financing-requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.ShowAll)]
        public async Task<IActionResult> GetAllRequests()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myFinancingService.AllRequests();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllRequests.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting financing requests", Data = null });
            }
        }

        /// <summary>
        /// Get all pending opportunity requests.
        /// </summary>
        [HttpGet("financing-requests/pending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.ShowPending)]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myFinancingService.PendingRequests();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPendingRequests.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting financing requests", Data = null });
            }
        }

        /// <summary>
        /// Get all accepted opportunity requests.
        /// </summary>
        [HttpGet("financing-requests/accepted")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.ShowAccepted)]
        public async Task<IActionResult> GetAcceptedRequests()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myFinancingService.AcceptedRequests();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAcceptedRequests.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting financing requests", Data = null });
            }
        }

        /// <summary>
        /// Get all rejected opportunity requests.
        /// </summary>
        [HttpGet("financing-requests/rejected")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.ShowRejected)]
        public async Task<IActionResult> GetRejectedRequests()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myFinancingService.RejectedRequests();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetRejectedRequests.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting financing requests", Data = null });
            }
        }
    }
}

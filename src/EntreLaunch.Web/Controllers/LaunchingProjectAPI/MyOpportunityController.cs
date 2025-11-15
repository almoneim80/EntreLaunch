namespace EntreLaunch.Web.Controllers.LaunchingProjectAPI
{
    [Authorize(Roles = "Admin, Entrepreneur")]
    [Route("api/[controller]")]
    public class MyOpportunityController : AuthenticatedController
    {
        private readonly IMyOpportunityService _myOpportunityFacade;
        private readonly ILogger<MyOpportunityController> _logger;
        private readonly IExtendedBaseService _extendedBaseService;
        public MyOpportunityController(
            IMyOpportunityService myOpportunityFacade,
            ILogger<MyOpportunityController> logger,
            IExtendedBaseService extendedBaseService)
        {
            _myOpportunityFacade = myOpportunityFacade;
            _logger = logger;
            _extendedBaseService = extendedBaseService;
        }

        /// <summary>
        /// Send opportunity investment request.
        /// </summary>
        [HttpPost("send-request")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.SenDOBportunityRequest)]
        public async Task<IActionResult> SendOpportunityRequest([FromBody] CreateOpportunityRequestDto request)
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
                var result = await _myOpportunityFacade.Requests.SendRequest(request);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in SenDOBportunityRequest.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while sending opportunity request", Data = null });
            }
        }

        /// <summary>
        /// Progress requests state (Accepted, Rejected).
        /// </summary>
        [HttpPost("requests/progress")]
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

                var result = await _myOpportunityFacade.Requests.ProgressRequests(processOpportunityRequest);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in ProgressRequest.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while progress opportunity request", Data = null });
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

                var result = await _myOpportunityFacade.Filters.Filtering(filterDto);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in Filtering.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while filtering opportunity", Data = null });
            }
        }

        /// <summary>
        /// Get all investment opportunities.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.ShowOpportunities)]
        public async Task<IActionResult> AllInvestmentOpportunities()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myOpportunityFacade.Queries.AllInvestmentOpportunities();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in AllInvestmentOpportunities.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting opportunities", Data = null });
            }
        }

        /// <summary>
        /// Get all opportunity requests.
        /// </summary>
        [HttpGet("requests")]
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

                var result = await _myOpportunityFacade.Requests.AllRequests();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in GetAllRequests.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Unexpected error occurred while getting financing requests", Data = null });
            }
        }

        /// <summary>
        /// Get all pending opportunity requests.
        /// </summary>
        [HttpGet("requests/pending")]
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

                var result = await _myOpportunityFacade.Requests.PendingRequests();
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
        [HttpGet("requests/accepted")]
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

                var result = await _myOpportunityFacade.Requests.AcceptedRequests();
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
        [HttpGet("requests/rejected")]
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

                var result = await _myOpportunityFacade.Requests.RejectedRequests();
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

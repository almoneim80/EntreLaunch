namespace EntreLaunch.Web.Controllers.LaunchingProjectAPI
{
    [Authorize(Roles = AppRoles.MyOpportunityRoles)]
    [Route("api/[controller]")]
    public class MyOpportunityController(
        IMyOpportunityService myOpportunityFacade,
        ILogger<MyOpportunityController> logger,
        ISubscriptionService subscriptionService,
        ILocalizationManager localization,
        IExtendedBaseService extendedBaseService) : AuthenticatedController(localization)
    {
        private readonly IMyOpportunityService _myOpportunityFacade = myOpportunityFacade;
        private readonly ILogger<MyOpportunityController> _logger = logger;
        private readonly IExtendedBaseService _extendedBaseService = extendedBaseService;
        private readonly ILocalizationManager _localizationManager = localization;
        private readonly ISubscriptionService _subscriptionService = subscriptionService;

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
        [RequiredPermission(MyOpportunityPermissions.SendOpportunityRequest)]
        public async Task<IActionResult> SendOpportunityRequest([FromBody] CreateOpportunityRequestDto request)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var invalidRefCheck = await request.OpportunityId.CheckIfEntityExistsAsync<Opportunity>(_extendedBaseService, _logger, _localizationManager);
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
                return this.UnexpectedError("sending opportunity request");
            }
        }

        /// <summary>
        /// Process requests state (Accepted, Rejected).
        /// </summary>
        [HttpPost("requests/process")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.ProgressRequest)]
        public async Task<IActionResult> ProcessRequest([FromBody] ProcessOpportunityRequestDto processOpportunityRequest)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _myOpportunityFacade.Requests.ProcessRequest(processOpportunityRequest);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in ProgressRequest.");
                return this.UnexpectedError("progress opportunity request");
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
                return this.UnexpectedError("filtering opportunity");
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
        [RequiredPermission(MyOpportunityPermissions.GetOpportunities)]
        public async Task<IActionResult> AllInvestmentOpportunities([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myOpportunityFacade.Queries.AllInvestmentOpportunities(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in AllInvestmentOpportunities.");
                return this.UnexpectedError("getting opportunities");
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
        [RequiredPermission(MyOpportunityPermissions.GetAll)]
        public async Task<IActionResult> GetAllRequests([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myOpportunityFacade.Requests.AllRequests(pagination, cancellationToken);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in GetAllRequests.");
                return this.UnexpectedError("getting financing requests");
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
        [RequiredPermission(MyOpportunityPermissions.GetPending)]
        public async Task<IActionResult> GetPendingRequests([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myOpportunityFacade.Requests.PendingRequests(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetPendingRequests.");
                return this.UnexpectedError("getting financing requests");
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
        [RequiredPermission(MyOpportunityPermissions.GetAccepted)]
        public async Task<IActionResult> GetAcceptedRequests([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myOpportunityFacade.Requests.AcceptedRequests(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAcceptedRequests.");
                return this.UnexpectedError("getting financing requests");
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
        [RequiredPermission(MyOpportunityPermissions.GetRejected)]
        public async Task<IActionResult> GetRejectedRequests([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myOpportunityFacade.Requests.RejectedRequests(pagination, cancellationToken);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetRejectedRequests.");
                return this.UnexpectedError("getting financing requests");
            }
        }

        /// <summary>
        /// Get all opportunity costs.
        /// </summary>
        [HttpGet("filters/costs")]
        [ProducesResponseType(typeof(GeneralResult<List<decimal>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCosts()
        {
            try
            {
                var result = await _myOpportunityFacade.Filters.GetAllCostsAsync();
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllCosts.");
                return this.UnexpectedError("get all opportunity costs");
            }
        }

        /// <summary>
        /// Get all opportunity sectors.
        /// </summary>
        [HttpGet("filters/sectors")]
        [ProducesResponseType(typeof(GeneralResult<List<string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSectors()
        {
            try
            {
                var result = await _myOpportunityFacade.Filters.GetAllSectorsAsync();
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllSectors.");
                return this.UnexpectedError("get all opportunity sectors");
            }
        }

        /// <summary>
        /// check access to opportunity.
        /// </summary>
        [HttpGet("can-access")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GeneralResult<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CanAccess()
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            var result = await _subscriptionService.HasActiveAccessAsync(CurrentUserId!, SubscriptionType.MyOpportunity);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get all financing opportunity requests submitted by the logged-in user.
        /// </summary>
        [HttpGet("my-requests")]
        [ProducesResponseType(typeof(GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(MyOpportunityPermissions.GeMyRequests)]
        public async Task<IActionResult> GetMyFinancingRequests([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myOpportunityFacade.Requests.GetUserRequestsAsync(CurrentUserId!, pagination, cancellationToken);
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetMyFinancingRequests.");
                return this.UnexpectedError("getting user financing requests");
            }
        }

        /// <summary>
        /// Delete a financing request submitted by the current user if it's still pending.
        /// </summary>
        /// <param name="requestId">The ID of the request to be deleted.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// - 200 OK: Request deleted successfully.
        /// - 400 Bad Request: If the request is not found or not deletable.
        /// - 401 Unauthorized: If the user is not authenticated.
        /// - 500 Internal Server Error: Unexpected failure.
        /// </returns>
        [HttpDelete("my-request/delete/{requestId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(MyOpportunityPermissions.DeleteOwnRequest)]
        public async Task<IActionResult> DeleteUserRequest([FromRoute] int requestId, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _myOpportunityFacade.Requests.DeleteUserRequestAsync(requestId, CurrentUserId!, cancellationToken);
                if (result.IsSuccess == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteUserRequest.");
                return this.UnexpectedError("delete user financing request");
            }
        }
    }
}

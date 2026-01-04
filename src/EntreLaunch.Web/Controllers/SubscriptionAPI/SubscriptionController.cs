namespace EntreLaunch.Web.Controllers.SubscriptionAPI
{
    [Authorize(Roles = AppRoles.SubscriptionRoles)]
    [Route("api/{Controller}")]
    [ApiController]
    public class SubscriptionController(
        ISubscriptionService subscriptionService,
        ILocalizationManager localization,
        ILogger<SubscriptionController> logger,
        IExtendedBaseService extendedBaseService) : AuthenticatedController(localization)
    {
        private readonly ILocalizationManager _localization = localization;
        private readonly ILogger<SubscriptionController> _logger = logger;
        private readonly ISubscriptionService _subscriptionService = subscriptionService;

        /// <summary>
        /// Determines whether the authenticated user currently has active access to a specific subscription type and reference ID.
        /// </summary>
        /// <param name="dto">An object containing the subscription type and reference ID to check access against.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns 200 OK with a boolean indicating active access status.
        /// Returns 401 for unauthorized users, or 500 for unexpected errors.
        /// </returns>
        [HttpPost("user/active-access")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.HasActiveAccess)]
        public async Task<IActionResult> HasActiveAccess([FromBody] SubscriptionLookupDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var access = await _subscriptionService.HasActiveAccessAsync(CurrentUserId!, dto.Type, dto.ReferenceId);
                return Ok(access);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while checking subscription access.");
                return StatusCode(500, new GeneralResult
                { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError"), Data = null });
            }
        }

        /// <summary>
        /// Extends the duration of an existing subscription by a specified number of additional days.
        /// </summary>
        /// <param name="dto">An object containing the subscription ID and the number of extra days to extend.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns 200 OK on successful extension.
        /// Returns 400 if input is invalid, 403 for permission issues, 404 if subscription not found, 422 for invalid data, or 500 for internal errors.
        /// </returns>
        [HttpPost("extend")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.ExtendSubscription)]
        public async Task<IActionResult> ExtendSubscription([FromBody] ExtendSubscriptionDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                if (dto.ExtraDays <= 1 || dto.ExtraDays > 365)
                {
                    return BadRequest(new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localization.GetLocalizedString("InvalidExtraDays"),
                        Data = null
                    });
                }

                var result = await _subscriptionService.ExtendSubscriptionAsync(dto.SubscriptionId, TimeSpan.FromDays(dto.ExtraDays));

                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.Unauthorized => Unauthorized(result),
                        ErrorType.PermissionDenied => StatusCode(403, result),
                        ErrorType.InvalidData => UnprocessableEntity(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while extending subscription ID {Id}.", dto.SubscriptionId);
                return StatusCode(500, new GeneralResult
                { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError"), Data = null });
            }
        }

        /// <summary>
        /// Cancels an existing subscription and optionally records the reason for cancellation.
        /// </summary>
        /// <param name="dto">An object containing the subscription ID and an optional reason for cancellation.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns 200 OK on successful cancellation.
        /// Returns appropriate status codes for bad requests, unauthorized access, not found, or server errors.
        /// </returns>
        [HttpPost("cancel")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.CancelSubscription)]
        public async Task<IActionResult> CancelSubscription([FromBody] CancelSubscriptionDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _subscriptionService.CancelSubscriptionAsync(dto.SubscriptionId, dto.Reason ?? "");

                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.Unauthorized => Unauthorized(result),
                        ErrorType.PermissionDenied => StatusCode(403, result),
                        ErrorType.InvalidData => UnprocessableEntity(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while cancelling subscription ID {Id}.", dto.SubscriptionId);
                return StatusCode(500, new GeneralResult
                { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError"), Data = null });
            }
        }

        /// <summary>
        /// Starts a new trial subscription for the current user for a specified type and reference.
        /// </summary>
        /// <param name="dto">An object containing the subscription type and reference ID.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns 200 OK upon successful initiation of the trial.
        /// Possible error responses include 400, 401, 422, or 500 depending on the nature of the issue.
        /// </returns>
        [HttpPost("start-trial")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.StartTrial)]
        public async Task<IActionResult> StartTrialSubscription([FromBody] StartTrialSubscriptionDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _subscriptionService.StartTrialSubscriptionAsync(CurrentUserId!, dto.Type, dto.ReferenceId);

                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.Unauthorized => Unauthorized(result),
                        ErrorType.PermissionDenied => StatusCode(403, result),
                        ErrorType.InvalidData => UnprocessableEntity(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during trial subscription.");
                return StatusCode(500, new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("UnexpectedError"),
                    Data = null
                });
            }
        }

        /// <summary>
        /// Upgrades an existing subscription to a new reference with an additional cost.
        /// </summary>
        /// <param name="dto">An object containing the current subscription ID, new reference ID, and additional price.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns 200 OK with updated subscription details on success.
        /// Returns suitable status codes for various failure scenarios including 400, 403, 404, 422, or 500.
        /// </returns>
        [HttpPost("upgrade")]
        [ProducesResponseType(typeof(GeneralResult<SubscriptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult<SubscriptionDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult<SubscriptionDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult<SubscriptionDto>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GeneralResult<SubscriptionDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult<SubscriptionDto>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GeneralResult<SubscriptionDto>), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.UpgradeSubscription)]
        public async Task<IActionResult> UpgradeSubscription([FromBody] UpgradeSubscriptionDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _subscriptionService.UpgradeSubscriptionAsync(
                    dto.CurrentSubscriptionId,
                    dto.NewReferenceId,
                    dto.AdditionalPrice);

                if (!result.IsSuccess)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InvalidData => UnprocessableEntity(result),
                        ErrorType.PermissionDenied => StatusCode(403, result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while upgrading subscription ID {Id}.", dto.CurrentSubscriptionId);
                return StatusCode(500, new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("UnexpectedError"),
                    Data = null
                });
            }
        }

        /// <summary>
        /// Creates a child subscription under a specified parent subscription for a designated user.
        /// </summary>
        /// <param name="dto">An object containing the parent subscription ID and the child user ID.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns 200 OK on successful creation.
        /// Error codes may include 400, 403, 404, 422, or 500 for corresponding failure reasons.
        /// </returns>
        [HttpPost("child")]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.CreateChildSubscription)]
        public async Task<IActionResult> CreateChildSubscription([FromBody] ChildSubscriptionCreateDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _subscriptionService.CreateChildSubscriptionAsync(dto.ParentSubscriptionId, dto.ChildUserId);

                if (result.IsSuccess == false)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InvalidData => UnprocessableEntity(result),
                        ErrorType.PermissionDenied => StatusCode(403, result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating child subscription from parent {ParentId}.", dto.ParentSubscriptionId);
                return StatusCode(500, new GeneralResult
                { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError"), Data = null });
            }
        }

        /// <summary>
        /// Retrieves the current active subscription details for a user based on type and reference ID.
        /// </summary>
        /// <param name="dto">An object specifying the subscription type and reference ID.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns 200 OK with subscription data if found.
        /// Returns appropriate errors for unauthorized access, not found records, or invalid input.
        /// </returns>
        [HttpGet("single/scription")]
        [ProducesResponseType(typeof(GeneralResult<SubscriptionDto?>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.GetUserSubscription)]
        public async Task<IActionResult> GetUserSubscription([FromQuery] SubscriptionLookupDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _subscriptionService.GetUserSubscriptionAsync(CurrentUserId!, dto.Type, dto.ReferenceId);
                return result.ErrorType switch
                {
                    ErrorType.BadRequest => BadRequest(result),
                    ErrorType.NotFound => NotFound(result),
                    ErrorType.Unauthorized => Unauthorized(result),
                    ErrorType.PermissionDenied => StatusCode(403, result),
                    ErrorType.InvalidData => UnprocessableEntity(result),
                    ErrorType.InternalServerError => StatusCode(500, result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving user subscription.");
                return StatusCode(500, new GeneralResult
                    { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError"), Data = null });
            }
        }

        /// <summary>
        /// Retrieves a list of all subscriptions associated with the currently authenticated user.
        /// </summary>
        [HttpGet("all/scriptions")]
        [ProducesResponseType(typeof(GeneralResult<PaginatedResult<SubscriptionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.GetUserSubscriptions)]
        public async Task<IActionResult> GetUserSubscriptions([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _subscriptionService.GetUserSubscriptionsAsync(CurrentUserId!, pagination, cancellationToken);

                if (!result.IsSuccess)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.Unauthorized => Unauthorized(result),
                        ErrorType.PermissionDenied => StatusCode(403, result),
                        ErrorType.InvalidData => UnprocessableEntity(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving user subscriptions.");
                return StatusCode(500, new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("UnexpectedError"),
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves a list of subscriptions filtered by a specific status.
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(typeof(GeneralResult<PaginatedResult<SubscriptionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.GetSubscriptionsByStatus)]
        public async Task<IActionResult> GetSubscriptionsByStatus([FromQuery] SubscriptionStatus status, [FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _subscriptionService.GetSubscriptionsByStatusAsync(status, pagination, cancellationToken);

                if (!result.IsSuccess)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.Unauthorized => Unauthorized(result),
                        ErrorType.PermissionDenied => StatusCode(403, result),
                        ErrorType.InvalidData => UnprocessableEntity(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving subscriptions by status.");
                return StatusCode(500, new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("UnexpectedError"),
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves a list of subscriptions that are expiring within a specified number of days.
        /// </summary>
        /// <param name="days">The number of upcoming days to check for expiring subscriptions. Must be between 1 and 90.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns 200 OK with a list of subscriptions expiring soon.
        /// Returns 400 if days parameter is invalid, 401 for unauthorized access, or 500 for internal errors.
        /// </returns>
        [HttpGet("expiring-soon")]
        [ProducesResponseType(typeof(List<SubscriptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.GetExpiringSoon)]
        public async Task<IActionResult> GetExpiringSoon([FromQuery] int days = 3, CancellationToken cancellationToken = default)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                if (days <= 0 || days > 90)
                {
                    return BadRequest(new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localization.GetLocalizedString("InvalidPeriod"),
                        Data = null
                    });
                }

                var result = await _subscriptionService.GetExpiringSoonAsync(TimeSpan.FromDays(days));
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving expiring subscriptions.");
                return StatusCode(500, new GeneralResult
                { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError"), Data = null });
            }
        }

        /// <summary>
        /// Retrieves statistical data for subscriptions, optionally starting from a specific date.
        /// </summary>
        /// <param name="fromDate">Optional. The start date from which to calculate statistics. If not specified, includes all available data.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns 200 OK with subscription statistics.
        /// Returns 500 if an internal server error occurs.
        /// </returns>
        [HttpGet("stats/subscription")]
        [ProducesResponseType(typeof(SubscriptionStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.GetSubscriptionStatistics)]
        public async Task<IActionResult> GetSubscriptionStatistics([FromQuery] DateTimeOffset? fromDate, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _subscriptionService.GetSubscriptionStatisticsAsync(fromDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving subscription statistics.");
                return StatusCode(500, new GeneralResult
                { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedError"), Data = null });
            }
        }

        /// <summary>
        /// Returns the enumeration values for available subscription types. Requires appropriate permissions.
        /// </summary>
        /// <returns>
        /// Returns 201 Created with a list of enum values representing subscription types.
        /// Returns 400, 401, or 500 in case of access issues or failures.
        /// </returns>
        [HttpGet("types")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.GetSubscriptionType)]
        public ActionResult<IEnumerable<EnumData>> GetSubscriptionType()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var enumValues = extendedBaseService.GetEnumValues<SubscriptionType>();
                return Ok(enumValues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the subscription GetSubscriptionType method.");
                return this.UnexpectedError("getting subscription types.");
            }
        }

        /// <summary>
        /// Returns the enumeration values for subscription statuses. Requires appropriate permissions.
        /// </summary>
        /// <returns>
        /// Returns 201 Created with a list of enum values for subscription statuses.
        /// Returns error responses for access violations or internal errors.
        /// </returns>
        [HttpGet("statuses")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(SubscriptionPermissions.GetSubscriptionStatus)]
        public ActionResult<IEnumerable<EnumData>> GetSubscriptionStatus()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var enumValues = extendedBaseService.GetEnumValues<SubscriptionStatus>();
                return Ok(enumValues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the subscription GetSubscriptionStatus method.");
                return this.UnexpectedError("getting subscription statuses.");
            }
        }
    }
}

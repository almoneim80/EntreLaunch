namespace EntreLaunch.Web.Controllers.PurchaseAPI
{
    [Authorize(Roles = AppRoles.PurchaseRoles)]
    [Route("api/permissions")]
    public class PurchaseController(
        IPurchaseService purchaseService,
        ILocalizationManager localization,
        ILogger<PurchaseController> logger,
        IExtendedBaseService extendedBaseService) : AuthenticatedController(localization)
    {
        private readonly IPurchaseService _purchaseService = purchaseService;
        private readonly ILocalizationManager _localization = localization;
        private readonly ILogger<PurchaseController> _logger = logger;

        /// <summary>
        /// Retrieves a list of purchases made by the current authenticated user, optionally filtered by item type.
        /// </summary>
        /// <param name="type">Optional. Specifies the type of purchase items to filter by (e.g., subscription, product).</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns a 200 OK with a list of purchase details on success.
        /// Returns appropriate error codes and messages for unauthorized access, invalid requests, or internal failures.
        /// </returns>
        [HttpGet("all/purchases")]
        [ProducesResponseType(typeof(GeneralResult<List<PurchaseDetailsDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(PurchasePermissions.GetUserPurchases)]
        public async Task<IActionResult> GetUserPurchases([FromQuery] PurchaseItemType? type, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _purchaseService.GetUserPurchasesAsync(CurrentUserId!, type);

                if (!result.IsSuccess)
                {
                    return result.ErrorType switch
                    {
                        ErrorType.BadRequest => BadRequest(result),
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.Unauthorized => Unauthorized(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving purchases for user {UserId}.", CurrentUserId);
                return StatusCode(500, new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("UnexpectedError"),
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves detailed information for a specific purchase by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the purchase to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns a 200 OK with purchase details if found.
        /// May return 404 if the purchase is not found, 403 for permission issues, or 500 for internal server errors.
        /// </returns>
        [HttpGet("single/purchase/{id}")]
        [ProducesResponseType(typeof(GeneralResult<PurchaseDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(PurchasePermissions.GetPurchaseById)]
        public async Task<IActionResult> GetPurchaseById([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _purchaseService.GetByIdAsync(id);

                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(result),
                    ErrorType.PermissionDenied => StatusCode(403, result),
                    ErrorType.InternalServerError => StatusCode(500, result),
                    _ => Ok(result)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving purchase ID {Id}.", id);
                return StatusCode(500, new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("UnexpectedError"),
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves aggregated statistical data related to purchases of a specific item type and reference ID.
        /// </summary>
        /// <param name="itemType">Specifies the type of items for which statistics are required.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <param name="referenceId">The reference identifier of the item to analyze. Must be greater than zero.</param>
        /// <returns>
        /// Returns a 200 OK with statistics data if available.
        /// Returns 400 if the reference ID is invalid, or appropriate error responses for not found, permission issues, or internal errors.
        /// </returns>
        [HttpGet("stats/purchase")]
        [ProducesResponseType(typeof(GeneralResult<PurchaseStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(PurchasePermissions.GetPurchaseStats)]
        public async Task<IActionResult> GetPurchaseStats([FromQuery] PurchaseItemType itemType, CancellationToken cancellationToken, [FromQuery] int referenceId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _purchaseService.GetPurchaseStatsAsync(itemType, referenceId);

                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(result),
                    ErrorType.PermissionDenied => StatusCode(403, result),
                    ErrorType.InternalServerError => StatusCode(500, result),
                    _ => Ok(result)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving stats for item {ItemType} and ID {RefId}.", itemType, referenceId);
                return StatusCode(500, new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("UnexpectedError"),
                    Data = null
                });
            }
        }

        /// <summary>
        /// Checks whether the current authenticated user has previously purchased a specific item.
        /// </summary>
        /// <param name="dto">An object containing the item type and reference ID to look up the purchase.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>
        /// Returns a 200 OK with a boolean indicating purchase status.
        /// Returns 400 for invalid input, 401 for unauthorized access, or 500 for server errors.
        /// </returns>
        [HttpPost("has-purchased")]
        [ProducesResponseType(typeof(GeneralResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GeneralResult), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(PurchasePermissions.HasUserPurchased)]
        public async Task<IActionResult> HasUserPurchased([FromBody] PurchaseLookupDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var modelCheck = this.ValidateModelState(_logger);
                if (modelCheck != null) return modelCheck;

                var result = await _purchaseService.HasUserPurchasedAsync(CurrentUserId!, dto.ItemType, dto.ReferenceId);

                return result.IsSuccess
                    ? Ok(result)
                    : result.ErrorType switch
                    {
                        ErrorType.NotFound => NotFound(result),
                        ErrorType.InternalServerError => StatusCode(500, result),
                        _ => BadRequest(result)
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error checking user purchase for type {Type}, ref {RefId}", dto.ItemType, dto.ReferenceId);
                return StatusCode(500, new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("UnexpectedError"),
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves the list of enumeration values for different types of purchase items.
        /// This endpoint requires specific permissions to access the data.
        /// </summary>
        /// <returns>
        /// Returns 201 Created with a list of enum values representing the types of purchase items.
        /// Returns 401 if the user is not authenticated, 400 for invalid requests, or 500 for internal server errors.
        /// </returns>
        [HttpGet("types/purchase")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(PurchasePermissions.GetPurchasesType)]
        public ActionResult<IEnumerable<EnumData>> GetPurchasesType()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var enumValues = extendedBaseService.GetEnumValues<PurchaseItemType>();
                return Ok(enumValues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the purchases GetPurchasesType method.");
                return this.UnexpectedError("getting purchases types.");
            }
        }
    }
}

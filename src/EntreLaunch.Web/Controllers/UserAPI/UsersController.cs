namespace EntreLaunch.Web.Controllers.UserAPI
{
    [Authorize(Roles = AppRoles.UserRoles)]
    [Route("api/[controller]")]
    public class UsersController(
        IMapper mapper,
        UserManager<User> userManager,
        ILogger<UsersController> logger,
        ILocalizationManager localization,
        CascadeDeleteService deleteService,
        IUserService userService,
        IUserProfileService userProfileService) : AuthenticatedController(localization)
    {
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<UsersController> _logger = logger;
        private readonly ILocalizationManager? _localization = localization;
        private readonly CascadeDeleteService _deleteService = deleteService;
        private readonly IUserService _userService = userService;
        private readonly IUserProfileService _userProfileService = userProfileService;

        /// <summary>
        /// Complete an existing user's data.
        /// </summary>
        [HttpPatch("complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(UserPermissions.Complete)]
        public async Task<IActionResult> Complete([FromBody] CompleteUserDetailsDto value)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                // Get the current authenticated user
                var user = await UserHelper.GetCurrentUserOrThrowAsync(_userManager, User);
                if (user == null)
                {
                    _logger.LogError(_localization!.GetLocalizedString("UserNotFound"));
                    return BadRequest(new GeneralResult(false, _localization!.GetLocalizedString("UserNotFound"), null));
                }

                // Update the user's data
                var result = await _userService.CompleteUserAsync(user, value);
                if(result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing user details for the current user.");
                return StatusCode(500, new GeneralResult(false, "Error completing user details for the current user.", null));
            }
        }

        /// <summary>
        /// Update current authenticated user's details.
        /// </summary>
        [HttpPatch("edit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(UserPermissions.Edit)]
        public async Task<IActionResult> Patch([FromBody] UserUpdateDto dto)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                // Get the current authenticated user
                var user = await UserHelper.GetCurrentUserOrThrowAsync(_userManager, User);
                if (user == null)
                {
                    _logger.LogError("User not found.");
                    return BadRequest(new GeneralResult(false, _localization!.GetLocalizedString("UserNotFound"), null));
                }

                // Update the user
                _mapper.Map(dto, user);
                var result = await _userService.UpdateUserAsync(user, dto);
                if(result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user details for the current user.");
                return StatusCode(500, new GeneralResult(false, "Error updating user details for the current user.", null));
            }
        }

        /// <summary>
        /// Retrieve a list of all users.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(UserPermissions.GetAll)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _userService.GetAllUsersAsync(pagination, cancellationToken);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users.");
                return StatusCode(500, new GeneralResult(false, "Error retrieving all users.", null));
            }
        }

        /// <summary>
        /// Retrieve a specific user's data.
        /// </summary>
        [HttpGet("get-one/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(UserPermissions.GetOne)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] string id)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var existingEntity = await _userManager.Users.FirstOrDefaultAsync(usr => usr.Id == id && !usr.IsDeleted);
                if (existingEntity == null)
                {
                    _logger.LogError("User {Id} not found.", id);
                    return BadRequest(new GeneralResult(false, _localization!.GetLocalizedString("UserNotFound"), null));
                }

                return Ok(new GeneralResult(true, "User data retrieved successfully.", _mapper.Map<UserDetailsDto>(existingEntity)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {Id}.", id);
                return StatusCode(500, new GeneralResult(false, "Error retrieving user.", null));
            }
        }

        /// <summary>
        /// Returns current user details.
        /// </summary>
        [HttpGet("self")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(UserPermissions.GetMe)]
        public async Task<IActionResult> GetSelf()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var user = await UserHelper.GetCurrentUserOrThrowAsync(_userManager, User);
                if (user == null)
                {
                    _logger.LogError("User not found for the current user.");
                    return BadRequest(new GeneralResult(false, _localization!.GetLocalizedString("UserNotFound"), null));
                }
                return Ok(new GeneralResult(true, "Your data retrieved successfully.", _mapper.Map<UserDetailsDto>(user)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user details for the current user.");
                return StatusCode(500, new GeneralResult(false, "An unexpected error occurred while retrieving your details.", null));
            }
        }

        /// <summary>
        /// Get the full profile for the logged-in user.
        /// </summary>
        [HttpGet("full-profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(UserPermissions.MyProfile)]
        public async Task<ActionResult<GeneralResult<UserFullProfileDto>>> GetFullProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new GeneralResult { IsSuccess = false, Message = "User not logged in." });
                }

                var result = await _userProfileService.GetFullProfileAsync(userId);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching full profile for user.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GeneralResult
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred while fetching profile."
                });
            }
        }

        /// <summary>
        /// Soft deletes an entity and its related entities with cascading soft delete.
        /// </summary>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(UserPermissions.Delete)]
        public async Task<IActionResult> DeleteWithCascade()
        {
            var userCheck = CheckUserOrUnauthorized();
            if (userCheck != null) return userCheck;

            // Get the current authenticated user
            var user = await UserHelper.GetCurrentUserOrThrowAsync(_userManager, User);
            if (user == null)
            {
                _logger.LogError(_localization!.GetLocalizedString("UserNotFound"));
                return BadRequest(new GeneralResult(false, _localization!.GetLocalizedString("UserNotFound"), null));
            }

            var transactionId = Guid.NewGuid(); // Generate a unique transaction ID for logging.
            _logger.LogInformation("Transaction {TransactionId}: Starting soft delete for entity ID {Id}.", transactionId, user!.Id.ToString());
            try
            {
                var result = await _deleteService.SoftDeleteUserCascadeAsync(user.Id);
                if (!result)
                {
                    _logger.LogWarning("Transaction {TransactionId}: Entity with ID {Id} not found or already deleted.", transactionId, user!.Id.ToString());
                    return BadRequest(new GeneralResult(false, "Entity not found or already deleted.", null));
                }

                _logger.LogInformation("Transaction {TransactionId}: Successfully soft deleted entity ID {Id}.", transactionId, user!.Id.ToString());
                return Ok(new GeneralResult(true, "Entity soft deleted successfully.", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction {TransactionId}: Unexpected error occurred while deleting entity ID {Id}.", transactionId, user!.Id.ToString());
                return StatusCode(500, new GeneralResult(false, "An unexpected error occurred while deleting your account.", null));
            }
        }

        ///// <summary>
        ///// Resend an OTP to a user's phone number.
        ///// </summary>
        //[HttpPost]
        //[Route("resend-otp")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResendOtp([FromBody] OtpResendDto dto)
        //{
        //    try
        //    {
        //        await _userService.ResendOtpAsync(dto.UserId, dto.PhoneNumber);
        //        return Ok(new { message = "OTP resent successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "Error resending OTP for user {UserId}", dto.UserId);
        //        return StatusCode(500, new { message = "An error occurred while resending the OTP." });
        //    }
        //}
    }
}

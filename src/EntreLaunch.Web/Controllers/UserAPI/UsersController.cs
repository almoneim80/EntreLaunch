namespace EntreLaunch.Web.Controllers.UserAPI
{
    [Route("api/[controller]")]
    public class UsersController : AuthenticatedController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UsersController> _logger;
        private readonly ILocalizationManager? _localization;
        private readonly CascadeDeleteService _deleteService;
        private readonly IUserService _userService;
        public UsersController(
            IMapper mapper,
            UserManager<User> userManager,
            ILogger<UsersController> logger,
            ILocalizationManager? localization,
            CascadeDeleteService deleteService,
            IUserService userService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
            _localization = localization;
            _deleteService = deleteService;
            _userService = userService;
        }

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

                // EntreLaunchdate the user's data
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
        /// EntreLaunchdate current authenticated user's details.
        /// </summary>
        [HttpPatch("edit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [RequiredPermission(UserPermissions.Edit)]
        public async Task<IActionResult> Patch([FromBody] UserEntreLaunchdateDto dto)
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

                // EntreLaunchdate the user
                _mapper.Map(dto, user);
                var result = await _userService.EntreLaunchdateUserAsync(user, dto);
                if(result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error EntreLaunchdating user details for the current user.");
                return StatusCode(500, new GeneralResult(false, "Error EntreLaunchdating user details for the current user.", null));
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
        [RequiredPermission(UserPermissions.ShowAll)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var allUsers = await _userManager.Users.Where(usr => !usr.IsDeleted).ToListAsync();
                if (allUsers.Count == 0)
                {
                    _logger.LogError("No users found.");
                    return BadRequest(new GeneralResult(false, _localization!.GetLocalizedString("NoUsersFound"), null));
                }

                var resultsToClient = _mapper.Map<UserDetailsDto[]>(allUsers).ToArray();
                return Ok(new GeneralResult(true, "All users data retrieved successfully.", resultsToClient));
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
        [RequiredPermission(UserPermissions.ShowOne)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByIdAsync(string id)
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
        [RequiredPermission(UserPermissions.ShowMe)]
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

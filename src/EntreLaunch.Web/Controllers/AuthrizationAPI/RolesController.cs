namespace EntreLaunch.Web.Controllers.AuthrizationAPI
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class RolesController(
        IRoleService roleService,
        UserManager<User> userManager,
        ILocalizationManager localization,
        ILogger<RolesController> logger) : AuthenticatedController(localization)
    {
        private readonly IRoleService _roleService = roleService;
        protected readonly UserManager<User> _userManager = userManager;
        private readonly ILocalizationManager _localization = localization;
        private readonly ILogger<RolesController> _logger = logger;

        /// <summary>
        /// Ensures that all default system roles are created if they do not already exist.
        /// Requires the user to have permission to ensure default roles.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// </returns>
        [HttpPost("default")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.Default)]
        public async Task<IActionResult> EnsureDefaultRoles()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _roleService.EnsureSeedRolesAsync();
                if(result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("Default roles ensured successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring default roles.");
                return this.UnexpectedError("ensuring default roles.");
            }
        }

        /// <summary>
        /// Assigns a role to a user by user ID.
        /// Requires the user to have permission to assign roles.
        /// </summary>
        /// <param name="input">
        /// An <see cref="AssignRoleDto"/> containing the user ID and role name.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the assignment.
        /// </returns>
        [HttpPost("assign")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.Assign)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto input)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _roleService.AssignRoleAsync(input.UserId, input.Role);
                if(result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("RolesController - AssignRole : Role {RoleName} assigned to user {UserId} successfully.", input.Role, input.UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RolesController - AssignRole : Error assigning role {RoleName} to user {UserId}.", input.Role, input.UserId);
                return this.UnexpectedError("assigning role.");
            }
        }

        /// <summary>
        /// Assigns a role to a user by their email address.
        /// Requires the user to have permission to assign roles by email.
        /// </summary>
        /// <param name="input">
        /// An <see cref="AssignRoleByEmailDto"/> containing the user's email and role name.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the assignment.
        /// </returns>
        [HttpPost("assign-by-email")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.AssignByEmail)]
        public async Task<IActionResult> AssignRoleByEmail([FromBody] AssignRoleByEmailDto input)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                // Find the user directly with UserManager
                var user = await _userManager.FindByEmailAsync(input.Email);
                if (user == null)
                {
                    return NotFound(new GeneralResult { IsSuccess = false, Message = "User not found.", Data = null });
                }

                var result = await _roleService.AssignRoleAsync(user.Id, input.Role);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("RolesController - AssignRoleByEmail : Role {RoleName} assigned to user {Email} successfully.", input.Role, user.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RolesController - AssignRoleByEmail : Error assigning role {RoleName} to user {Email}.", input.Role, input.Email);
                return this.UnexpectedError("assigning role.");
            }
        }

        /// <summary>
        /// Removes a role from a user by user ID.
        /// Requires the user to have permission to remove roles.
        /// </summary>
        /// <param name="input">
        /// A <see cref="DeleteRoleFromUserDto"/> containing the user ID and the role name to remove.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the removal.
        /// </returns>
        [HttpPost("remove-role")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.Remove)]
        public async Task<IActionResult> RemoveRole([FromBody] DeleteRoleFromUserDto input)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _roleService.RemoveRoleAsync(input.UserId, input.Role);
                if(result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("RolesController - RemoveRole : Role {RoleName} removed from user {UserId} successfully.", input.Role, input.UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RolesController - RemoveRole : Error removing role {RoleName} from user {UserId}.", input.Role, input.UserId);
                return this.UnexpectedError("removing role.");
            }
        }

        /// <summary>
        /// Checks if a role with a given name exists in the system.
        /// Requires the user to have permission to check role existence.
        /// </summary>
        /// <param name="roleName">
        /// The name of the role to check.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating whether the role exists.
        /// </returns>
        [HttpGet("exists")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.Exists)]
        public async Task<IActionResult> RoleExists([FromQuery] string roleName)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _roleService.RoleExistsAsync(roleName);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("RolesController - RoleExists : Role {RoleName} exists: {Exists}.", roleName, result.Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking role existence for {RoleName}.", roleName);
                return this.UnexpectedError("checking role existence.");
            }
        }

        /// <summary>
        /// Retrieves all users assigned to a specific role.
        /// Requires the user to have permission to retrieve users in a role.
        /// </summary>
        /// <param name="roleName">
        /// The name of the role whose users are to be retrieved.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of users assigned to the role.
        /// </returns>
        [HttpGet("get-by-role")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.UsersInRole)]
        public async Task<IActionResult> GetUsersInRole([FromQuery] string roleName)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _roleService.GetUsersInRoleAsync(roleName);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("RolesController - GetUsersInRole : Users in role {RoleName} retrieved successfully.", roleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users in role {RoleName}.", roleName);
                return this.UnexpectedError("retrieving users in role.");
            }
        }

        /// <summary>
        /// Checks if a specific user is assigned to a given role.
        /// Requires the user to have permission to verify user-role assignments.
        /// </summary>
        /// <param name="userId">
        /// The ID of the user.
        /// </param>
        /// <param name="roleName">
        /// The name of the role.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating if the user is assigned to the role.
        /// </returns>
        [HttpGet("has-role")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.UsersInRole)]
        public async Task<IActionResult> IsUserInRole([FromQuery] string userId, [FromQuery] string roleName)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _roleService.IsUserInRoleAsync(userId, roleName);
                if (!result.IsSuccess)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("RolesController - IsUserInRole : User {UserId} is in role {RoleName}: {IsInRole}.", userId, roleName, result.Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RolesController - IsUserInRole : Error checking if user {UserId} is in role {RoleName}.", userId, roleName);
                return this.UnexpectedError("checking if user is in role.");
            }
        }

        /// <summary>
        /// Retrieves all roles available in the system.
        /// Requires the user to have permission to view all roles.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of all roles.
        /// </returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.GetAll)]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _roleService.GetAllRolesAsync();
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("RolesController - GetAllRoles : All roles retrieved successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all roles.");
                return this.UnexpectedError("retrieving all roles.");
            }
        }
    }
}

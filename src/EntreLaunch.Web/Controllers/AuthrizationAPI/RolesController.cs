namespace EntreLaunch.Web.Controllers.AuthrizationAPI
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController : AuthenticatedController
    {
        private readonly IRoleService _roleService;
        protected readonly UserManager<User> _userManager;
        private readonly ILocalizationManager? _localization;
        private readonly ILogger<RolesController> _logger;
        public RolesController(
            IRoleService roleService,
            UserManager<User> userManager,
            ILocalizationManager? localization,
            ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _userManager = userManager;
            _localization = localization;
            _logger = logger;
        }

        /// <summary>
        /// Ensures default roles exist in the system.
        /// </summary>
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = _localization!.GetLocalizedString("ErrorEnsuringDefaultRoles"), Data = null });
            }
        }

        /// <summary>
        /// Adds a new role to the system.
        /// </summary>
        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.Create)]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _roleService.AddRoleAsync(roleName);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("RolesController - AddRole : Role {RoleName} added successfully.", roleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RolesController - AddRole : Error adding role {RoleName}.", roleName);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error adding role.", Data = null });
            }
        }

        /// <summary>
        /// Assigns a role to a user by ID.
        /// </summary>
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error assigning role.", Data = null });
            }
        }

        /// <summary>
        /// Assigns a role to a user by email.
        /// </summary>
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error assigning role.", Data = null });
            }
        }

        /// <summary>
        /// Removes a role from a user by ID.
        /// </summary>
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error removing role.", Data = null });
            }
        }

        /// <summary>
        /// EntreLaunchdates the name of a role.
        /// </summary>
        [HttpPatch("EntreLaunchdate-name")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.EntreLaunchdate)]
        public async Task<IActionResult> EntreLaunchdateRoleName([FromBody] EntreLaunchdateRoleDto input)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _roleService.EntreLaunchdateRoleNameAsync(input.OldRoleName, input.NewRoleName);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("RolesController - EntreLaunchdateRoleName : Role {OldRoleName} EntreLaunchdated to {NewRoleName} successfully.", input.OldRoleName, input.NewRoleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RolesController - EntreLaunchdateRoleName : Error EntreLaunchdating role {OldRoleName} to {NewRoleName}.", input.OldRoleName, input.NewRoleName);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error EntreLaunchdating role.", Data = null });
            }
        }

        /// <summary>
        /// Checks if a role exists in the system.
        /// </summary>
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error checking role existence.", Data = null });
            }
        }

        /// <summary>
        /// Retrieves all users assigned to a specific role.
        /// </summary>
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error retrieving users in role.", Data = null });
            }
        }

        /// <summary>
        /// Checks if a specific user is assigned to a role.
        /// </summary>
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error checking if user is in role.", Data = null });
            }
        }

        /// <summary>
        /// Retrieves all roles in the system.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.ShowAll)]
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
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error retrieving all roles.", Data = null });
            }
        }

        /// <summary>
        /// Deletes a role from the system.
        /// </summary>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(RolePermissions.Delete)]
        public async Task<IActionResult> DeleteRole([FromQuery] string roleName)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _roleService.DeleteRoleAsync(roleName);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("RolesController - DeleteRole : Role {RoleName} deleted successfully.", roleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleName}.", roleName);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error deleting role.", Data = null });
            }
        }
    }
}

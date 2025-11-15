namespace EntreLaunch.Web.Controllers.AuthrizationAPI
{
    [Authorize(Roles = "Admin")]
    [Route("api/permissions")]
    [ApiController]
    public class PermissionsController : AuthenticatedController
    {
        private readonly IPermissionService _permissionService;
        private readonly UserManager<User> _userManager;
        private readonly ILocalizationManager? _localization;
        private readonly ILogger<PermissionsController> _logger;
        public PermissionsController(
            IPermissionService permissionService,
            UserManager<User> userManager,
            ILocalizationManager? localization,
            ILogger<PermissionsController> logger)
        {
            _permissionService = permissionService;
            _userManager = userManager;
            _localization = localization;
            _logger = logger;
        }

        /// <summary>
        /// Adding a claim to the role.
        /// </summary>
        [HttpPost("add-one")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.Create)]
        public async Task<IActionResult> AddPermissionToRole(string roleName, [FromBody] string permission)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _permissionService.AddPermissionToRoleAsync(roleName, permission);
                if(result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("Successfully added permission {Permission} to role {RoleName}.", permission, roleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding permission {Permission} to role {RoleName}.", permission, roleName);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error adding permission.", Data = null });
            }
        }

        /// <summary>
        /// Adding claims to the role.
        /// </summary>
        [HttpPost("add-multiple")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.Create)]
        public async Task<IActionResult> AddPermissionsToRole(string roleName, [FromBody] List<string> permissions)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _permissionService.AddPermissionsToRoleAsync(roleName, permissions);
                if(result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("Successfully added multiple permissions to role {RoleName}.", roleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding multiple permissions to role {RoleName}.", roleName);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error adding multiple permissions.", Data = null });
            }
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.All)]
        public IActionResult GetAllPermissions()
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var permissions = new List<string>();
                var permissionClasses = typeof(Permissions).GetNestedTypes()
                    .Where(t => t.IsClass && t.IsAbstract && t.IsSealed);

                foreach (var pc in permissionClasses)
                {
                    var fields = pc.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                   .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

                    foreach (var f in fields)
                    {
                        var value = f.GetRawConstantValue() as string;
                        if (!string.IsNullOrEmpty(value))
                        {
                            permissions.Add(value);
                        }
                    }
                }

                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all permissions.");
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error retrieving all permissions.", Data = null });
            }
        }

        /// <summary>
        /// Retrieve role claims.
        /// </summary>
        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.ShowByRole)]
        public async Task<IActionResult> GetPermissionsForRole(string roleName)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _permissionService.GetPermissionsForRoleAsync(roleName);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("Successfully retrieved permissions for role {RoleName}.", roleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions for role {RoleName}.", roleName);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error retrieving permissions.", Data = null });
            }
        }

        /// <summary>
        /// Fetch User Claims.
        /// </summary>
        [HttpGet("by-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.ShowByUser)]
        public async Task<IActionResult> GetPermissionsForUser(string userId)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _permissionService.GetPermissionsForUserAsync(userId);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("Successfully retrieved permissions for user {UserId}.", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions for user {UserId}.", userId);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error retrieving permissions.", Data = null });
            }
        }

        /// <summary>
        /// Check if the user has a claim.
        /// </summary>
        [HttpGet("check")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.CheckUserPermission)]
        public async Task<IActionResult> CheckUserPermission(string userId, [FromQuery] string permission)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _permissionService.UserHasPermissionAsync(userId, permission);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("Permission check for user {UserId} with permission {Permission}: {Result}", userId, permission, result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission} for user {UserId}.", permission, userId);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error checking permission.", Data = null });
            }
        }

        /// <summary>
        /// Deleting a claim from a role.
        /// </summary>
        [HttpDelete("remove")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(PermissionOfRolePermissions.Delete)]
        public async Task<IActionResult> RemovePermissionFromRole(string roleName, [FromBody] string permission)
        {
            try
            {
                var userCheck = CheckUserOrUnauthorized();
                if (userCheck != null) return userCheck;

                var result = await _permissionService.RemovePermissionFromRoleAsync(roleName, permission);
                if (result.IsSuccess == false)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("Successfully removed permission {Permission} from role {RoleName}.", permission, roleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing permission {Permission} from role {RoleName}.", permission, roleName);
                return StatusCode(500, new GeneralResult { IsSuccess = false, Message = "Error removing permission.", Data = null });
            }
        }
    }
}

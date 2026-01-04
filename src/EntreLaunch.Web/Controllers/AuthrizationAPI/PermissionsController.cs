namespace EntreLaunch.Web.Controllers.AuthrizationAPI
{
    [Authorize(Roles = AppRoles.SuperAdmin)]
    [Route("api/permissions")]
    [ApiController]
    public class PermissionsController(
        IPermissionService permissionService,
        ILocalizationManager localization,
        ILogger<PermissionsController> logger) : AuthenticatedController(localization)
    {
        private readonly IPermissionService _permissionService = permissionService;
        private readonly ILogger<PermissionsController> _logger = logger;

        /// <summary>
        /// Adds a single permission claim to a specified role.
        /// Requires create permission.
        /// </summary>
        /// <param name="roleName">The name of the role to modify.</param>
        /// <param name="permission">The permission to add.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result.</returns>
        [HttpPost("add-one/{roleName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.Create)]
        public async Task<IActionResult> AddPermissionToRole([FromRoute] string roleName, [FromBody] string permission)
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
                return this.UnexpectedError("adding permission.");
            }
        }

        /// <summary>
        /// Adds multiple permission claims to a specified role.
        /// Requires create permission.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="permissions">The list of permissions to add.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result.</returns>
        [HttpPost("add-multiple/{roleName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.Create)]
        public async Task<IActionResult> AddPermissionsToRole([FromRoute] string roleName, [FromBody] List<string> permissions)
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
                return this.UnexpectedError("adding multiple permissions.");
            }
        }

        /// <summary>
        /// Retrieves all defined permissions in the system.
        /// Requires permission to view all permissions.
        /// </summary>
        /// <returns>A list of all permissions.</returns>
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
                return this.UnexpectedError("retrieving all permissions.");
            }
        }

        /// <summary>
        /// Retrieves all permissions assigned to a specific role.
        /// Requires permission to view permissions by role.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>A list of permissions associated with the role.</returns>
        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.GetByRole)]
        public async Task<IActionResult> GetPermissionsForRole([FromQuery] string roleName)
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
                return this.UnexpectedError("retrieving permissions.");
            }
        }

        /// <summary>
        /// Retrieves all permissions assigned to a specific user.
        /// Requires permission to view permissions by user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of permissions associated with the user.</returns>
        [HttpGet("by-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.GetByUser)]
        public async Task<IActionResult> GetPermissionsForUser([FromQuery] string userId)
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
                return this.UnexpectedError("retrieving permissions.");
            }
        }

        /// <summary>
        /// Checks whether a user has a specific permission.
        /// Requires check permission.
        /// </summary>
        /// <param name="userId">The ID of the user to check.</param>
        /// <param name="permission">The permission to verify.</param>
        /// <returns>True if the user has the permission; otherwise, false.</returns>
        [HttpGet("check")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequiredPermission(PermissionOfRolePermissions.CheckUserPermission)]
        public async Task<IActionResult> CheckUserPermission([FromQuery] string userId, [FromQuery] string permission)
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
                return this.UnexpectedError("checking permission.");
            }
        }

        /// <summary>
        /// Removes a specific permission claim from a specified role.
        /// Requires delete permission.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <param name="permission">The permission to remove.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result.</returns>
        [HttpDelete("remove")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequiredPermission(PermissionOfRolePermissions.Delete)]
        public async Task<IActionResult> RemovePermissionFromRole([FromRoute] string roleName, [FromBody] string permission)
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
                return this.UnexpectedError("removing permission.");
            }
        }
    }
}

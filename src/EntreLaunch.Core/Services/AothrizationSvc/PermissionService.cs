namespace EntreLaunch.Services.AothrizationSvc
{
    public class PermissionService : IPermissionService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<PermissionService> _logger;
        public PermissionService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ILogger<PermissionService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        //// <inheritdoc />
        public async Task<GeneralResult> AddPermissionToRoleAsync(string roleName, string permission)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(permission))
                {
                    _logger.LogError("PermissionService - AddPermissionToRoleAsync : Permission cannot be null or empty.");
                    return new GeneralResult(false, "Permission cannot be null or empty.");
                }

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogWarning("PermissionService - AddPermissionToRoleAsync : Role '{RoleName}' not found.", roleName);
                    return new GeneralResult(false, $"Role '{roleName}' does not exist.");
                }

                var existingClaims = await _roleManager.GetClaimsAsync(role);
                if (existingClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    _logger.LogInformation("PermissionService - AddPermissionToRoleAsync : Permission '{Permission}' already exists for role '{RoleName}'.", permission, roleName);
                    return new GeneralResult(false, $"Permission '{permission}' already exists for role '{roleName}'. Please choose a different permission.");
                }

                var claim = new Claim("Permission", permission);
                var result = await _roleManager.AddClaimAsync(role, claim);
                if (!result.Succeeded)
                {
                    _logger.LogError("PermissionService - AddPermissionToRoleAsync : Failed to add permission '{Permission}' to role '{RoleName}'. Errors: {Errors}", permission, roleName, result.Errors);
                    return new GeneralResult(false, $"Failed to add permission '{permission}' to role '{roleName}'. Please try again.");
                }

                _logger.LogInformation("PermissionService - AddPermissionToRoleAsync : Successfully added permission '{Permission}' to role '{RoleName}'.", permission, roleName);
                return new GeneralResult(true, $"AddPermissionToRole: Successfully added permission '{permission}' to role '{roleName}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - AddPermissionToRoleAsync : Unexpected error while adding permission '{Permission}' to role '{RoleName}'.", permission, roleName);
                return new GeneralResult(false, $"Unexpected error while adding permission '{permission}' to role '{roleName}'. Please try again.");
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> AddPermissionsToRoleAsync(string roleName, List<string> permissions)
        {
            try
            {
                if (permissions == null || !permissions.Any())
                {
                    _logger.LogError("Permissions list cannot be null or empty.");
                    return new GeneralResult(false, "Permissions list cannot be null or empty.");
                }

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogWarning("AddPermissionsToRole: Role '{RoleName}' not found.", roleName);
                    return new GeneralResult(false, $"Role '{roleName}' does not exist.");
                }

                var existingClaims = await _roleManager.GetClaimsAsync(role);
                var distinctPermissions = permissions.Distinct();
                var failedPermissions = new List<string>();

                foreach (var permission in distinctPermissions)
                {
                    if (string.IsNullOrWhiteSpace(permission))
                    {
                        _logger.LogWarning("AddPermissionsToRole: Encountered a null or empty permission in the list.");
                        continue;
                    }

                    if (existingClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                    {
                        _logger.LogInformation("AddPermissionsToRole: Permission '{Permission}' already exists for role '{RoleName}'.", permission, roleName);
                        continue;
                    }

                    var claim = new Claim("Permission", permission);
                    var result = await _roleManager.AddClaimAsync(role, claim);

                    if (!result.Succeeded)
                    {
                        _logger.LogError("AddPermissionsToRole: Failed to add permission '{Permission}' to role '{RoleName}'. Errors: {Errors}", permission, roleName, result.Errors);
                        failedPermissions.Add(permission);
                    }
                }

                if(failedPermissions.Any())
                {
                    _logger.LogInformation("AddPermissionsToRole: Successfully added permissions to role.");
                    return new GeneralResult(true, "Successfully added all permissions to role. except: " + string.Join(", ", failedPermissions) + ".");
                }

                _logger.LogInformation("AddPermissionsToRole: Successfully added permissions to role.");
                return new GeneralResult(true, "Successfully added all permissions to role.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddPermissionsToRole: Unexpected error while adding permissions to role '{RoleName}'.", roleName);
                return new GeneralResult(false, "AddPermissionsToRole: Unexpected error while adding permissions to role" + roleName + ".");
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> RemovePermissionFromRoleAsync(string roleName, string permission)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(permission))
                {
                    _logger.LogWarning("Role name or permission is null or empty.");
                    return new GeneralResult(false, "Role name or permission is null or empty.");
                }

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogWarning("PermissionService - RemovePermissionFromRole: Role '{RoleName}' not found.", roleName);
                    return new GeneralResult(false, $"Role '{roleName}' does not exist.");
                }

                var claim = new Claim("Permission", permission);
                var result = await _roleManager.RemoveClaimAsync(role, claim);

                if (!result.Succeeded)
                {
                    _logger.LogError("PermissionService - RemovePermissionFromRole: Failed to remove permission '{Permission}' from role '{RoleName}'. Errors: {Errors}", permission, roleName, result.Errors);
                    return new GeneralResult(false, $"RemovePermissionFromRole: Failed to remove permission '{permission}' from role '{roleName}'. Errors: {result.Errors}.");
                }

                _logger.LogInformation("PermissionService - RemovePermissionFromRole: Successfully removed permission '{Permission}' from role '{RoleName}'.", permission, roleName);
                return new GeneralResult(true, $"RemovePermissionFromRole: Successfully removed permission '{permission}' from role '{roleName}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - RemovePermissionFromRole: Unexpected error while removing permission '{Permission}' from role '{RoleName}'.", permission, roleName);
                return new GeneralResult(false, $"Unexpected error while removing permission '{permission}' from role '{roleName}'. Please try again.");
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult<List<string>>> GetPermissionsForRoleAsync(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogWarning("PermissionService - GetPermissionsForRole: Role '{RoleName}' not found.", roleName);
                    return new GeneralResult<List<string>>(false, $"Role '{roleName}' does not exist.");
                }

                var claims = await _roleManager.GetClaimsAsync(role);
                var permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

                _logger.LogInformation("PermissionService - GetPermissionsForRole: Retrieved {Count} permissions for role '{RoleName}'.", permissions.Count, roleName);
                return new GeneralResult<List<string>>(true, "Successfully retrieved permissions for role.", permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - GetPermissionsForRole: Unexpected error while retrieving permissions for role '{RoleName}'.", roleName);
                return new GeneralResult<List<string>>(false, $"Unexpected error while retrieving permissions for role '{roleName}'. Please try again.");
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult<List<string>>> GetPermissionsForUserAsync(string userId)
        {
            try
            {
                if (userId == null)
                {
                   _logger.LogError("PermissionService - GetPermissionsForUser: User cannot be null.");
                   return new GeneralResult<List<string>>(false, "User cannot be null.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID {UserId}.", userId);
                    return new GeneralResult<List<string>>(false, "User not found.");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var permissions = new List<string>();

                foreach (var role in roles)
                {
                    var roleClaims = await GetPermissionsForRoleAsync(role);
                    if (roleClaims.Data != null)
                    {
                        permissions.AddRange(roleClaims.Data);
                    }
                }

                var distinctPermissions = permissions.Distinct().ToList();
                _logger.LogInformation("PermissionService - GetPermissionsForUser: Retrieved {Count} distinct permissions for user '{UserId}'.", distinctPermissions.Count, user.Id);
                return new GeneralResult<List<string>>(true, "Successfully retrieved permissions for user.", distinctPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - GetPermissionsForUser: Unexpected error while retrieving permissions for user '{UserId}'.", userId);
                return new GeneralResult<List<string>>(false, $"Unexpected error while retrieving permissions for user '{userId}'. Please try again.");
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult<bool>> UserHasPermissionAsync(string userId, string permission)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID {UserId}.", userId);
                    return new GeneralResult<bool>(false, "User not found.");
                }

                var permissions = await GetPermissionsForUserAsync(userId);
                if (permissions.Data == null)
                {
                    _logger.LogWarning("PermissionService - UserHasPermission: User '{UserId}' does not have any permissions.", user.Id);
                    return new GeneralResult<bool>(false, "User does not have any permissions.");
                }

                var hasPermission = permissions.Data.Contains(permission);
                _logger.LogInformation("PermissionService - UserHasPermission: User '{UserId}' {HasPermission} the permission '{Permission}'.", user.Id, hasPermission ? "has" : "does not have", permission);
                return new GeneralResult<bool>(true, "Successfully checked if user has permission.", hasPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - UserHasPermission: Unexpected error while checking permission '{Permission}' for user '{UserId}'.", permission, userId);
                return new GeneralResult<bool>(false, $"Unexpected error while checking permission '{permission}' for user '{userId}'. Please try again.");
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> RemovePermissionFromUserAsync(User user, string permission)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogError("PermissionService - RemovePermissionFromUser: User cannot be null.");
                    return new GeneralResult { IsSuccess = false, Message = "User cannot be null." };
                }

                if (string.IsNullOrWhiteSpace(permission))
                {
                    _logger.LogError("PermissionService - RemovePermissionFromUser: Permission cannot be null or empty.");
                    return new GeneralResult { IsSuccess = false, Message = "Permission cannot be null or empty." };
                }

                var claims = await _userManager.GetClaimsAsync(user);
                var claimToRemove = claims.FirstOrDefault(c => c.Type == "Permission" && c.Value == permission);

                if (claimToRemove == null)
                {
                    _logger.LogWarning("PermissionService - RemovePermissionFromUser: User '{UserId}' does not have permission '{Permission}'.", user.Id, permission);
                    return new GeneralResult { IsSuccess = false, Message = $"The user does not have the permission '{permission}'." };
                }

                var result = await _userManager.RemoveClaimAsync(user, claimToRemove);
                if (!result.Succeeded)
                {
                    _logger.LogError("PermissionService - RemovePermissionFromUser: Failed to remove permission '{Permission}' from user '{UserId}'. Errors: {Errors}", permission, user.Id, result.Errors);
                    return new GeneralResult { IsSuccess = false, Message = $"RemovePermissionFromUser: Failed to remove permission '{permission}' from user '{user.Id}'. Errors: {result.Errors}." };
                }

                _logger.LogInformation("PermissionService - RemovePermissionFromUser: Successfully removed permission '{Permission}' from user '{UserId}'.", permission, user.Id);
                return new GeneralResult { IsSuccess = true, Message = $"Successfully removed permission '{permission}' from user '{user.Id}'." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - RemovePermissionFromUser: Unexpected error while removing permission '{Permission}' from user '{UserId}'.", permission, user?.Id);
                return new GeneralResult { IsSuccess = false, Message = $"RemovePermissionFromUser: Unexpected error while removing permission '{permission}' from user '{user!.Id}'." };
            }
        }
    }
}

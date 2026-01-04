using Twilio.TwiML.Messaging;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.AothrizationSvc
{
    public class PermissionService(
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager,
        ILogger<PermissionService> logger,
        ILocalizationManager localization) : IPermissionService
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<PermissionService> _logger = logger;
        private readonly ILocalizationManager _localization = localization;

        //// <inheritdoc />
        public async Task<GeneralResult> AddPermissionToRoleAsync(string roleName, string permission)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(permission))
                {
                    _logger.LogError("PermissionService - AddPermissionToRoleAsync : Permission cannot be null or empty.");
                    return new GeneralResult(false, _localization.GetLocalizedString("PermissionCannotBeEmpty"));
                }

                var allPermissions = PermissionsSeeding
                    .GetAllPermissions(includeAdminOnly: true)
                    .Select(p => p.Value)
                    .ToHashSet();

                if (!allPermissions.Contains(permission))
                {
                    _logger.LogWarning("{Method}: Invalid permission '{Permission}' not found in system.", "AddPermissionToRoleAsync", permission);
                    return new GeneralResult(false, _localization.GetLocalizedString("PermissionInvalid"), ErrorType.NotFound);
                }

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogWarning("PermissionService - AddPermissionToRoleAsync : Role '{RoleName}' not found.", roleName);
                    return new GeneralResult(false, _localization.GetLocalizedString("RoleNotFound"));
                }

                var existingClaims = await _roleManager.GetClaimsAsync(role);
                if (existingClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    _logger.LogInformation("PermissionService - AddPermissionToRoleAsync : Permission '{Permission}' already exists for role '{RoleName}'.", permission, roleName);
                    return new GeneralResult(false, _localization.GetLocalizedString("PermissionAlreadyExistsForRole"));
                }

                var claim = new Claim("Permission", permission);
                var result = await _roleManager.AddClaimAsync(role, claim);
                if (!result.Succeeded)
                {
                    _logger.LogError("PermissionService - AddPermissionToRoleAsync : Failed to add permission '{Permission}' to role '{RoleName}'. Errors: {Errors}", permission, roleName, result.Errors);
                    return new GeneralResult(false, _localization.GetLocalizedString("AddPermissionFailed"));
                }

                _logger.LogInformation("PermissionService - AddPermissionToRoleAsync : Successfully added permission '{Permission}' to role '{RoleName}'.", permission, roleName);
                return new GeneralResult(true, _localization.GetLocalizedString("AddPermissionSuccess"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - AddPermissionToRoleAsync : Unexpected error while adding permission '{Permission}' to role '{RoleName}'.", permission, roleName);
                return new GeneralResult(false, _localization.GetLocalizedString("UnexpectedAddPermissionError"));
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
                    return new GeneralResult(false, _localization.GetLocalizedString("PermissionsListCannotBeEmpty"));
                }

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogWarning("AddPermissionsToRole: Role '{RoleName}' not found.", roleName);
                    return new GeneralResult(false, _localization.GetLocalizedString("RoleNotFound"));
                }

                var existingClaims = await _roleManager.GetClaimsAsync(role);
                var distinctPermissions = permissions.Distinct();
                var failedPermissions = new List<string>();

                var allPermissions = PermissionsSeeding
                    .GetAllPermissions(includeAdminOnly: true)
                    .Select(p => p.Value)
                    .ToHashSet();

                foreach (var permission in distinctPermissions)
                {
                    if (!allPermissions.Contains(permission))
                    {
                        _logger.LogWarning("AddPermissionsToRole: Invalid permission '{Permission}' not found in system.", permission);
                        failedPermissions.Add(permission);
                        continue;
                    }

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

                if (failedPermissions.Any())
                {
                    var failedList = string.Join(", ", failedPermissions);
                    var message = string.Format(_localization.GetLocalizedString("PartialPermissionsAddSuccess"), failedList);

                    _logger.LogWarning("AddPermissionsToRole: Some permissions failed to be added: {FailedPermissions}", failedList);
                    return new GeneralResult(true, message);
                }

                _logger.LogInformation("AddPermissionsToRole: Successfully added permissions to role.");
                return new GeneralResult(true, _localization.GetLocalizedString("AllPermissionsAddSuccess"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddPermissionsToRole: Unexpected error while adding permissions to role '{RoleName}'.", roleName);
                return new GeneralResult(false, _localization.GetLocalizedString("UnexpectedAddPermissionsError"));
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
                    return new GeneralResult(false, _localization.GetLocalizedString("RoleOrPermissionIsEmpty"));
                }

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogWarning("PermissionService - RemovePermissionFromRole: Role '{RoleName}' not found.", roleName);
                    return new GeneralResult(false, _localization.GetLocalizedString("RoleNotFound"));
                }

                var allPermissions = PermissionsSeeding
                    .GetAllPermissions(includeAdminOnly: true)
                    .Select(p => p.Value)
                    .ToHashSet();

                if (!allPermissions.Contains(permission))
                {
                    _logger.LogWarning("PermissionService - RemovePermissionFromUserAsync: Invalid permission '{Permission}' requested.", permission);
                    return new GeneralResult(false, _localization.GetLocalizedString("PermissionInvalid"), null, ErrorType.NotFound);
                }

                var existingClaims = await _roleManager.GetClaimsAsync(role);
                var hasPermission = existingClaims.Any(c => c.Type == "Permission" && c.Value == permission);

                if (!hasPermission)
                {
                    _logger.LogWarning("PermissionService - RemovePermissionFromRole: Permission '{Permission}' is not assigned to role '{RoleName}'.", permission, roleName);
                    return new GeneralResult(false, _localization.GetLocalizedString("PermissionNotAssignedToRole"), ErrorType.InvalidData);
                }

                var claim = new Claim("Permission", permission);
                var result = await _roleManager.RemoveClaimAsync(role, claim);

                if (!result.Succeeded)
                {
                    _logger.LogError("PermissionService - RemovePermissionFromRoleAsync: Failed to remove permission '{Permission}' from role '{RoleName}'. Errors: {Errors}", permission, roleName, result.Errors);
                    return new GeneralResult(false, _localization.GetLocalizedString("RemovePermissionFailed"));
                }

                _logger.LogInformation("PermissionService - RemovePermissionFromRole: Successfully removed permission '{Permission}' from role '{RoleName}'.", permission, roleName);
                return new GeneralResult(true, _localization.GetLocalizedString("RemovePermissionSuccess"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - RemovePermissionFromRole: Unexpected error while removing permission '{Permission}' from role '{RoleName}'.", permission, roleName);
                return new GeneralResult(false, _localization.GetLocalizedString("UnexpectedRemovePermissionError"));
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
                    return new GeneralResult<List<string>>(false, _localization.GetLocalizedString("RoleNotFound"));
                }

                var claims = await _roleManager.GetClaimsAsync(role);
                var permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

                _logger.LogInformation("PermissionService - GetPermissionsForRole: Retrieved {Count} permissions for role '{RoleName}'.", permissions.Count, roleName);
                return new GeneralResult<List<string>>(true, _localization.GetLocalizedString("GetPermissionsSuccess"), permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - GetPermissionsForRole: Unexpected error while retrieving permissions for role '{RoleName}'.", roleName);
                return new GeneralResult<List<string>>(false, _localization.GetLocalizedString("UnexpectedGetPermissionsError"));
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
                   return new GeneralResult<List<string>>(false, _localization.GetLocalizedString("UserIdCannotBeNull"));
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID {UserId}.", userId);
                    return new GeneralResult<List<string>>(false, _localization.GetLocalizedString("UserNotFound"));
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
                if(!distinctPermissions.Any())
                {
                    return new GeneralResult<List<string>>(false, _localization.GetLocalizedString("UserHasNoPermissions"));
                }

                _logger.LogInformation("PermissionService - GetPermissionsForUser: Retrieved {Count} distinct permissions for user '{UserId}'.", distinctPermissions.Count, user.Id);
                return new GeneralResult<List<string>>(true, _localization.GetLocalizedString("CheckUserPermissionSuccess"), distinctPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - GetPermissionsForUser: Unexpected error while retrieving permissions for user '{UserId}'.", userId);
                return new GeneralResult<List<string>>(false, _localization.GetLocalizedString("UnexpectedCheckUserPermissionError"));
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
                    return new GeneralResult<bool>(false, _localization.GetLocalizedString("UserNotFound"));
                }

                var allPermissions = PermissionsSeeding
                    .GetAllPermissions(includeAdminOnly: true)
                    .Select(p => p.Value)
                    .ToHashSet();

                if (!allPermissions.Contains(permission))
                {
                    _logger.LogWarning("PermissionService - UserHasPermission: Invalid permission '{Permission}' requested.", permission);
                    return new GeneralResult<bool>(false, _localization.GetLocalizedString("PermissionInvalid"), false, ErrorType.NotFound);
                }

                var permissions = await GetPermissionsForUserAsync(userId);
                if (permissions.Data == null)
                {
                    _logger.LogWarning("PermissionService - UserHasPermission: User '{UserId}' does not have any permissions.", user.Id);
                    return new GeneralResult<bool>(false, _localization.GetLocalizedString("UserDoesNotHavePermission"));
                }

                var hasPermission = permissions.Data.Contains(permission);
                if(hasPermission)
                {
                    _logger.LogInformation("PermissionService - UserHasPermission: User '{UserId}' has the permission '{Permission}'.", user.Id, permission);
                    return new GeneralResult<bool>(true, _localization.GetLocalizedString("UserHavePermission"), hasPermission);
                }

                _logger.LogWarning("PermissionService - UserHasPermission: User '{UserId}' does not have the permission '{Permission}'.", user.Id, permission);
                return new GeneralResult<bool>(false, _localization.GetLocalizedString("UserDoesNotHavePermission"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - UserHasPermission: Unexpected error while checking permission '{Permission}' for user '{UserId}'.", permission, userId);
                return new GeneralResult<bool>(false, _localization.GetLocalizedString("UnexpectedUserHasPermissionError"));
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
                    return new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UserIdCannotBeNull") };
                }

                if (string.IsNullOrWhiteSpace(permission))
                {
                    _logger.LogError("PermissionService - RemovePermissionFromUser: Permission cannot be null or empty.");
                    return new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("PermissionCanNotBeNull") };
                }

                var allPermissions = PermissionsSeeding
                    .GetAllPermissions(includeAdminOnly: true)
                    .Select(p => p.Value)
                    .ToHashSet();

                if (!allPermissions.Contains(permission))
                {
                    _logger.LogWarning("PermissionService - RemovePermissionFromUserAsync: Invalid permission '{Permission}' requested.", permission);
                    return new GeneralResult(false, _localization.GetLocalizedString("PermissionInvalid"), null, ErrorType.NotFound);
                }

                var claims = await _userManager.GetClaimsAsync(user);
                var claimToRemove = claims.FirstOrDefault(c => c.Type == "Permission" && c.Value == permission);

                if (claimToRemove == null)
                {
                    _logger.LogWarning("PermissionService - RemovePermissionFromUser: User '{UserId}' does not have permission '{Permission}'.", user.Id, permission);
                    return new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UserHasNoPermissions") };
                }

                var result = await _userManager.RemoveClaimAsync(user, claimToRemove);
                if (!result.Succeeded)
                {
                    _logger.LogError("PermissionService - RemovePermissionFromUser: Failed to remove permission '{Permission}' from user '{UserId}'. Errors: {Errors}", permission, user.Id, result.Errors);
                    return new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("RemoveUserPermissionFailed") };
                }

                _logger.LogInformation("PermissionService - RemovePermissionFromUser: Successfully removed permission '{Permission}' from user '{UserId}'.", permission, user.Id);
                return new GeneralResult { IsSuccess = true, Message = _localization.GetLocalizedString("RemoveUserPermissionSuccess") };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PermissionService - RemovePermissionFromUser: Unexpected error while removing permission '{Permission}' from user '{UserId}'.", permission, user?.Id);
                return new GeneralResult { IsSuccess = false, Message = _localization.GetLocalizedString("UnexpectedRemoveUserPermissionError") };
            }
        }
    }
}

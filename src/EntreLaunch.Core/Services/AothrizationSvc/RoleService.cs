using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.DTOs.UserDtos;

namespace EntreLaunch.Services.AothrizationSvc
{
    public class RoleService(
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager,
        IConfiguration configuration,
        ILogger<RoleService> logger,
        ILocalizationManager localizationManager) : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<RoleService> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        //// <inheritdoc />
        public async Task<GeneralResult> EnsureSeedRolesAsync()
        {
            try
            {
                var defaultRoles = _configuration.GetSection("DefaultRoles").Get<DefaultRolesConfig>() ?? new DefaultRolesConfig();
                var createdRoles = new List<string>();
                foreach (var role in defaultRoles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        var result = await _roleManager.CreateAsync(new IdentityRole(role));
                        if (!result.Succeeded)
                        {
                            _logger.LogError($"RoleService - EnsureSeedRolesAsync : Failed to create role {role}.");
                            return new GeneralResult(false, _localizationManager.GetLocalizedString("FailedToCreateRole"));
                        }

                        createdRoles.Add(role);
                    }
                }

                if (createdRoles.Any())
                {
                    var message = $"Created roles: {string.Join(", ", createdRoles)}";
                    _logger.LogInformation("RoleService - EnsureSeedRolesAsync : " + message);
                    return new GeneralResult(true, string.Format(_localizationManager.GetLocalizedString("RolesCreatedSuccessfully"), string.Join(", ", createdRoles)));
                }

                _logger.LogInformation("RoleService - EnsureSeedRolesAsync : Default roles already exist.");
                return new GeneralResult(true, _localizationManager.GetLocalizedString("DefaultRolesExist"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleService - EnsureSeedRolesAsync : Error ensuring default roles.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ErrorEnsuringRoles"));
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult<List<string>>> GetAllRolesAsync()
        {
            try
            {
                _logger.LogInformation("RoleService - GetAllRolesAsync : Retrieving all roles.");
                var roles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
                return new GeneralResult<List<string>>(true, _localizationManager.GetLocalizedString("RolesRetrievedSuccessfully"), roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleService - GetAllRolesAsync : Error retrieving all roles.");
                return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("ErrorRetrievingRoles"), null);
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> AddRoleAsync(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    _logger.LogError("RoleService - AddRoleAsync : Role name cannot be null or empty.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("RoleNameCannotBeEmpty"), null);
                }

                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogInformation($"RoleService - AddRoleAsync : Role {roleName} already exists.");
                    return new GeneralResult(true, _localizationManager.GetLocalizedString("RoleAlreadyExists"), null);
                }

                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    _logger.LogError($"RoleService - AddRoleAsync : Failed to add role {roleName}.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("FailedToAddRole"), null);
                }

                _logger.LogInformation($"RoleService - AddRoleAsync : Role {roleName} added successfully.");
                return new GeneralResult(true, _localizationManager.GetLocalizedString("RoleAddedSuccessfully"), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - AddRoleAsync : Error adding role {roleName}.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ErrorAddingRole"));
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult<bool>> RoleExistsAsync(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    _logger.LogError("RoleService - RoleExistsAsync : Role name cannot be null or empty.");
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("RoleNameCannotBeEmpty"), false);
                }

                _logger.LogInformation($"RoleService - RoleExistsAsync : Checking existence of role {roleName}.");
                var result = await _roleManager.RoleExistsAsync(roleName);
                if (!result)
                {
                    return new GeneralResult<bool>(true, _localizationManager.GetLocalizedString("RoleNotFound"), result);
                }

                return new GeneralResult<bool>(true, _localizationManager.GetLocalizedString("RoleFound"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - RoleExistsAsync : Error checking existence of role {roleName}.");
                return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("ErrorInCheckingRole"));
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> AssignRoleAsync(string userId, string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogError("RoleService - AssignRoleAsync : User ID cannot be null or empty.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserIdCannotBeEmpty"));
                }

                if (string.IsNullOrWhiteSpace(role))
                {
                    _logger.LogError("RoleService - AssignRoleAsync : Role name cannot be null or empty.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("RoleNameCannotBeEmpty"));
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted || !user.IsActive)
                {
                    _logger.LogError($"RoleService - AssignRoleAsync : User {userId} not found or inactive.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserNotFoundOrInactive"));
                }

                if (!await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogError($"RoleService - AssignRoleAsync : Role {role} does not exist.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("RoleDoesNotExist"));
                }

                if (await _userManager.IsInRoleAsync(user, role))
                {
                    _logger.LogWarning("User {UserId} already has role {RoleName}.", userId, role);
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserAlreadyInRole"));
                }

                var result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    _logger.LogError($"RoleService - AssignRoleAsync : Failed to assign role {role} to user {userId}.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("FailedToAssignRole"));
                }

                _logger.LogInformation($"RoleService - AssignRoleAsync : Role {role} assigned to user {userId} successfully.");
                return new GeneralResult(true, _localizationManager.GetLocalizedString("RoleAssignedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - AssignRoleAsync : Error assigning role {role} to user {userId}.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ErrorAssigningRole"));
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> DeleteRoleAsync(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogError($"Role '{roleName}' does not exist.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("RoleDoesNotExist"));
                }

                var usersInRole = await GetUsersInRoleAsync(roleName);
                if (usersInRole.IsSuccess && usersInRole.Data?.Any() == true)
                {
                    _logger.LogWarning($"RoleService - DeleteRoleAsync : Role '{roleName}' cannot be deleted because it is assigned to one or more users.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("RoleInUseCannotDelete"));
                }

                if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError($"RoleService - DeleteRoleAsync : Role '{roleName}' cannot be deleted.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("AdminRoleCannotDelete"));
                }

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    _logger.LogError($"RoleService - DeleteRoleAsync : Failed to delete role {roleName}: {result.Errors}");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("FailedToDeleteRole"));
                }

                _logger.LogInformation($"RoleService - DeleteRoleAsync : Role {roleName} deleted successfully.");
                return new GeneralResult(true, _localizationManager.GetLocalizedString("RoleDeletedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - DeleteRoleAsync : Error deleting role {roleName}.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ErrorDeletingRole"));
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> UpdateRoleNameAsync(string oldRoleName, string newRoleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(oldRoleName) || string.IsNullOrWhiteSpace(newRoleName))
                {
                    _logger.LogError("RoleService - UpdateRoleNameAsync : Role name cannot be null or empty.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("RoleNameCannotBeEmpty"));
                }

                var role = await _roleManager.FindByNameAsync(oldRoleName);
                if (role == null)
                {
                    _logger.LogError($"RoleService - UpdateRoleNameAsync : Role '{oldRoleName}' does not exist.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("RoleDoesNotExist"));
                }

                if (await _roleManager.RoleExistsAsync(newRoleName))
                {
                    _logger.LogWarning($"RoleService - UpdateRoleNameAsync : Role '{newRoleName}' already exists.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("RoleAlreadyExistsWithName"));
                }

                role.Name = newRoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    string errors = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    _logger.LogError("RoleService - UpdateRoleNameAsync : Failed to update role name from" + $"{oldRoleName} " + $"to" + $"{newRoleName}", errors);
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("FailedToUpdateRoleName"));
                }

                _logger.LogInformation($"RoleService - UpdateRoleNameAsync : Role name updated from {oldRoleName} to {newRoleName} successfully.");
                return new GeneralResult(true, _localizationManager.GetLocalizedString("RoleNameUpdatedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - UpdateRoleNameAsync : Error updating role name from {oldRoleName} to {newRoleName}.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ErrorUpdatingRoleName"));
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> RemoveRoleAsync(string userId, string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogError("RoleService - RemoveRoleAsync : User id cannot be null or empty.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserIdCannotBeEmpty"));
                }

                if (string.IsNullOrWhiteSpace(role))
                {
                    _logger.LogError("RoleService - RemoveRoleAsync : Role name cannot be null or empty.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("RoleNameCannotBeEmpty"));
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    _logger.LogError($"RoleService - RemoveRoleAsync : User {userId} not found or deleted.");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserNotFound"));
                }

                var isUserInRole = await IsUserInRoleAsync(user.Id, role);
                if (isUserInRole.Data == false)
                {
                    _logger.LogWarning("RoleService - RemoveRoleAsync : User with ID {UserId} not found in role {RoleName}.", userId, role);
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserNotInRole"));
                }

                var result = await _userManager.RemoveFromRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    _logger.LogError($"RoleService - RemoveRoleAsync : Failed to remove role {role} from user {userId}: {result.Errors}");
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("FailedToRemoveRole"));
                }

                _logger.LogInformation($"RoleService - RemoveRoleAsync : Role {role} removed from user {userId} successfully.");
                return new GeneralResult(true, _localizationManager.GetLocalizedString("RoleRemovedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - RemoveRoleAsync : Error removing role {role} from user {userId}.");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ErrorRemovingRole"));
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult<List<UserDetailsDto>>> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    _logger.LogError("RoleService - GetUsersInRoleAsync : Role name cannot be null or empty.");
                    return new GeneralResult<List<UserDetailsDto>>(false, _localizationManager.GetLocalizedString("RoleNameCannotBeEmpty"));
                }

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogError($"RoleService - GetUsersInRoleAsync : Role '{roleName}' does not exist.");
                    return new GeneralResult<List<UserDetailsDto>>(false, _localizationManager.GetLocalizedString("RoleDoesNotExist"));
                }

                var users = await _userManager.GetUsersInRoleAsync(roleName);
                var userDtos = users.Select(u => new UserDetailsDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    AvatarUrl = u.AvatarUrl,
                    DOB = u.DOB,
                    Description = u.Description,
                    Specialization = u.Specialization,
                    CountryCode = u.CountryCode
                }).ToList();

                if(!userDtos.Any())
                {
                    _logger.LogWarning($"RoleService - GetUsersInRoleAsync : No users found in role {roleName}.");
                    return new GeneralResult<List<UserDetailsDto>>(false, _localizationManager.GetLocalizedString("NoUsersInRole"));
                }

                _logger.LogInformation($"RoleService - GetUsersInRoleAsync : Getting users in role {roleName}.");
                return new GeneralResult<List<UserDetailsDto>>(true, _localizationManager.GetLocalizedString("UsersInRoleRetrieved"), userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - GetUsersInRoleAsync : Error getting users in role {roleName}.");
                return new GeneralResult<List<UserDetailsDto>>(false, _localizationManager.GetLocalizedString("ErrorGettingUsersInRole"));
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult<bool>> IsUserInRoleAsync(string userId, string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogError("RoleService - IsUserInRoleAsync : User ID cannot be null or empty.");
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("UserIdCannotBeEmpty"));
                }

                if (string.IsNullOrWhiteSpace(roleName))
                {
                    _logger.LogError("RoleService - IsUserInRoleAsync : Role name cannot be null or empty.");
                    return new GeneralResult<bool>(false, "Role name cannot be null or empty.", false);
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(usr => usr.Id == userId && !usr.IsDeleted);
                if (user == null)
                {
                    _logger.LogError($"RoleService - IsUserInRoleAsync : User {userId} not found.");
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("UserNotFound"));
                }

                var result = await _userManager.IsInRoleAsync(user, roleName);
                if (result)
                {
                    _logger.LogInformation($"RoleService - IsUserInRoleAsync : User {userId} is in role {roleName}.");
                    return new GeneralResult<bool>(true, _localizationManager.GetLocalizedString("UserIsInRole"), true);
                }

                _logger.LogInformation($"RoleService - IsUserInRoleAsync : User {userId} is not in role {roleName}.");
                return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("UserIsNotInRole"), false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - IsUserInRoleAsync : Error checking if user {userId} is in role {roleName}.");
                return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("ErrorCheckingUserRole"), false);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<List<string>>> GetUserRolesAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogError("RoleService - GetUserRolesAsync : User ID cannot be null or empty.");
                    return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("UserIdCannotBeEmpty"));
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    _logger.LogError($"RoleService - GetUserRolesAsync : User with ID '{userId}' not found.");
                    return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("UserNotFound"));
                }

                var roles = await _userManager.GetRolesAsync(user);
                _logger.LogInformation($"RoleService - GetUserRolesAsync : Retrieved roles for user {userId}.");
                return new GeneralResult<List<string>>(true, _localizationManager.GetLocalizedString("RolesRetrievedForUser"), roles.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - GetUserRolesAsync : Error retrieving roles for user {userId}.");
                return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("ErrorRetrievingUserRoles"), null);
            }
        }
    }
}

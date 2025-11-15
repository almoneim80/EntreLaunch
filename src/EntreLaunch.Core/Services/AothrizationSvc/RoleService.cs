using Nest;
using EntreLaunch.Interfaces.AuthrizationIntf;

namespace EntreLaunch.Services.AothrizationSvc
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RoleService> _logger;
        public RoleService(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            IConfiguration configuration,
            ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

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
                            return new GeneralResult(false, $"Failed to create role {role}.", null);
                        }

                        createdRoles.Add(role);
                    }
                }

                if (createdRoles.Any())
                {
                    var message = $"Created roles: {string.Join(", ", createdRoles)}";
                    _logger.LogInformation("RoleService - EnsureSeedRolesAsync : " + message);
                    return new GeneralResult(true, message);
                }

                _logger.LogInformation("RoleService - EnsureSeedRolesAsync : Default roles already exist.");
                return new GeneralResult(true, "Default roles already exist.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleService - EnsureSeedRolesAsync : Error ensuring default roles.");
                return new GeneralResult(false, "Error ensuring default roles.", null);
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult<List<string>>> GetAllRolesAsync()
        {
            try
            {
                _logger.LogInformation("RoleService - GetAllRolesAsync : Retrieving all roles.");
                var roles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
                return new GeneralResult<List<string>>(true, "Roles retrieved successfully.", roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoleService - GetAllRolesAsync : Error retrieving all roles.");
                return new GeneralResult<List<string>>(false, "Error retrieving all roles.", null);
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
                    return new GeneralResult(false, "Role name cannot be null or empty.", null);
                }

                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogInformation($"RoleService - AddRoleAsync : Role {roleName} already exists.");
                    return new GeneralResult(true, $"Role {roleName} already exists.", null);
                }

                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    _logger.LogError($"RoleService - AddRoleAsync : Failed to add role {roleName}.");
                    return new GeneralResult(false, $"Failed to add role {roleName}.", null);
                }

                _logger.LogInformation($"RoleService - AddRoleAsync : Role {roleName} added successfully.");
                return new GeneralResult(true, $"Role {roleName} added successfully.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - AddRoleAsync : Error adding role {roleName}.");
                return new GeneralResult(false, $"Error adding role {roleName}.");
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
                    return new GeneralResult<bool>(false, "Role name cannot be null or empty.", false);
                }

                _logger.LogInformation($"RoleService - RoleExistsAsync : Checking existence of role {roleName}.");
                var result = await _roleManager.RoleExistsAsync(roleName);
                return new GeneralResult<bool>(true, $"Role {roleName} exists.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - RoleExistsAsync : Error checking existence of role {roleName}.");
                return new GeneralResult<bool>(false, $"Error checking existence of role {roleName}.");
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
                    return new GeneralResult(false, "User ID cannot be null or empty.");
                }

                if (string.IsNullOrWhiteSpace(role))
                {
                    _logger.LogError("RoleService - AssignRoleAsync : Role name cannot be null or empty.");
                    return new GeneralResult(false, "Role name cannot be null or empty.");
                }


                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted || !user.IsActive)
                {
                    _logger.LogError($"RoleService - AssignRoleAsync : User {userId} not found or inactive.");
                    return new GeneralResult(false, $"User {userId} not found or deleted or inactive");
                }

                if (!await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogError($"RoleService - AssignRoleAsync : Role {role} does not exist.");
                    return new GeneralResult(false, $"Role {role} does not exist.");
                }

                if (await _userManager.IsInRoleAsync(user, role))
                {
                    _logger.LogWarning("User {UserId} already has role {RoleName}.", userId, role);
                    return new GeneralResult(false, $"User {userId} already has role {role}.");
                }

                var result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    _logger.LogError($"RoleService - AssignRoleAsync : Failed to assign role {role} to user {userId}.");
                    return new GeneralResult(false, $"Failed to assign role {role} to user {userId}.");
                }

                _logger.LogInformation($"RoleService - AssignRoleAsync : Role {role} assigned to user {userId} successfully.");
                return new GeneralResult(true, $"Role {role} assigned to user {userId} successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - AssignRoleAsync : Error assigning role {role} to user {userId}.");
                return new GeneralResult(false, $"Error assigning role {role} to user {userId}.");
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
                    return new GeneralResult(false, $"RoleService - DeleteRoleAsync : Role '{roleName}' does not exist.");
                }

                var usersInRole = await GetUsersInRoleAsync(roleName);
                if (usersInRole.IsSuccess && usersInRole.Data?.Any() == true)
                {
                    _logger.LogWarning($"RoleService - DeleteRoleAsync : Role '{roleName}' cannot be deleted because it is assigned to one or more users.");
                    return new GeneralResult(false, $"Role '{roleName}' cannot be deleted because it is assigned to one or more users.");
                }

                if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError($"RoleService - DeleteRoleAsync : Role '{roleName}' cannot be deleted.");
                    return new GeneralResult(false, $"Role '{roleName}' cannot be deleted.");
                }

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    _logger.LogError($"RoleService - DeleteRoleAsync : Failed to delete role {roleName}: {result.Errors}");
                    return new GeneralResult(false, $"Failed to delete role {roleName}");
                }

                _logger.LogInformation($"RoleService - DeleteRoleAsync : Role {roleName} deleted successfully.");
                return new GeneralResult(true, $"Role {roleName} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - DeleteRoleAsync : Error deleting role {roleName}.");
                return new GeneralResult(false, $"Error deleting role {roleName}.");
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> EntreLaunchdateRoleNameAsync(string oldRoleName, string newRoleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(oldRoleName) || string.IsNullOrWhiteSpace(newRoleName))
                {
                    _logger.LogError("RoleService - EntreLaunchdateRoleNameAsync : Role name cannot be null or empty.");
                    return new GeneralResult(false, "Role name cannot be null or empty.");
                }

                var role = await _roleManager.FindByNameAsync(oldRoleName);
                if (role == null)
                {
                    _logger.LogError($"RoleService - EntreLaunchdateRoleNameAsync : Role '{oldRoleName}' does not exist.");
                    return new GeneralResult(false, $"Role '{oldRoleName}' does not exist.");
                }

                if (await _roleManager.RoleExistsAsync(newRoleName))
                {
                    _logger.LogWarning($"RoleService - EntreLaunchdateRoleNameAsync : Role '{newRoleName}' already exists.");
                    return new GeneralResult(false, $"Role '{newRoleName}' already exists.");
                }

                role.Name = newRoleName;
                var result = await _roleManager.EntreLaunchdateAsync(role);
                if (!result.Succeeded)
                {
                    string errors = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    _logger.LogError("RoleService - EntreLaunchdateRoleNameAsync : Failed to EntreLaunchdate role name from" + $"{oldRoleName} " + $"to" + $"{newRoleName}", errors);
                    return new GeneralResult(false, "Failed to EntreLaunchdate role name.");
                }

                _logger.LogInformation($"RoleService - EntreLaunchdateRoleNameAsync : Role name EntreLaunchdated from {oldRoleName} to {newRoleName} successfully.");
                return new GeneralResult(true, $"Role name EntreLaunchdated from {oldRoleName} to {newRoleName} successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - EntreLaunchdateRoleNameAsync : Error EntreLaunchdating role name from {oldRoleName} to {newRoleName}.");
                return new GeneralResult(false, $"Error EntreLaunchdating role name from {oldRoleName} to {newRoleName}.");
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
                    return new GeneralResult(false, "User id cannot be null or empty.");
                }

                if (string.IsNullOrWhiteSpace(role))
                {
                    _logger.LogError("RoleService - RemoveRoleAsync : Role name cannot be null or empty.");
                    return new GeneralResult(false, "Role name cannot be null or empty.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    _logger.LogError($"RoleService - RemoveRoleAsync : User {userId} not found or deleted.");
                    return new GeneralResult(false, $"User {userId} not found or deleted.");
                }

                var isUserInRole = await IsUserInRoleAsync(user.Id, role);
                if (isUserInRole.Data == false)
                {
                    _logger.LogWarning("RoleService - RemoveRoleAsync : User with ID {UserId} not found in role {RoleName}.", userId, role);
                    return new GeneralResult(false, isUserInRole.Message ?? "User not found in role.");
                }

                var result = await _userManager.RemoveFromRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    _logger.LogError($"RoleService - RemoveRoleAsync : Failed to remove role {role} from user {userId}: {result.Errors}");
                    return new GeneralResult(false, $"Failed to remove role {role} from user {userId}");
                }

                _logger.LogInformation($"RoleService - RemoveRoleAsync : Role {role} removed from user {userId} successfully.");
                return new GeneralResult(true, $"Role {role} removed from user {userId} successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - RemoveRoleAsync : Error removing role {role} from user {userId}.");
                return new GeneralResult(false, $"Error removing role {role} from user {userId}.");
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult<List<User>>> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    _logger.LogError("RoleService - GetUsersInRoleAsync : Role name cannot be null or empty.");
                    return new GeneralResult<List<User>>(false, "Role name cannot be null or empty.");
                }

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogError($"RoleService - GetUsersInRoleAsync : Role '{roleName}' does not exist.");
                    return new GeneralResult<List<User>>(false, $"Role '{roleName}' does not exist.");
                }

                var users = await _userManager.GetUsersInRoleAsync(roleName);
                _logger.LogInformation($"RoleService - GetUsersInRoleAsync : Getting users in role {roleName}.");
                return new GeneralResult<List<User>>(true, $"Users in role {roleName} retrieved successfully.", users.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - GetUsersInRoleAsync : Error getting users in role {roleName}.");
                return new GeneralResult<List<User>>(false, $"Error getting users in role {roleName}.");
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
                    return new GeneralResult<bool>(false, "Role name cannot be null or empty.", false);
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
                    return new GeneralResult<bool>(false, $"User {userId} not found.", false);
                }

                var result = await _userManager.IsInRoleAsync(user, roleName);
                if (result)
                {
                    _logger.LogInformation($"RoleService - IsUserInRoleAsync : User {userId} is in role {roleName}.");
                    return new GeneralResult<bool>(true, $"User {userId} is in role {roleName}.", result);
                }

                _logger.LogInformation($"RoleService - IsUserInRoleAsync : User {userId} is not in role {roleName}.");
                return new GeneralResult<bool>(true, $"User {userId} is not in role {roleName}.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - IsUserInRoleAsync : Error checking if user {userId} is in role {roleName}.");
                return new GeneralResult<bool>(false, $"Error checking if user {userId} is in role {roleName}.", false);
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
                    return new GeneralResult<List<string>>(false, "User ID cannot be null or empty.");
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    _logger.LogError($"RoleService - GetUserRolesAsync : User with ID '{userId}' not found.");
                    return new GeneralResult<List<string>>(false, $"User with ID '{userId}' not found.");
                }

                var roles = await _userManager.GetRolesAsync(user);
                _logger.LogInformation($"RoleService - GetUserRolesAsync : Retrieved roles for user {userId}.");
                return new GeneralResult<List<string>>(true, $"Roles for user {userId} retrieved successfully.", roles.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RoleService - GetUserRolesAsync : Error retrieving roles for user {userId}.");
                return new GeneralResult<List<string>>(false, $"Error retrieving roles for user {userId}.", null);
            }
        }
    }
}

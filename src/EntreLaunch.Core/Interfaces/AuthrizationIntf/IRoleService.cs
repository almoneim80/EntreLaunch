namespace EntreLaunch.Interfaces.AuthrizationIntf
{
    public interface IRoleService
    {
        /// <summary>
        /// Ensure virtual roles exist.
        /// </summary>
        Task<GeneralResult> EnsureSeedRolesAsync();

        /// <summary>
        /// Bring in all the roles.
        /// </summary>
        Task<GeneralResult<List<string>>> GetAllRolesAsync();

        /// <summary>
        /// Add a new role. 
        /// </summary>
        Task<GeneralResult> AddRoleAsync(string roleName);

        /// <summary>
        ///  Assign a role to the user.
        /// </summary>
        Task<GeneralResult> AssignRoleAsync(string userId, string role);

        /// <summary>
        ///  Delete roles.
        /// </summary>
        Task<GeneralResult> DeleteRoleAsync(string roleName);

        /// <summary>
        /// EntreLaunchdate Role Name.
        /// </summary>
        Task<GeneralResult> EntreLaunchdateRoleNameAsync(string oldRoleName, string newRoleName);

        /// <summary>
        /// Removing a role from a user.
        /// </summary>
        Task<GeneralResult> RemoveRoleAsync(string userId, string role);

        /// <summary>
        /// Get all users in a specific role.
        /// </summary>
        Task<GeneralResult<List<User>>> GetUsersInRoleAsync(string roleName);

        /// <summary>
        /// Check for a specific role.
        /// </summary>
        Task<GeneralResult<bool>> RoleExistsAsync(string roleName);

        /// <summary>
        /// Make sure the user is assigned to a specific role.
        /// </summary>
        Task<GeneralResult<bool>> IsUserInRoleAsync(string userId, string roleName);

        /// <summary>
        /// Get all roles for a specific user.
        /// </summary>
        Task<GeneralResult<List<string>>> GetUserRolesAsync(string userId);
    }
}

namespace EntreLaunch.Interfaces.AuthrizationIntf;
public interface IPermissionService
{
    /// <summary>
    /// Add permission to role.
    /// </summary>
    Task<GeneralResult> AddPermissionToRoleAsync(string roleName, string permission);

    /// <summary>
    /// Add permissions to role.
    /// </summary>
    Task<GeneralResult> AddPermissionsToRoleAsync(string roleName, List<string> permissions);

    /// <summary>
    /// Remove permission from role.
    /// </summary>
    Task<GeneralResult> RemovePermissionFromRoleAsync(string roleName, string permission);

    /// <summary>
    /// Get permissions for role.
    /// </summary>
    Task<GeneralResult<List<string>>> GetPermissionsForRoleAsync(string roleName);

    /// <summary>
    /// Get permissions for user.
    /// </summary>
    Task<GeneralResult<List<string>>> GetPermissionsForUserAsync(string userId);

    /// <summary>
    /// Check if user has permission.
    /// </summary>
    Task<GeneralResult<bool>> UserHasPermissionAsync(string userId, string permission);

    /// <summary>
    /// Remove permission from user.
    /// </summary>
    Task<GeneralResult> RemovePermissionFromUserAsync(User user, string permission);
}

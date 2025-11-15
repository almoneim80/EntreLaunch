namespace EntreLaunch.Interfaces.AuthenticationIntf
{
    public interface IIdentityService
    {
        /// <summary>
        /// Find user by email address.
        /// </summary>
        Task<GeneralResult<User>> FindOnRegister(string email);

        /// <summary>
        /// Create user claims principal from user.
        /// </summary>
        Task<GeneralResult<ClaimsPrincipal>> CreateUserClaimsPrincipal(User user);

        /// <summary>
        /// Create user claims from user.
        /// </summary>
        Task<GeneralResult<List<Claim>>> CreateUserClaims(User user);

        /// <summary>
        /// Login user.
        /// </summary>
        Task<GeneralResult> LoginAsync(LoginDto dto);

        /// <summary>
        /// regenerate access token using refresh token.
        /// </summary>
        Task<GeneralResult> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Logout user by user id and refresh token.
        /// </summary>
        Task<GeneralResult> LogoutAsync(string userId, string? refreshToken);
    }
}

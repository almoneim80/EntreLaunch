namespace EntreLaunch.Interfaces.AuthenticationIntf;

/// <summary>
/// Interface for JWT token service.
/// </summary>
public interface ITokenService
{
    Task<JWTokenDto> GenerateTokenWithRefreshTokenAsync(User user, IEnumerable<Claim>? extraClaims = null, TimeSpan? expiresIn = null);
    string GenerateRefreshToken();
    string HashRefreshToken(string rawToken);
}

#nullable disable
namespace EntreLaunch.DTOs;

/// <summary>
/// DTO for user login.
/// </summary>
public class LoginDto
{
    [Required]
    [EmailAddress]
    required public string Email { get; set; }

    [Required]
    required public string Password { get; set; }
}

/// <summary>
/// DTO for refresh token.
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

/// <summary>
/// DTO for logout.
/// </summary>
public class LogoutRequest
{
    public string RefreshToken { get; set; }
}

/// <summary>
/// DTO for JWT token.
/// </summary>
public class JWTokenDto
{
    [Required]
    required public string Token { get; set; }

    [Required]
    required public DateTime Expiration { get; set; }

    [Required]
    required public string RefreshToken { get; set; }

    public UserLogedData userLogedData { get; set; }
}

/// <summary>
/// DTO for user information.
/// </summary>
public class UserInfo
{
    // بيانات المستخدم
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string FamilyName { get; set; }
    public bool ConfirmedEmail { get; set; }
}

/// <summary>
/// Result of a user authentication attempt.
/// </summary>
public class AuthResult
{
    public string ErrorMessage { get; set; }
    public bool Success { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}

/* USER GENERAL DATA  */
public class UserLogedData
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public double NationalId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string AvatarUrl { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public string Description { get; set; }
    public string Specialization { get; set; }
    public Country CountryCode { get; set; }
}

/* GOOGLE AUTH DTO */

/// <summary>
/// DTO for external authentication.
/// </summary>
public class ExternalRegisterDto
{
    public UserInfo UserInfo { get; set; }
    // بيانات التسجيل
    public string Provider { get; set; }
    public string ProviderKey { get; set; }
    public string ProviderDisplayName { get; set; }
    public string Token { get; set; }
    public string ErrorMessage { get; set; }
    public bool Success { get; set; }
}

public class GoogleTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("id_token")]
    public string IdToken { get; set; }
}

public class GoogleUserInfoResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("verified_email")]
    public bool VerifiedEmail { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("given_name")]
    public string GivenName { get; set; }

    [JsonPropertyName("family_name")]
    public string FamilyName { get; set; }

    [JsonPropertyName("picture")]
    public string Picture { get; set; }

    [JsonPropertyName("locale")]
    public string Locale { get; set; }
}

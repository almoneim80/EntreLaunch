namespace EntreLaunch.Configuration;

public class EntitiesConfig
{
    public string[] Include { get; set; } = Array.Empty<string>();

    public string[] Exclude { get; set; } = Array.Empty<string>();
}

public class MethodsReturnData
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string>? AdditionalData { get; set; }
}

// CacheSettings 
public class CacheSettings
{
    public int CacheExpirationMinutes { get; set; }
    public CacheItemPriority CacheItemPriority { get; set; } = CacheItemPriority.Normal;
    public long? CacheItemSize { get; set; }
}

// EmailSenderOptions
public class EmailSenderOptions
{
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

// Localization 
public class LocalizationSettings
{
    public string? DefaultCulture { get; set; }
    public string? RatingsPath { get; set; }
    public string? CulturesPath { get; set; }
    public string? CountriesPath { get; set; }
}

// Otp Verification Options 
public class OtpVerificationOptions
{
    // Enable or disable in-memory cache
    public bool IsInMemoryCache { get; set; }

    // Active to generate URL to verify code with Id OTP
    public bool EnableUrl { get; set; }
    public int Iterations { get; set; }
    public int Size { get; set; }
    public int Length { get; set; }
    public int Expire { get; set; }
    public string? BaseOtpUrl { get; set; }
}

// Reset Password
public class ResetPasswordDto
{
#nullable disable
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string NewPassword { get; set; }

    [Required]
    public string Token { get; set; }
}

// Enum Data
public class EnumData
{
    public int Value { get; set; }
    public string Description { get; set; }
}

/// <summary>
/// Base Service Config.
/// </summary>
public class BaseServiceConfig
{
    public string Server { get; set; } = string.Empty;

    public int Port { get; set; } = 0;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

// Result DTO for create user response 
public class ResultDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}

public class PostgresConfig : BaseServiceConfig
{
    public string Database { get; set; } = string.Empty;

    public string ConnectionString => $"User ID={UserName};Password={Password};Server={Server};Port={Port};Database={Database};Pooling=true;";
}

public class ElasticConfig : BaseServiceConfig
{
    public bool UseHttps { get; set; } = false;

    public string IndexPrefix { get; set; } = string.Empty;

    public string Url => $"http{(UseHttps ? "s" : string.Empty)}://{Server}:{Port}";
}

public class ExtensionConfig
{
    public string Extension { get; set; } = string.Empty;

    public string MaxSize { get; set; } = string.Empty;
}

public class MediaConfig
{
    public string[] Extensions { get; set; } = Array.Empty<string>();

    public ExtensionConfig[] MaxSize { get; set; } = Array.Empty<ExtensionConfig>();

    public string CacheTime { get; set; }
}

public class FileConfig
{
    public string[] Extensions { get; set; } = Array.Empty<string>();

    public ExtensionConfig[] MaxSize { get; set; } = Array.Empty<ExtensionConfig>();
}

public class EmailVerificationApiConfig
{
    public string Url { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
}

public class AccountDetailsApiConfig
{
    public string Url { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
}

public class ApiSettingsConfig
{
    public int MaxListSize { get; set; }

    public string MaxImportSize { get; set; } = string.Empty;

    public string DefaultLanguage { get; set; } = "en-US";
}

public class GeolocationApiConfig
{
    public string Url { get; set; } = string.Empty;

    public string AuthKey { get; set; } = string.Empty;
}

public class TaskConfig
{
    public bool Enable { get; set; }

    public string CronSchedule { get; set; } = string.Empty;

    public int RetryCount { get; set; }

    public int RetryInterval { get; set; }
}

public class TaskWithBatchConfig : TaskConfig
{
    public int BatchSize { get; set; }
}

public class DomainVerificationTaskConfig : TaskWithBatchConfig
{
    public int BatchInterval { get; set; }
}

public class CacheProfileSettings
{
    public string Type { get; set; } = string.Empty;

    public string VaryByHeader { get; set; } = string.Empty;

    public int? DurationInDays { get; set; }
}

public class AppSettings
{
    public PostgresConfig Postgres { get; set; } = new PostgresConfig();

    public ElasticConfig Elastic { get; set; } = new ElasticConfig();
}

public class CorsConfig
{
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}

public class EmailConfig : BaseServiceConfig
{
    public bool UseSsl { get; set; }
}

public class JwtConfig
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Secret { get; set; } = string.Empty;
    public int RefreshTokenExpirationDays { get; set; } = 30;
}

public class EntreLaunchdateRoleDto
{
    public string OldRoleName { get; set; } = string.Empty;
    public string NewRoleName { get; set; } = string.Empty;
}

public class DefaultRolesConfig : List<string>
{
}

public class DefaultUserConfig
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DefaultRolesConfig Roles { get; set; } = new DefaultRolesConfig();
}

public class DefaultUsersConfig : List<DefaultUserConfig>
{
}
public class AssignRoleDto
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class DeleteRoleFromUserDto
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class AssignRoleByEmailDto
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CookiesConfig
{
    public bool Enable { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ExpireTime { get; set; } = 12; // Gets or sets expiration time in hours.
}

public class IdentityConfig
{
    public double LockoutTime { get; set; } = 5;

    public int MaxFailedAccessAttempts { get; set; } = 10;
}

#nullable enable
// payment Result
public class PaymentResult
{
    public bool IsSuccess { get; set; } 

    public string? PaymentStatus { get; set; } 

    public string? TransactionId { get; set; } 

    public decimal? PaidAmount { get; set; } 

    public DateTimeOffset? PaymentDate { get; set; } 
    public string? RedirectUrl { get; set; } 

    public string? ErrorMessage { get; set; } 
}

// Refund Result
public class RefundResult
{
    public bool IsSuccess { get; set; } 
    public string? RefundStatus { get; set; } 

    public string? TransactionId { get; set; } 
    public decimal? RefundedAmount { get; set; } 

    public DateTimeOffset? RefundDate { get; set; } 

    public string? ErrorMessage { get; set; } 
}

// PayTabs
public class PayTabsOptions
{
    public string ProfileId { get; set; } = null!;
    public string ServerKey { get; set; } = null!;
    public string BaseUrl { get; set; } = null!;
    public string ReturnUrl { get; set; } = null!;
    public string CallbackUrl { get; set; } = null!;
    public List<string>? SEntreLaunchportedCurrencies { get; set; }
    public string? DefaultCurrency { get; set; }
}

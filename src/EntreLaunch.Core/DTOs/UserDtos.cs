namespace EntreLaunch.DTOs;

public class UserCreateDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public double? NationalId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}

public class CompleteUserDetailsDto
{
    public string? AvatarUrl { get; set; }
    public DateTimeOffset? DOB { get; set; }
    public string? Description { get; set; }
    public string? Specialization { get; set; }
    public Country? CountryCode { get; set; }
}

public class UserEntreLaunchdateDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public double? NationalId { get; set; }
    public string? PhoneNumber { get; set; } 
    public string? AvatarUrl { get; set; }
    public DateTimeOffset? DOB { get; set; } 
    public string? Description { get; set; }
    public string? Specialization { get; set; }
    public Country? CountryCode { get; set; } 
}

public class UserDetailsDto : UserCreateDto
{
    public Guid? Id { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTimeOffset? DOB { get; set; }
    public string? Description { get; set; }
    public string? Specialization { get; set; }
    public Country? CountryCode { get; set; }
}

public class OtpVerificationDto
{
    public string UserId { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
}

public class OtpResendDto
{
    public string UserId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

using Twilio.TwiML.Messaging;
using EntreLaunch.DTOs.AuthenticationDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.AuthenticationSvc
{
    public class IdentityService(
        UserManager<User> userManager,
        ILogger<IdentityService> logger,
        SignInManager<User> signInManager,
        IPermissionService permissionService,
        PgDbContext dbContext,
        IOptions<JwtConfig> jwtConfig,
        ITokenService tokenService,
        ILocalizationManager localizationManager) : IIdentityService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<IdentityService> _logger = logger;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly PgDbContext _dbContext = dbContext;
        private readonly IOptions<JwtConfig> _jwtConfig = jwtConfig;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult<User>> FindOnRegister(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("Email cannot be null or empty.");
                    return new GeneralResult<User>(false, _localizationManager.GetLocalizedString("EmailCannotBeEmpty"), null);
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new User
                    {
                        FirstName = email,
                        Email = email,
                        CreatedAt = DateTime.UtcNow,
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Failed to create user with email {Email}. Errors: {Errors}", email, result.Errors);
                        return new GeneralResult<User>(false, _localizationManager.GetLocalizedString("FailedToCreateUser"), null);
                    }

                    _logger.LogInformation("User with email {Email} created successfully.", email);
                    return new GeneralResult<User>(true, _localizationManager.GetLocalizedString("UserCreatedSuccessfully"), user);
                }

                return new GeneralResult<User>(true, _localizationManager.GetLocalizedString("UserFoundSuccessfully"), user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during FindOnRegister for email {Email}.", email);
                return new GeneralResult<User>(false, _localizationManager.GetLocalizedString("ErrorDuringFindOnRegister"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ClaimsPrincipal>> CreateUserClaimsPrincipal(User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogWarning("User cannot be null.");
                    return new GeneralResult<ClaimsPrincipal>(false, _localizationManager.GetLocalizedString("UserCannotBeNull"), null);
                }

                var claims = await CreateUserClaims(user);
                if (claims.Data == null)
                {
                    _logger.LogWarning("Claims cannot be null.");
                    return new GeneralResult<ClaimsPrincipal>(false, _localizationManager.GetLocalizedString("ClaimsCannotBeNull"), null);
                }

                var identity = new ClaimsIdentity(claims.Data);
                _logger.LogInformation("ClaimsPrincipal created successfully for user {UserId}.", user.Id);
                return new GeneralResult<ClaimsPrincipal>(true, _localizationManager.GetLocalizedString("ClaimsPrincipalCreated"), new ClaimsPrincipal(identity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ClaimsPrincipal for user {UserId}.", user?.Id);
                return new GeneralResult<ClaimsPrincipal>(false, _localizationManager.GetLocalizedString("ErrorCreatingClaimsPrincipal"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<Claim>>> CreateUserClaims(User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogWarning("User cannot be null.");
                    return new GeneralResult<List<Claim>>(false, _localizationManager.GetLocalizedString("UserCannotBeNull"), null);
                }

                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("LoginProvider", "Google"),
                };

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                _logger.LogInformation("Claims created successfully for user {UserId}.", user.Id);
                return new GeneralResult<List<Claim>>(true, _localizationManager.GetLocalizedString("ClaimsCreatedSuccessfully"), claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error while creating claims for user {UserId}.", user?.Id);
                return new GeneralResult<List<Claim>>(false, _localizationManager.GetLocalizedString("ErrorCreatingClaims"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> LoginAsync(LoginDto dto)
        {
            try
            {
                var now = DateTimeOffset.UtcNow;
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null || user.IsDeleted)
                {
                    _logger.LogWarning("Login failed. User not found or deactivated for email: {Email}", dto.Email);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("LoginUserNotFoundOrDeactivated"),
                        Data = null
                    };
                }

                if (!user.EmailConfirmed)
                {
                    _logger.LogInformation("Email is not confirmed for user {Email}.", dto.Email);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("EmailNotConfirmed"),
                        Data = null
                    };
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Account locked for user {Email}.", dto.Email);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AccountLocked"),
                        Data = null
                    };
                }

                var signResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, true);
                if (!signResult.Succeeded)
                {
                    if (signResult.IsLockedOut)
                    {
                        _logger.LogWarning("Too many requests for user {Email}.", dto.Email);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = _localizationManager.GetLocalizedString("TooManyLoginAttempts"),
                            Data = null
                        };
                    }
                    else
                    {
                        _logger.LogWarning("UnExpected error while login for user {Email}.", dto.Email);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = _localizationManager.GetLocalizedString("UnexpectedLoginError"),
                            Data = null
                        };
                    }
                }

                var tokenDto = await _tokenService.GenerateTokenWithRefreshTokenAsync(user);
                var hashedRefresh = _tokenService.HashRefreshToken(tokenDto.RefreshToken);
                _dbContext.RefreshTokens.Add(new RefreshToken
                {
                    UserId = user.Id,
                    TokenHash = hashedRefresh,
                    Expiration = now.AddDays(_jwtConfig.Value.RefreshTokenExpirationDays),
                    CreatedAt = now
                });

                await _dbContext.SaveChangesAsync();

                // fetch user permissions and roles
                var roleNames = await _userManager.GetRolesAsync(user);
                var userRoles = new List<UserRoleDto>();

                foreach (var roleName in roleNames)
                {
                    var permissionResult = await permissionService.GetPermissionsForRoleAsync(roleName);
                    var rolePermissions = permissionResult.Data ?? new List<string>();

                    userRoles.Add(new UserRoleDto
                    {
                        RoleName = roleName,
                        Permissions = rolePermissions
                            .Select(p => new UserPermissionsDto { PermissionName = p })
                            .ToList()
                    });
                }

                var userInfo = new UserLogedData
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    NationalId = user.NationalId ?? 0,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AvatarUrl = user.AvatarUrl,
                    DateOfBirth = user.DOB ?? now,
                    Specialization = user.Specialization,
                    CountryCode = user.CountryCode,
                    UserRoleAndPermissions = userRoles
                };

                tokenDto.userLogedData = userInfo;

                _logger.LogInformation("Login successful for user {Email}.", dto.Email);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("LoginSuccess"),
                    Data = tokenDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}.", dto.Email);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("LoginError"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("RefreshTokenRequired"),
                        Data = null
                    };
                }

                // calculate the hash of the incoming refresh token
                var hashed = _tokenService.HashRefreshToken(refreshToken);

                // find the refresh token in the database
                var stored = await _dbContext.RefreshTokens.FirstOrDefaultAsync(r =>
                r.TokenHash == hashed &&
                !r.IsUsed &&
                !r.IsRevoked &&
                r.Expiration > DateTimeOffset.UtcNow);
                if (stored == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("InvalidRefreshToken"),
                        Data = null
                    };
                }

                // find the user
                var user = await _userManager.FindByIdAsync(stored.UserId);
                if (user == null || user.IsDeleted)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                // disable the old refresh token
                stored.IsUsed = true;
                stored.IsRevoked = true;
                _dbContext.RefreshTokens.Update(stored);

                // generate a new refresh token
                var tokenDto = await _tokenService.GenerateTokenWithRefreshTokenAsync(user);
                var newHashed = _tokenService.HashRefreshToken(tokenDto.RefreshToken);
                _dbContext.RefreshTokens.Add(new RefreshToken
                {
                    UserId = user.Id,
                    TokenHash = newHashed,
                    Expiration = DateTimeOffset.UtcNow.AddDays(_jwtConfig.Value.RefreshTokenExpirationDays),
                    CreatedAt = DateTimeOffset.UtcNow
                });

                await _dbContext.SaveChangesAsync();

                var roleNames = await _userManager.GetRolesAsync(user);
                var userRoles = new List<UserRoleDto>();
                foreach (var roleName in roleNames)
                {
                    var permResult = await permissionService.GetPermissionsForRoleAsync(roleName);
                    var permissions = permResult.Data ?? new List<string>();

                    userRoles.Add(new UserRoleDto
                    {
                        RoleName = roleName,
                        Permissions = permissions.Select(p => new UserPermissionsDto { PermissionName = p }).ToList()
                    });
                }

                var userInfo = new UserLogedData
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    NationalId = user.NationalId ?? 0,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AvatarUrl = user.AvatarUrl,
                    DateOfBirth = user.DOB ?? DateTimeOffset.UtcNow,
                    Specialization = user.Specialization,
                    CountryCode = user.CountryCode,
                    UserRoleAndPermissions = userRoles
                };

                tokenDto.userLogedData = userInfo;

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TokenRefreshedSuccessfully"),
                    Data = tokenDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error accurred while refreshing token.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorRefreshingToken"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> LogoutAsync(string userId, string? refreshToken)
        {
            const string method = nameof(LogoutAsync);

            try
            {
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    var hashed = _tokenService.HashRefreshToken(refreshToken);

                    var token = await _dbContext.RefreshTokens
                        .FirstOrDefaultAsync(r =>
                            r.UserId == userId &&
                            r.TokenHash == hashed);

                    if (token == null)
                    {
                        _logger.LogWarning("{Method}: Logout failed. Refresh token not found for user {UserId}.", method, userId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = _localizationManager.GetLocalizedString("RefreshTokenNotFound"),
                            Data = null,
                            ErrorType = ErrorType.NotFound
                        };
                    }

                    if (token.IsUsed || token.IsRevoked)
                    {
                        _logger.LogWarning("{Method}: Refresh token already used or revoked for user {UserId}.", method, userId);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = _localizationManager.GetLocalizedString("RefreshTokenAlreadyRevoked"),
                            Data = null,
                            ErrorType = ErrorType.InvalidData
                        };
                    }

                    token.IsUsed = true;
                    token.IsRevoked = true;
                    _dbContext.RefreshTokens.Update(token);

                    await _dbContext.SaveChangesAsync();
                }

                await _signInManager.SignOutAsync();

                _logger.LogInformation("{Method}: Logout successful for user {UserId}.", method, userId);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("LogoutSuccess"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Unexpected error occurred while logging out user {UserId}.", method, userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("LogoutError"),
                    Data = null,
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }
    }
}


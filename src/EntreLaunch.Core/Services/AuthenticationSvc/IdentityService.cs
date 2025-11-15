namespace EntreLaunch.Services.AuthenticationSvc
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<IdentityService> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly PgDbContext _dbContext;
        private readonly IOptions<JwtConfig> _jwtConfig;
        private readonly ITokenService _tokenService;
        public IdentityService(
            UserManager<User> userManager,
            ILogger<IdentityService> logger,
            SignInManager<User> signInManager,
            PgDbContext dbContext,
            IOptions<JwtConfig> jwtConfig,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _jwtConfig = jwtConfig;
            _tokenService = tokenService;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<User>> FindOnRegister(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("Email cannot be null or empty.");
                    return new GeneralResult<User>(false, "Email cannot be null or empty.", null);
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
                        return new GeneralResult<User>(false, "Failed to create user.", null);
                    }

                    _logger.LogInformation("User with email {Email} created successfully.", email);
                    return new GeneralResult<User>(true, "User created successfully.", user);
                }

                return new GeneralResult<User>(true, "User found successfully.", user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during FindOnRegister for email {Email}.", email);
                return new GeneralResult<User>(false, "Error during FindOnRegister.", null);
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
                    return new GeneralResult<ClaimsPrincipal>(false, "User cannot be null.", null);
                }

                var claims = await CreateUserClaims(user);
                if (claims.Data == null)
                {
                    _logger.LogWarning("Claims cannot be null.");
                    return new GeneralResult<ClaimsPrincipal>(false, "Claims cannot be null.", null);
                }

                var identity = new ClaimsIdentity(claims.Data);
                _logger.LogInformation("ClaimsPrincipal created successfully for user {UserId}.", user.Id);
                return new GeneralResult<ClaimsPrincipal>(true, "ClaimsPrincipal created successfully.", new ClaimsPrincipal(identity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ClaimsPrincipal for user {UserId}.", user?.Id);
                return new GeneralResult<ClaimsPrincipal>(false, "Error creating ClaimsPrincipal.", null);
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
                    return new GeneralResult<List<Claim>>(false, "User cannot be null.", null);
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
                return new GeneralResult<List<Claim>>(true, "Claims created successfully.", claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error while creating claims for user {UserId}.", user?.Id);
                return new GeneralResult<List<Claim>>(false, "Unexpected error while creating claims.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null || user.IsDeleted)
                {
                    _logger.LogWarning("Login failed. User not found or deactivated for email: {Email}", dto.Email);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Login failed. User not found or deactivated.",
                        Data = null
                    };
                }

                if (!user.EmailConfirmed)
                {
                    _logger.LogInformation("Email is not confirmed for user {Email}.", dto.Email);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Email is not confirmed.",
                        Data = null
                    };
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Account locked for user {Email}.", dto.Email);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Account locked.",
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
                            Message = "Too many requests for login.",
                            Data = null
                        };
                    }
                    else
                    {
                        _logger.LogWarning("UnExpected error while login for user {Email}.", dto.Email);
                        return new GeneralResult
                        {
                            IsSuccess = false,
                            Message = "UnExpected error while login.",
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
                    Expiration = DateTimeOffset.UtcNow.AddDays(_jwtConfig.Value.RefreshTokenExpirationDays),
                    CreatedAt = DateTimeOffset.UtcNow
                });

                await _dbContext.SaveChangesAsync();

                var userInfo = new UserLogedData
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    NationalId = user.NationalId ?? 0,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AvatarUrl = user.AvatarUrl,
                    DateOfBirth = user.DOB ?? DateTimeOffset.Now,
                    Specialization = user.Specialization,
                    CountryCode = user.CountryCode,
                };

                tokenDto.userLogedData = userInfo;

                _logger.LogInformation("Login successful for user {Email}.", dto.Email);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Login successful.",
                    Data = tokenDto,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}.", dto.Email);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error during login.",
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
                        Message = "Refresh token is required.",
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
                        Message = "Invalid refresh token.",
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
                        Message = "User not found.",
                        Data = null
                    };
                }

                // disable the old refresh token
                stored.IsUsed = true;
                stored.IsRevoked = true;
                _dbContext.RefreshTokens.EntreLaunchdate(stored);

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
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Token refreshed successfully.",
                    Data = tokenDto,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error accurred while refreshing token.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error accurred while refreshing token.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> LogoutAsync(string userId, string? refreshToken)
        {
            try
            {
                // Revoke the Refresh Token sent.
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    var hashed = _tokenService.HashRefreshToken(refreshToken);
                    var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(r =>
                        r.UserId == userId && r.TokenHash == hashed && !r.IsUsed && !r.IsRevoked);

                    if (token != null)
                    {
                        token.IsUsed = true;
                        token.IsRevoked = true;
                        _dbContext.RefreshTokens.EntreLaunchdate(token);
                        await _dbContext.SaveChangesAsync();
                    }
                }

                // If you have Cookie/Session authentication
                // (e.g. when using SignInManager)
                await _signInManager.SignOutAsync();
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Logout successful.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error accurred while logging out user {UserId}.", userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error accurred while logging out.",
                    Data = null
                };
            }
        }
    }
}


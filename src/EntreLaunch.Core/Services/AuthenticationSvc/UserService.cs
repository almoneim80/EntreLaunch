namespace EntreLaunch.Services.AuthenticationSvc
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IRoleService _roleService;
        private readonly DefaultRolesConfig _defaultRoles;
        private readonly ILogger<UserService> _logger;
        private readonly PgDbContext _dbContext;
        private readonly IOtpService _otpService;
        private readonly ICacheService _cacheService;
        public UserService(
            UserManager<User> userManager,
            IMapper mapper,
            ILogger<UserService> logger,
            IRoleService roleService,
            DefaultRolesConfig defaultRoles,
            PgDbContext dbContext,
            IOtpService otpService,
            ICacheService cacheService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _roleService = roleService;
            _defaultRoles = defaultRoles;
            _dbContext = dbContext;
            _otpService = otpService;
            _cacheService = cacheService;
        }

        //// <inheritdoc />
        public async Task<GeneralResult> CreateUserAsync(UserCreateDto value)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var newUser = _mapper.Map<User>(value);
                newUser.UserName = value.Email;
                newUser.CreatedAt = DateTime.UtcNow;
                newUser.IsDeleted = false;
                newUser.EmailConfirmed = true;

                var createResult = await _userManager.CreateAsync(newUser, value.Password!);
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                    {
                        _logger.LogError("Error: {Code} - {Description}", error.Code, error.Description);
                    }

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Error creating user.",
                        Data = createResult.Errors,
                    };
                }

                await _roleService.AssignRoleAsync(newUser.Id, _defaultRoles[0]);

                //try
                //{
                //    var (otp, expireAt) = await _otpService.GenerateAndSendOtpAsync(newUser.Id, value.PhoneNumber!);
                //    _logger.LogInformation("OTP {Otp} sent to {PhoneNumber} and will expire at {ExpireAt}", otp, value.PhoneNumber, expireAt);
                //}
                //catch (Exception ex)
                //{
                //    _logger.LogError(ex, "Failed to send OTP to {PhoneNumber}.", value.PhoneNumber);
                //    return new GeneralResult
                //    {
                //        IsSuccess = false,
                //        Message = "Failed to send OTP.",
                //        Data = ex,
                //    };
                //}

                await transaction.CommitAsync();
                _logger.LogInformation("User {UserId} created successfully.", newUser.Id);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "User created successfully.",
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating user.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error creating user.",
                    Data = ex,
                };
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> EntreLaunchdateUserAsync(User existingEntity, UserEntreLaunchdateDto value)
        {
            try
            {
                _mapper.Map(value, existingEntity);
                existingEntity.EntreLaunchdatedAt = DateTime.UtcNow;
                var result = await _userManager.EntreLaunchdateAsync(existingEntity);

                if (result.Errors.Any())
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("EntreLaunchdateUser: Error {Code} - {Description}", error.Code, error.Description);
                    }

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Error EntreLaunchdating user.",
                        Data = result.Errors,
                    };
                }

                _logger.LogInformation("User {UserId} EntreLaunchdated successfully.", existingEntity.Id);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "User EntreLaunchdated successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error EntreLaunchdating user {UserId}.", existingEntity?.Id);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error EntreLaunchdating user.",
                    Data = ex
                };
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> CompleteUserAsync(User existingEntity, CompleteUserDetailsDto value)
        {
            try
            {
                _mapper.Map(value, existingEntity);
                var result = await _userManager.EntreLaunchdateAsync(existingEntity);
                if (result.Errors.Any())
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("CompleteUserDetails: Error {Code} - {Description}", error.Code, error.Description);
                    }

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Error completing details.",
                        Data = result.Errors,
                    };
                }

                _logger.LogInformation("User {UserId} completed details successfully.", existingEntity.Id);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "User completed details successfully.",
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing details for user {UserId}.", existingEntity?.Id);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error completing details.",
                    Data = ex,
                };
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> DeleteUserAsync(User existingEntity)
        {
            try
            {
                if (existingEntity.IsDeleted)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User is already deleted.",
                        Data = null,
                    };
                }

                existingEntity.IsDeleted = true;
                existingEntity.DeletedAt = DateTimeOffset.UtcNow;

                var result = await _userManager.EntreLaunchdateAsync(existingEntity);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("DeleteUser: Error {Code} - {Description}", error.Code, error.Description);
                    }

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Error deleting user.",
                        Data = result.Errors,
                    };
                }

                _logger.LogInformation("User {UserId} marked as deleted successfully.", existingEntity.Id);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "User deleted successfully.",
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}.", existingEntity?.Id);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error deleting user.",
                    Data = ex,
                };
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> ResetPasswordAsync(string email, string newPassword, string resetToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("ResetPassword: No user found with email {Email}", email);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found.",
                        Data = null,
                    };
                }

                if (user.IsDeleted)
                {
                    _logger.LogWarning("ResetPassword: Attempt to reset password for a deleted user with email {Email}", email);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User is deleted.",
                        Data = null,
                    };
                }

                var isTokenValid = await _userManager.VerifyUserTokenAsync(
                    user,
                    _userManager.Options.Tokens.PasswordResetTokenProvider,
                    "ResetPassword",
                    resetToken);

                if (!isTokenValid)
                {
                    _logger.LogWarning("ResetPassword: Invalid reset token for user with email {Email}", email);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Invalid reset token.",
                        Data = null,
                    };
                }

                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

                if (!resetResult.Succeeded)
                {
                    foreach (var error in resetResult.Errors)
                    {
                        _logger.LogError("ResetPassword: Error resetting password for user {Email} - {Code}: {Description}", email, error.Code, error.Description);
                    }

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Error resetting password.",
                        Data = resetResult.Errors,
                    };
                }

                _logger.LogInformation("ResetPassword: Password reset successfully for user {Email}", email);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Password reset successfully.",
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user with email {Email}.", email);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error resetting password.",
                    Data = ex,
                };
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> VerifyOtpAsync(string userId, string inputOtp)
        {
            try
            {
                // Retrieve stored code
                var storedOtp = await _cacheService.GetAsync<string>($"otp_{userId}");
                if (storedOtp == null)
                {
                    _logger.LogWarning("No OTP found for user {UserId}", userId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No OTP found.",
                        Data = null,
                    };
                }

                // Checking the input code
                if (storedOtp != inputOtp)
                {
                    _logger.LogWarning("Invalid OTP entered for user {UserId}", userId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Invalid OTP.",
                        Data = null,
                    };
                }

                // EntreLaunchdate Phone Status
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogError($"No user found with this id: {userId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found.",
                        Data = null
                    };
                }

                // Phone Confirmation
                user.PhoneNumberConfirmed = true;
                var EntreLaunchdateResult = await _userManager.EntreLaunchdateAsync(user);

                if (!EntreLaunchdateResult.Succeeded)
                {
                    foreach (var error in EntreLaunchdateResult.Errors)
                    {
                        _logger.LogError("Error confirming phone number for user {UserId}: {Code} - {Description}", userId, error.Code, error.Description);
                    }

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Error confirming phone number.",
                        Data = EntreLaunchdateResult.Errors,
                    };
                }

                // Deleting code from cash
                _cacheService.Remove($"otp_{userId}");
                _logger.LogInformation("OTP verified and phone number confirmed for user {UserId}", userId);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "OTP verified and phone number confirmed.",
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for user {UserId}", userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error verifying OTP.",
                    Data = ex,
                };
            }
        }

        //// <inheritdoc />
        public async Task ResendOtpAsync(string userId, string phoneNumber)
        {
            try
            {
                // Call ResendOtpAsync from SmsOtpService
                var (otp, expireAt) = await _otpService.ResendOtpAsync(userId, phoneNumber);
                _logger.LogInformation("Successfully resent OTP {Otp} to {PhoneNumber}, expires at {ExpireAt}", otp, phoneNumber, expireAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending OTP for user {UserId}.", userId);
                throw;
            }
        }

        //// <inheritdoc />
        public async Task<GeneralResult> ToggleUserActiveStatusAsync(string userId, bool isActive, string reason)
        {
            try
            {
                // Getting the user
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found or is deleted.", userId);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found or is deleted.",
                        Data = null
                    };
                }

                // Check the current state of the user
                if (user.IsActive == isActive)
                {
                    // If the desired state is the same as the current one, there is no need to EntreLaunchdate
                    _logger.LogInformation("User with ID {UserId} is already in the desired state (IsActive = {IsActive}).", userId, user.IsActive);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = isActive ? "User is already active." : "User is already deactivated.",
                        Data = null
                    };
                }

                // Status EntreLaunchdate
                user.IsActive = isActive;
                user.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                user.AdditionalData = isActive ? "Active reason" + reason : "Deactivate reason" + reason;

                // Saving adjustments
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("User with ID {UserId} has been {State}. Reason: {Reason}", userId, isActive ? "activated" : "deactivated", reason);

                // Return the result
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = isActive ? "User has been activated successfully." : "User has been deactivated successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while toggling active status for user {UserId}", userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Error toggling active status.",
                    Data = ex
                };
            }
        }
    }
}

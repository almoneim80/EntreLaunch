using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.DTOs.UserDtos;

namespace EntreLaunch.Services.AuthenticationSvc
{
    public class UserService(
        UserManager<User> userManager,
        IMapper mapper,
        ILogger<UserService> logger,
        IRoleService roleService,
        DefaultRolesConfig defaultRoles,
        PgDbContext dbContext,
        IOtpService otpService,
        ICacheService cacheService,
        ILocalizationManager localizationManager) : IUserService
    {
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IRoleService _roleService = roleService;
        private readonly DefaultRolesConfig _defaultRoles = defaultRoles;
        private readonly ILogger<UserService> _logger = logger;
        private readonly PgDbContext _dbContext = dbContext;
        private readonly IOtpService _otpService = otpService;
        private readonly ICacheService _cacheService = cacheService;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
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
                        Message = _localizationManager.GetLocalizedString("ErrorCreatingUser"),
                        Data = createResult.Errors,
                    };
                }

                await _roleService.AssignRoleAsync(newUser.Id, AppRoles.Entrepreneur);

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

                var userDetails = new UserDetailsDto
                {
                    Id = newUser.Id,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    NationalId = newUser.NationalId,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    AvatarUrl = newUser.AvatarUrl,
                    DOB = newUser.DOB,
                    Description = newUser.Description,
                    Specialization = newUser.Specialization,
                    CountryCode = newUser.CountryCode
                };

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("UserCreatedSuccessfully"),
                    Data = userDetails,
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating user.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorCreatingUser"),
                    Data = ex,
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateUserAsync(User existingEntity, UserUpdateDto value)
        {
            try
            {
                _mapper.Map(value, existingEntity);
                existingEntity.UpdatedAt = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(existingEntity);

                if (result.Errors.Any())
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("UpdateUser: Error {Code} - {Description}", error.Code, error.Description);
                    }

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ErrorUpdatingUser"),
                        Data = result.Errors,
                    };
                }

                _logger.LogInformation("User {UserId} updated successfully.", existingEntity.Id);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("UserUpdatedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}.", existingEntity?.Id);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorUpdatingUser"),
                    Data = ex
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CompleteUserAsync(User existingEntity, CompleteUserDetailsDto value)
        {
            try
            {
                _mapper.Map(value, existingEntity);
                var result = await _userManager.UpdateAsync(existingEntity);
                if (result.Errors.Any())
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("CompleteUserDetails: Error {Code} - {Description}", error.Code, error.Description);
                    }

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ErrorCompletingDetails"),
                        Data = result.Errors,
                    };
                }

                _logger.LogInformation("User {UserId} completed details successfully.", existingEntity.Id);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("UserCompletedSuccessfully"),
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing details for user {UserId}.", existingEntity?.Id);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorCompletingDetails"),
                    Data = ex,
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteUserAsync(User existingEntity)
        {
            try
            {
                if (existingEntity.IsDeleted)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserAlreadyDeleted"),
                        Data = null,
                    };
                }

                existingEntity.IsDeleted = true;
                existingEntity.DeletedAt = DateTimeOffset.UtcNow;

                var result = await _userManager.UpdateAsync(existingEntity);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("DeleteUser: Error {Code} - {Description}", error.Code, error.Description);
                    }

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ErrorDeletingUser"),
                        Data = result.Errors,
                    };
                }

                _logger.LogInformation("User {UserId} marked as deleted successfully.", existingEntity.Id);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("UserDeletedSuccessfully"),
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}.", existingEntity?.Id);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorDeletingUser"),
                    Data = ex,
                };
            }
        }

        /// <inheritdoc />
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
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null,
                    };
                }

                if (user.IsDeleted)
                {
                    _logger.LogWarning("ResetPassword: Attempt to reset password for a deleted user with email {Email}", email);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserIsDeleted"),
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
                        Message = _localizationManager.GetLocalizedString("InvalidResetToken"),
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
                        Message = _localizationManager.GetLocalizedString("ErrorResettingPassword"),
                        Data = resetResult.Errors,
                    };
                }

                _logger.LogInformation("ResetPassword: Password reset successfully for user {Email}", email);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("PasswordResetSuccessfully"),
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user with email {Email}.", email);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorResettingPassword"),
                    Data = ex,
                };
            }
        }

        /// <inheritdoc />
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
                        Message = _localizationManager.GetLocalizedString("NoOtpFound"),
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
                        Message = _localizationManager.GetLocalizedString("InvalidOtp"),
                        Data = null,
                    };
                }

                // Update Phone Status
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogError($"No user found with this id: {userId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                // Phone Confirmation
                user.PhoneNumberConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        _logger.LogError("Error confirming phone number for user {UserId}: {Code} - {Description}", userId, error.Code, error.Description);
                    }

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ErrorConfirmingPhoneNumber"),
                        Data = updateResult.Errors,
                    };
                }

                // Deleting code from cash
                _cacheService.Remove($"otp_{userId}");
                _logger.LogInformation("OTP verified and phone number confirmed for user {UserId}", userId);
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("OtpVerifiedSuccessfully"),
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for user {UserId}", userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorVerifyingOtp"),
                    Data = ex,
                };
            }
        }

        /// <inheritdoc />
        public async Task ResendOtpAsync(string userId, string phoneNumber)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found or is deleted.", userId);
                    throw new Exception("User not found or is deleted.");
                }

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

        /// <inheritdoc />
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
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                // Check the current state of the user
                if (user.IsActive == isActive)
                {
                    // If the desired state is the same as the current one, there is no need to update
                    _logger.LogInformation("User with ID {UserId} is already in the desired state (IsActive = {IsActive}).", userId, user.IsActive);
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = isActive ? _localizationManager.GetLocalizedString("UserActivatedSuccessfully") : _localizationManager.GetLocalizedString("UserDeactivatedSuccessfully"),
                        Data = null
                    };
                }

                // Status update
                user.IsActive = isActive;
                user.UpdatedAt = DateTimeOffset.UtcNow;
                user.AdditionalData = isActive ? "Active reason" + reason : "Deactivate reason" + reason;

                // Saving adjustments
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("User with ID {UserId} has been {State}. Reason: {Reason}", userId, isActive ? "activated" : "deactivated", reason);

                // Return the result
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = isActive ? _localizationManager.GetLocalizedString("UserActivatedSuccessfully") : _localizationManager.GetLocalizedString("UserDeactivatedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while toggling active status for user {UserId}", userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ErrorTogglingActiveStatus"),
                    Data = ex
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<UserDetailsDto>>> GetAllUsersAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _userManager.Users
                    .Where(u => !u.IsDeleted)
                    .AsQueryable();

                var totalCount = await query.CountAsync(cancellationToken);
                if (totalCount == 0)
                {
                    _logger.LogWarning("No users found.");
                    return new GeneralResult<PaginatedResult<UserDetailsDto>>(false, _localizationManager.GetLocalizedString("NoUsersFound"), null);
                }

                var users = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                var mapped = _mapper.Map<List<UserDetailsDto>>(users);

                var result = new PaginatedResult<UserDetailsDto>
                {
                    Items = mapped,
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount
                };

                return new GeneralResult<PaginatedResult<UserDetailsDto>>(true, _localizationManager.GetLocalizedString("UsersRetrieved"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users.");
                return new GeneralResult<PaginatedResult<UserDetailsDto>>(false, _localizationManager.GetLocalizedString("ErrorRetrievingUsers"), null);
            }
        }
    }
}

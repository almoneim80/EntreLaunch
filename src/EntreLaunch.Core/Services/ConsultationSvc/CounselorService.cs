using AutoMapper.QueryableExtensions;
using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ConsultationDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.ConsultationSvc
{
    public class CounselorService(
        ILogger<CounselorService> logger,
        IMapper mapper,
        PgDbContext pgDbContext,
        UserManager<User> userManager,
        IRoleService roleService,
        DefaultRolesConfig defaultRoles,
        ILocalizationManager localizationManager) : ICounselorService
    {
        private readonly ILogger<CounselorService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly PgDbContext _dbContext = pgDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IRoleService _roleService = roleService;
        private readonly DefaultRolesConfig _defaultRoles = defaultRoles;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> IsCounselor(int id)
        {
            try
            {
                if (id == 0)
                {
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("IdIsRequired"), false);
                }

                var counselor = _dbContext.Counselors.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
                if (counselor == null)
                {
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("CounselorNotFound"), false);
                }

                var result = await _roleService.IsUserInRoleAsync(counselor.UserId, "Counselor");
                if (result.IsSuccess == false)
                {
                    return new GeneralResult<bool>(false, result.Message, false);
                }

                return new GeneralResult<bool>(true, _localizationManager.GetLocalizedString("CounselorFound"), true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("UnexpectedErrorCheckingCounselor"), false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> SubmitCounselorApplication(CreateCounselorRequestDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AllDataIsRequired"),
                        Data = null
                    };
                }

                var user = await _userManager.FindByIdAsync(dto.UserId);
                if (user == null)
                {
                    _logger.LogError($"No user found with this id: {dto.UserId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                if (user.Specialization == null || user.DOB == null || user.CountryCode == 0)
                {
                    _logger.LogError($"user profile {user!.Id} not completed.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("IncompleteUserProfile"),
                        Data = null
                    };
                }

                var counselorRequestMap = _mapper.Map<Counselor>(dto);
                counselorRequestMap.CreatedAt = DateHelper.UtcNow;
                counselorRequestMap.Status = CounselorRequesttStatus.Pending;
                counselorRequestMap.IsDeleted = false;
                counselorRequestMap.Active = false;
                await _dbContext.Counselors.AddAsync(counselorRequestMap);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("CounselorRequestSent"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending counselor request.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorSendingRequest"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateCounselorApplicationStatus(ProcessCounselorRequestDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (dto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AllDataIsRequired"),
                        Data = null
                    };
                }

                var counselorRequest = _dbContext.Counselors.FirstOrDefault(o => o.Id == dto.Id && !o.IsDeleted);

                if (counselorRequest == null)
                {
                    _logger.LogError($"No counselor request with Id {dto.Id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoDataFound"),
                        Data = null
                    };
                }

                if (counselorRequest.Status == dto.Status)
                {
                    _logger.LogError($"Counselor request with Id {dto.Id} is already {dto.Status}");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("RequestAlreadyInStatus"),
                        Data = null
                    };
                }

                counselorRequest.Status = dto.Status;
                counselorRequest.UpdatedAt = DateHelper.UtcNow;
                if (dto.Status == CounselorRequesttStatus.Accepted)
                {
                    counselorRequest.Active = true;
                }

                if (dto.Status == CounselorRequesttStatus.Rejected)
                {
                    counselorRequest.Active = false;
                }

                _dbContext.Counselors.Update(counselorRequest);
                _dbContext.SaveChanges();

                if (dto.Status == CounselorRequesttStatus.Accepted)
                {
                    var result = await _roleService.IsUserInRoleAsync(counselorRequest.UserId, "Counselor");
                    if (result.IsSuccess == false)
                    {
                        await _roleService.AssignRoleAsync(counselorRequest.UserId, AppRoles.Counselor);
                    }
                }

                await transaction.CommitAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("CounselorRequestUpdated"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while processing counselor request.");
                await transaction.RollbackAsync();
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorProcessingRequest"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>> GetAllCounselorApplications(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = GetAllActiveCounselorsQuery()
                    .Select(c => new CounselorRequestDetailsDto
                    {
                        Id = c.Id,
                        FullName = c.User.FirstName + " " + c.User.LastName,
                        NationalId = c.User.NationalId,
                        Specialization = c.User.Specialization,
                        CountryCode = c.User.CountryCode,
                        Email = c.User.Email,
                        PhoneNumber = c.User.PhoneNumber,
                        DateOfBirth = c.User.DOB,
                        Qualification = c.Qualification,
                        City = c.City,
                        SpecializationExperience = c.SpecializationExperience,
                        ConsultingExperience = c.ConsultingExperience,
                        DailyHours = c.DailyHours,
                        SocialMediaAccounts = c.SocialMediaAccounts,
                        Status = c.Status,
                        CreatedAt = c.CreatedAt,
                        counselorTimeData = c.ConsultationTimes != null ? c.ConsultationTimes 
                            .Where(t => !t.IsDeleted)
                            .Select(t => new CounselorTimeDataDto
                            {
                                DateTimeSlot = t.DateTimeSlot,
                                IsBooked = t.IsBooked
                            }).ToList() : new List<CounselorTimeDataDto>()
                    }
                    );

                var paginated = await query.ToPagedResultAsync(pagination, cancellationToken);

                if (!paginated.Items.Any())
                {
                    _logger.LogInformation("No counselors found.");
                    return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                        false, _localizationManager.GetLocalizedString("NoCounselorsFound"), null);
                }

                _logger.LogInformation("Counselors found.");
                return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                    true, _localizationManager.GetLocalizedString("CounselorsFound"), paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting counselors.");
                return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                    false, _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingCounselors"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>> GetCounselorRequestsBasedOnStatus(CounselorRequesttStatus status, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Counselors
                    .Include(c => c.User)
                    .Include(c => c.ConsultationTimes)
                    .Where(c => !c.IsDeleted && c.Status == status)
                    .Select(c => new CounselorRequestDetailsDto
                    {
                        Id = c.Id,
                        FullName = c.User.FirstName + " " + c.User.LastName,
                        NationalId = c.User.NationalId,
                        Specialization = c.User.Specialization,
                        CountryCode = c.User.CountryCode,
                        Email = c.User.Email,
                        PhoneNumber = c.User.PhoneNumber,
                        DateOfBirth = c.User.DOB,
                        Qualification = c.Qualification,
                        City = c.City,
                        SpecializationExperience = c.SpecializationExperience,
                        ConsultingExperience = c.ConsultingExperience,
                        DailyHours = c.DailyHours,
                        SocialMediaAccounts = c.SocialMediaAccounts,
                        Status = c.Status,
                        CreatedAt = c.CreatedAt,
                        counselorTimeData = c.ConsultationTimes != null ? c.ConsultationTimes
                            .Where(t => !t.IsDeleted)
                            .Select(t => new CounselorTimeDataDto
                            {
                                DateTimeSlot = t.DateTimeSlot,
                                IsBooked = t.IsBooked
                            }).ToList() : new List<CounselorTimeDataDto>()
                    }
                    );

                var paginated = await query.ToPagedResultAsync(pagination, cancellationToken);

                if (!paginated.Items.Any())
                {
                    _logger.LogInformation("No counselors found for status: {Status}", status);
                    return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                        false, _localizationManager.GetLocalizedString("NoPendingCounselorsFound"), null);
                }

                _logger.LogInformation("Counselors found for status: {Status}", status);
                return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                    true, _localizationManager.GetLocalizedString("PendingCounselorsFound"), paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting counselors by status.");
                return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                    false, _localizationManager.GetLocalizedString("ErrorGettingCounselors"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>> GetAllActiveCounselors(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Counselors
                    .Where(c => c.Active)
                    .Include(c => c.User)
                    .Include(c => c.ConsultationTimes)
                    .Where(c => !c.IsDeleted && c.Active)
                    .Select(c => new CounselorRequestDetailsDto
                    {
                        Id = c.Id,
                        FullName = c.User.FirstName + " " + c.User.LastName,
                        NationalId = c.User.NationalId,
                        Specialization = c.User.Specialization,
                        CountryCode = c.User.CountryCode,
                        Email = c.User.Email,
                        PhoneNumber = c.User.PhoneNumber,
                        DateOfBirth = c.User.DOB,
                        Qualification = c.Qualification,
                        City = c.City,
                        SpecializationExperience = c.SpecializationExperience,
                        ConsultingExperience = c.ConsultingExperience,
                        DailyHours = c.DailyHours,
                        SocialMediaAccounts = c.SocialMediaAccounts,
                        Status = c.Status,
                        CreatedAt = c.CreatedAt, 
                        counselorTimeData = c.ConsultationTimes != null ? c.ConsultationTimes
                            .Where(t => !t.IsDeleted)
                            .Select(t => new CounselorTimeDataDto
                            {
                                DateTimeSlot = t.DateTimeSlot,
                                IsBooked = t.IsBooked
                            }).ToList() : new List<CounselorTimeDataDto>()
                    }
                    );

                var paginated = await query.ToPagedResultAsync(pagination, cancellationToken);

                if (!paginated.Items.Any())
                {
                    _logger.LogInformation("No active counselors found.");
                    return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                        false, _localizationManager.GetLocalizedString("NoCounselorsForSpecialization"), null);
                }

                return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                    true, _localizationManager.GetLocalizedString("CounselorsRetrievedSuccessfully"), paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving active counselors.");
                return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                    false, _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingCounselors"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>> GetCounselorsBySpecialization(string specialization, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Counselors
                    .Include(c => c.User)
                    .Include(c => c.ConsultationTimes)
                    .Where(c => !c.IsDeleted && c.Active && c.User.Specialization == specialization)
                    .Select(c => new CounselorRequestDetailsDto
                    {
                        Id = c.Id,
                        FullName = c.User.FirstName + " " + c.User.LastName,
                        NationalId = c.User.NationalId,
                        Specialization = c.User.Specialization,
                        CountryCode = c.User.CountryCode,
                        Email = c.User.Email,
                        PhoneNumber = c.User.PhoneNumber,
                        DateOfBirth = c.User.DOB,
                        Qualification = c.Qualification,
                        City = c.City,
                        SpecializationExperience = c.SpecializationExperience,
                        ConsultingExperience = c.ConsultingExperience,
                        DailyHours = c.DailyHours,
                        SocialMediaAccounts = c.SocialMediaAccounts,
                        Status = c.Status,
                        CreatedAt = c.CreatedAt,
                        counselorTimeData = c.ConsultationTimes != null ? c.ConsultationTimes
                            .Where(t => !t.IsDeleted)
                            .Select(t => new CounselorTimeDataDto
                            {
                                DateTimeSlot = t.DateTimeSlot,
                                IsBooked = t.IsBooked
                            }).ToList() : new List<CounselorTimeDataDto>()
                    }
                    );

                var paginated = await query.ToPagedResultAsync(pagination, cancellationToken);

                if (!paginated.Items.Any())
                {
                    _logger.LogInformation("No counselors for specialization {Specialization}.", specialization);
                    return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                        false, _localizationManager.GetLocalizedString("NoCounselorsForSpecialization"), null);
                }

                return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                    true, _localizationManager.GetLocalizedString("CounselorsBySpecializationRetrieved"), paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving counselors by specialization.");
                return new GeneralResult<PaginatedResult<CounselorRequestDetailsDto>>(
                    false, _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingSpecialization"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CounselorRequestDetailsDto>>> GetCounselorProfileById(int id)
        {
            try
            {
                var counselors = await _dbContext.Counselors
                .Include(c => c.User)
                .Where(c => c.Id == id && !c.IsDeleted && c.Active)
                .Select(c => new CounselorRequestDetailsDto
                {
                    // user date
                    Id = c.Id,
                    FullName = c.User.FirstName + " " + c.User.LastName,
                    NationalId = c.User.NationalId,
                    Specialization = c.User.Specialization,
                    CountryCode = c.User.CountryCode,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    DateOfBirth = c.User.DOB,

                    // Counselor data
                    Qualification = c.Qualification,
                    City = c.City,
                    SpecializationExperience = c.SpecializationExperience,
                    ConsultingExperience = c.ConsultingExperience,
                    DailyHours = c.DailyHours,
                    SocialMediaAccounts = c.SocialMediaAccounts,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
                }).ToListAsync();

                if (!counselors.Any())
                {
                    _logger.LogInformation("No counselor found.");
                    return new GeneralResult<List<CounselorRequestDetailsDto>>(false, _localizationManager.GetLocalizedString("NoCounselorFound"), null);
                }

                _logger.LogInformation("Retrieved counselors successfully.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(true, _localizationManager.GetLocalizedString("CounselorsRetrievedSuccessfully"), counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving counselors.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(false, _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingCounselors"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<string>>> GetAllCounselorSpecializations()
        {
            try
            {
                var specialization = await _dbContext.Counselors
                .Include(c => c.User)
                .Select(c => c.User.Specialization).Distinct().ToListAsync();

                if (!specialization.Any())
                {
                    _logger.LogError("No specialization found.");
                    return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("NoSpecializationFound"), null);
                }

                return new GeneralResult<List<string>>(true, _localizationManager.GetLocalizedString("CounselorSpecializationsRetrieved"), specialization!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving counselor specializations.");
                return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("UnexpectedErrorInCounselorSpecializations"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CreateAvailableTimeSlot(ConsultationTimeCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AllDataIsRequired"),
                        Data = null
                    };
                }

                var counselor = await _dbContext.Counselors
                    .FirstOrDefaultAsync(c => c.Id == dto.CounselorId && !c.IsDeleted && c.Active);

                if (counselor == null)
                {
                    _logger.LogError($"This counselor {dto.CounselorId} does not exist or is not active.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("CounselorNotFoundOrInactive"),
                        Data = null
                    };
                }

                var isTimeExists = await _dbContext.ConsultationTimes.AnyAsync(ct =>
                    ct.CounselorId == dto.CounselorId &&
                    ct.DateTimeSlot.UtcDateTime.Date == dto.DateTimeSlot.UtcDateTime.Date &&
                    ct.DateTimeSlot.UtcDateTime.Hour == dto.DateTimeSlot.UtcDateTime.Hour);

                if (isTimeExists)
                {
                    _logger.LogError("This time already exists.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("TimeSlotAlreadyExists"),
                        Data = null
                    };
                }

                var consultationTimeMap = _mapper.Map<ConsultationTime>(dto);
                consultationTimeMap.CreatedAt = DateHelper.UtcNow;
                consultationTimeMap.IsBooked = false;
                consultationTimeMap.IsDeleted = false;
                await _dbContext.ConsultationTimes.AddAsync(consultationTimeMap);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Consultation time with id {consultationTimeMap.Id} added successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("ConsultationTimeAddedSuccessfully"),
                    Data = _mapper.Map<ConsultationTimeDetailsDto>(consultationTimeMap)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating consultation time.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorWhileCreatingConsultationTime"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateAvailableTimeSlot(int id, ConsultationTimeUpdateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AllDataIsRequired"),
                        Data = null
                    };
                }

                var consultationTime = _dbContext.ConsultationTimes.FirstOrDefault(o => o.Id == id && !o.IsDeleted && !o.IsBooked);
                if (consultationTime == null)
                {
                    _logger.LogError($"No consultation time found with Id {id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoDataFoundOrTimeAlreadyBooked"),
                        Data = null
                    };
                }

                var consultationTimeMap = _mapper.Map<ConsultationTime>(dto);
                consultationTimeMap.UpdatedAt = DateHelper.UtcNow;
                _dbContext.ConsultationTimes.Update(consultationTimeMap);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Consultation time with id {consultationTimeMap.Id} edited successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("ConsultationTimeEditedSuccessfully"),
                    Data = _mapper.Map<ConsultationTimeDetailsDto>(consultationTimeMap)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while editing consultation time.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorWhileEditingConsultationTime"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<ConsultationTimeDetailsDto>>> GetAvailableTimeSlotsByCounselor(int counselorId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var consultationTimesQuery = _dbContext.ConsultationTimes
                    .AsNoTracking()
                    .Where(ct => ct.CounselorId == counselorId && !ct.IsDeleted);

                var pagedResult = await consultationTimesQuery
                    .ProjectTo<ConsultationTimeDetailsDto>(_mapper.ConfigurationProvider)
                    .ToPagedResultAsync(pagination, cancellationToken);

                if (!pagedResult.Items.Any())
                {
                    _logger.LogWarning("No consultation times found for counselor ID {CounselorId}.", counselorId);
                    return new GeneralResult<PaginatedResult<ConsultationTimeDetailsDto>>(false, _localizationManager.GetLocalizedString("NoConsultationTimesFound"), null);
                }

                return new GeneralResult<PaginatedResult<ConsultationTimeDetailsDto>>(true, _localizationManager.GetLocalizedString("DataFound"), pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving consultation times for counselor ID {CounselorId}.", counselorId);
                return new GeneralResult<PaginatedResult<ConsultationTimeDetailsDto>>(false, _localizationManager.GetLocalizedString("UnexpectedErrorWhileGettingData"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<CounselorRequestDetailsDto>> GetCounselorByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogError("UserId is required.");
                    return new GeneralResult<CounselorRequestDetailsDto>(
                        false, _localizationManager.GetLocalizedString("UserIdIsRequired"), null);
                }

                var counselor = await _dbContext.Counselors
                    .Include(c => c.User)
                    .Include(c => c.ConsultationTimes)
                    .Where(c => !c.IsDeleted && c.UserId == userId)
                    .FirstOrDefaultAsync();

                if (counselor == null)
                {
                    _logger.LogInformation($"No counselor found for userId: {userId}");
                    return new GeneralResult<CounselorRequestDetailsDto>(
                        false, _localizationManager.GetLocalizedString("CounselorNotFound"), null);
                }

                var dto = new CounselorRequestDetailsDto
                {
                    Id = counselor.Id,
                    FullName = $"{counselor.User.FirstName} {counselor.User.LastName}",
                    Email = counselor.User.Email,
                    PhoneNumber = counselor.User.PhoneNumber,
                    DateOfBirth = counselor.User.DOB,
                    NationalId = counselor.User.NationalId,
                    Specialization = counselor.User.Specialization,
                    CountryCode = counselor.User.CountryCode,

                    Qualification = counselor.Qualification,
                    City = counselor.City,
                    SpecializationExperience = counselor.SpecializationExperience,
                    ConsultingExperience = counselor.ConsultingExperience,
                    DailyHours = counselor.DailyHours,
                    SocialMediaAccounts = counselor.SocialMediaAccounts,
                    Status = counselor.Status,
                    CreatedAt = counselor.CreatedAt,
                    counselorTimeData = counselor.ConsultationTimes?
                        .Where(t => !t.IsDeleted)
                        .Select(t => new CounselorTimeDataDto
                        {
                            DateTimeSlot = t.DateTimeSlot,
                            IsBooked = t.IsBooked
                        }).ToList()
                };

                return new GeneralResult<CounselorRequestDetailsDto>(
                    true, _localizationManager.GetLocalizedString("CounselorFound"), dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetCounselorByUserId.");
                return new GeneralResult<CounselorRequestDetailsDto>(
                    false, _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingCounselor"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> HasPendingApplication(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogError("UserId is required.");
                    return new GeneralResult<bool>(
                        false, _localizationManager.GetLocalizedString("UserIdIsRequired"), false);
                }

                var hasPending = await _dbContext.Counselors
                    .AnyAsync(c => !c.IsDeleted &&
                                   c.UserId == userId &&
                                   c.Status == CounselorRequesttStatus.Pending);

                if (hasPending)
                {
                    _logger.LogInformation("User has a pending counselor application.");
                    return new GeneralResult<bool>(
                        true, _localizationManager.GetLocalizedString("PendingCounselorRequestExists"), true);
                }

                return new GeneralResult<bool>(
                    true, _localizationManager.GetLocalizedString("NoPendingCounselorRequest"), false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking for pending counselor application.");
                return new GeneralResult<bool>(
                    false, _localizationManager.GetLocalizedString("UnexpectedErrorCheckingPendingApplication"), false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<CounselorSummaryStatsDto>> GetCounselorSummaryStats()
        {
            try
            {
                var activeCount = await _dbContext.Counselors
                    .CountAsync(c => !c.IsDeleted && c.Active);

                var pendingCount = await _dbContext.Counselors
                    .CountAsync(c => !c.IsDeleted && c.Status == CounselorRequesttStatus.Pending);

                var availableHours = await _dbContext.ConsultationTimes
                    .Where(ct => !ct.IsDeleted && !ct.IsBooked)
                    .CountAsync(); // يمكن اعتبار كل وقت ساعة واحدة

                var stats = new CounselorSummaryStatsDto
                {
                    ActiveCounselors = activeCount,
                    PendingRequests = pendingCount,
                    AvailableHours = availableHours
                };

                return new GeneralResult<CounselorSummaryStatsDto>(
                    true,
                    _localizationManager.GetLocalizedString("CounselorSummaryRetrieved"),
                    stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving counselor summary stats.");
                return new GeneralResult<CounselorSummaryStatsDto>(
                    false,
                    _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingSummaryStats"),
                    null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<ConsultationAllData>>> GetConsultationsByCounselorId(string counselorUserId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(counselorUserId))
                {
                    _logger.LogError("CounselorId is required.");
                    return new GeneralResult<PaginatedResult<ConsultationAllData>>(false, _localizationManager.GetLocalizedString("CounselorIdIsRequired"), null);
                }

                var counselor = await _dbContext.Users
                    .AsNoTracking()
                    .Where(u => u.Id == counselorUserId && !u.IsDeleted)
                    .Select(u => u.Counselor)
                    .FirstOrDefaultAsync(cancellationToken);

                if (counselor == null)
                {
                    _logger.LogError("No user found with this id: {UserId}.", counselorUserId);
                    return new GeneralResult<PaginatedResult<ConsultationAllData>>(false, _localizationManager.GetLocalizedString("CounselorNotFound"), null);
                }

                var query = _dbContext.Consultations
                    .AsNoTracking()
                    .Include(c => c.Client)
                    .Include(c => c.ConsultationTime)
                    .Where(c => c.CounselorId == counselor.Id && !c.IsDeleted)
                    .Select(c => new ConsultationAllData
                    {
                        Id = c.Id,
                        Type = c.Type,
                        Status = c.Status,
                        Description = c.Description,
                        counselorData = null,
                        ConsultationTimeDate = c.ConsultationTime != null ? c.ConsultationTime.DateTimeSlot : DateTimeOffset.UtcNow,
                        customerData = new CustomerData
                        {
                            Id = c.Client.Id,
                            FirstName = c.Client.FirstName ?? "",
                            LastName = c.Client.LastName ?? "",
                            Email = c.Client.Email ?? ""
                        }
                    });

                var pagedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                if (!pagedResult.Items.Any())
                {
                    _logger.LogInformation("No consultations found for counselor ID {CounselorId}.", counselor.Id);
                    return new GeneralResult<PaginatedResult<ConsultationAllData>>(false, _localizationManager.GetLocalizedString("NoConsultationsFound"), null);
                }

                return new GeneralResult<PaginatedResult<ConsultationAllData>>(true, _localizationManager.GetLocalizedString("DataFound"), pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving consultations.");
                return new GeneralResult<PaginatedResult<ConsultationAllData>>(false, _localizationManager.GetLocalizedString("UnexpectedErrorWhileGettingData"), null);
            }
        }

        /// <inheritdoc />
        public async Task GenerateDailyRecurringTimeSlots()
        {
            var recurringSlots = await _dbContext.ConsultationTimes
                .Where(ct => ct.IsRecurringDaily && !ct.IsDeleted)
                .ToListAsync();

            foreach (var slot in recurringSlots)
            {
                var newDate = DateHelper.UtcNow.Date.AddHours(slot.DateTimeSlot.Hour);

                var exists = await _dbContext.ConsultationTimes.AnyAsync(ct =>
                    ct.CounselorId == slot.CounselorId &&
                    ct.DateTimeSlot.Date == newDate.Date &&
                    ct.DateTimeSlot.Hour == newDate.Hour);

                if (!exists)
                {
                    var todaySlot = new ConsultationTime
                    {
                        CounselorId = slot.CounselorId,
                        DateTimeSlot = newDate,
                        CreatedAt = DateHelper.UtcNow,
                        IsBooked = false,
                        IsDeleted = false,
                        IsRecurringDaily = true
                    };

                    await _dbContext.ConsultationTimes.AddAsync(todaySlot);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        private IQueryable<Counselor> GetAllActiveCounselorsQuery()
        {
            return _dbContext.Counselors
                .Include(c => c.User)
                .Include(c => c.ConsultationTimes)
                .Where(c => !c.IsDeleted);
        }

        private async Task<List<CounselorRequestDetailsDto>> GetCounselorsByStatusAsync(CounselorRequesttStatus? status = null)
        {
            var query = _dbContext.Counselors
                .Include(c => c.User)
                .Where(c => !c.IsDeleted);

            if (status.HasValue)
                query = query.Where(c => c.Status == status);

            return await query
                .Select(c => new CounselorRequestDetailsDto
                {
                    Id = c.Id,
                    FullName = c.User.FirstName + " " + c.User.LastName,
                    NationalId = c.User.NationalId,
                    Specialization = c.User.Specialization,
                    CountryCode = c.User.CountryCode,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    DateOfBirth = c.User.DOB,
                    Qualification = c.Qualification,
                    City = c.City,
                    SpecializationExperience = c.SpecializationExperience,
                    ConsultingExperience = c.ConsultingExperience,
                    DailyHours = c.DailyHours,
                    SocialMediaAccounts = c.SocialMediaAccounts,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    counselorTimeData = c.ConsultationTimes != null ? c.ConsultationTimes
                        .Where(t => !t.IsDeleted)
                        .Select(t => new CounselorTimeDataDto
                        {
                            DateTimeSlot = t.DateTimeSlot,
                            IsBooked = t.IsBooked
                        }).ToList()
                        : null,
                }).ToListAsync();
        }
    }
}

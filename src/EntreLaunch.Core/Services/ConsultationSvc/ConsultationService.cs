namespace EntreLaunch.Services.ConsultationSvc
{
    public class ConsultationService : IConsultation
    {
        private readonly ILogger<MyOpportunityService> _logger;
        private readonly IMapper _mapper;
        private readonly PgDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IRoleService _roleService;
        private readonly DefaultRolesConfig _defaultRoles;
        private readonly IEmailVerificationExtension _emailVerificationExtension;
        public ConsultationService(
            ILogger<MyOpportunityService> logger,
            IMapper mapper,
            PgDbContext pgDbContext,
            UserManager<User> userManager,
            IRoleService roleService,
            DefaultRolesConfig defaultRoles,
            IEmailVerificationExtension emailVerificationExtension)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = pgDbContext;
            _userManager = userManager;
            _roleService = roleService;
            _defaultRoles = defaultRoles;
            _emailVerificationExtension = emailVerificationExtension;
        }

        // Counselor methods

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> IsCounselor(int id)
        {
            try
            {
                if (id == 0)
                {
                    return new GeneralResult<bool>(false, "Id is required.", false);
                }

                var counselor = _dbContext.Counselors.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
                if (counselor == null)
                {
                    return new GeneralResult<bool>(false, "Counselor not found.", false);
                }

                var result = await _roleService.IsUserInRoleAsync(counselor.UserId, "Counselor");
                if (result.IsSuccess == false)
                {
                    return new GeneralResult<bool>(false, result.Message, false);
                }

                return new GeneralResult<bool>(true, "Counselor found.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult<bool>(false, "Unexpected error while checking if the user is a counselor.", false);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> SendCounselorRequest([FromBody] CreateCounselorRequestDto counselorRequest)
        {
            try
            {
                if (counselorRequest == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                var user = await _userManager.FindByIdAsync(counselorRequest.UserId);
                if (user == null)
                {
                    _logger.LogError($"No user found with this id: {counselorRequest.UserId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found.",
                        Data = null
                    };
                }

                if (user.Specialization == null || user.DOB == null || user.CountryCode == 0)
                {
                    _logger.LogError($"user profile {user!.Id} not completed.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Please complete your profile first.",
                        Data = null
                    };
                }

                var counselorRequestMap = _mapper.Map<Counselor>(counselorRequest);
                counselorRequestMap.CreatedAt = DateTimeOffset.UtcNow;
                counselorRequestMap.Status = CounselorRequesttStatus.Pending;
                counselorRequestMap.IsDeleted = false;
                counselorRequestMap.Active = false;
                await _dbContext.Counselors.AddAsync(counselorRequestMap);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "counselor request sent successfully.",
                    Data = new CounselorRequestDetailsDto
                    {
                        Id = counselorRequestMap.Id,
                        FullName = counselorRequestMap.User.FirstName + " " + counselorRequestMap.User.LastName,
                        Email = counselorRequestMap.User.Email,
                        NationalId = counselorRequestMap.User.NationalId,
                        PhoneNumber = counselorRequestMap.User.PhoneNumber,
                        DateOfBirth = counselorRequestMap.User.DOB,
                        CountryCode = counselorRequestMap.User.CountryCode,

                        Qualification = counselorRequestMap.Qualification,
                        City = counselorRequestMap.City,
                        SpecializationExperience = counselorRequestMap.SpecializationExperience,
                        ConsultingExperience = counselorRequestMap.ConsultingExperience,
                        DailyHours = counselorRequestMap.DailyHours,
                        SocialMediaAccounts = counselorRequestMap.SocialMediaAccounts,
                        Status = counselorRequestMap.Status,
                        CreatedAt = counselorRequestMap.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending counselor request.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while sending counselor request.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CounselorRequestDetailsDto>>> AllCounselorRequest()
        {
            try
            {
                var counselors = await _dbContext.Counselors
                .Include(c => c.User)
                .Where(c => !c.IsDeleted)
                .Select(c => new CounselorRequestDetailsDto
                {
                    // user date
                    FullName = c.User.FirstName + " " + c.User.LastName,
                    NationalId = c.User.NationalId,
                    Specialization = c.User.Specialization,
                    CountryCode = c.User.CountryCode,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    DateOfBirth = c.User.DOB,

                    // Counselor data
                    Id = c.Id,
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
                    _logger.LogInformation("No counselors found.");
                    return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "No counselors found.", null);
                }

                _logger.LogInformation("Counselors found.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(true, "Counselors found.", counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting counselors.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "Unexpected error while getting counselors.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CounselorRequestDetailsDto>>> PendingCounselorRequest()
        {
            try
            {
                var counselors = await _dbContext.Counselors
                .Include(c => c.User)
                .Where(c => c.Status == CounselorRequesttStatus.Pending && !c.IsDeleted)
                .Select(c => new CounselorRequestDetailsDto
                {
                    // user date
                    FullName = c.User.FirstName + " " + c.User.LastName,
                    NationalId = c.User.NationalId,
                    Specialization = c.User.Specialization,
                    CountryCode = c.User.CountryCode,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    DateOfBirth = c.User.DOB,

                    // Counselor data
                    Id = c.Id,
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
                    _logger.LogInformation("No pending counselors request found.");
                    return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "No pending counselors request found.", null);
                }

                _logger.LogInformation("Pending counselors request found.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(true, "Pending counselors request found.", counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting counselors.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "Unexpected error while getting counselors.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CounselorRequestDetailsDto>>> AcceptedCounselorRequest()
        {
            try
            {
                var counselors = await _dbContext.Counselors
                .Include(c => c.User)
                .Where(c => c.Status == CounselorRequesttStatus.Accepted && !c.IsDeleted)
                .Select(c => new CounselorRequestDetailsDto
                {
                    // user date
                    FullName = c.User.FirstName + " " + c.User.LastName,
                    NationalId = c.User.NationalId,
                    Specialization = c.User.Specialization,
                    CountryCode = c.User.CountryCode,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    DateOfBirth = c.User.DOB,

                    // Counselor data
                    Id = c.Id,
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
                    _logger.LogInformation("No accepted counselors request found.");
                    return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "No accepted counselors request found.", null);
                }

                return new GeneralResult<List<CounselorRequestDetailsDto>>(true, "Accepted counselors request found.", counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting counselors.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "Unexpected error while getting counselors.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CounselorRequestDetailsDto>>> RejectedCounselorRequest()
        {
            try
            {
                var counselors = await _dbContext.Counselors
                .Include(c => c.User)
                .Where(c => c.Status == CounselorRequesttStatus.Rejected && !c.IsDeleted)
                .Select(c => new CounselorRequestDetailsDto
                {
                    // user date
                    FullName = c.User.FirstName + " " + c.User.LastName,
                    NationalId = c.User.NationalId,
                    Specialization = c.User.Specialization,
                    CountryCode = c.User.CountryCode,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    DateOfBirth = c.User.DOB,

                    // Counselor data
                    Id = c.Id,
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
                    _logger.LogInformation("No rejected counselors request found.");
                    return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "No rejected counselors request found.", null);
                }

                return new GeneralResult<List<CounselorRequestDetailsDto>>(true, "Rejected counselors request found.", counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting counselors.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "Unexpected error while getting counselors.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ProgressCounselorRequest(ProcessCounselorRequestDto processCounselorRequest)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (processCounselorRequest == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                var counselorRequest = _dbContext.Counselors.FirstOrDefault(o => o.Id == processCounselorRequest.Id && !o.IsDeleted);

                if (counselorRequest == null)
                {
                    _logger.LogError($"No counselor request with Id {processCounselorRequest.Id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No data found.",
                        Data = null
                    };
                }

                if (counselorRequest.Status == processCounselorRequest.Status)
                {
                    _logger.LogError($"Counselor request with Id {processCounselorRequest.Id} is already {processCounselorRequest.Status}");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Counselor request {processCounselorRequest.Id} is already {processCounselorRequest.Status}",
                        Data = null
                    };
                }

                counselorRequest.Status = processCounselorRequest.Status;
                counselorRequest.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                if (processCounselorRequest.Status == CounselorRequesttStatus.Accepted)
                {
                    counselorRequest.Active = true;
                }

                if (processCounselorRequest.Status == CounselorRequesttStatus.Rejected)
                {
                    counselorRequest.Active = false;
                }

                _dbContext.Counselors.EntreLaunchdate(counselorRequest);
                _dbContext.SaveChanges();

                if (processCounselorRequest.Status == CounselorRequesttStatus.Accepted)
                {
                    var result = await _roleService.IsUserInRoleAsync(counselorRequest.UserId, "Counselor");
                    if(result.IsSuccess == false)
                    {
                        await _roleService.AssignRoleAsync(counselorRequest.UserId, _defaultRoles[2]);
                    }
                }

                await transaction.CommitAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = $"Counselor request {processCounselorRequest.Id} EntreLaunchdated to {processCounselorRequest.Status} successfully.",
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
                    Message = "Unexpected error while processing counselor request.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CounselorRequestDetailsDto>>> CounselorBySpecialization(string specialization)
        {
            try
            {
                var counselors = await _dbContext.Counselors
                .Include(c => c.User)
                .Where(c => c.User.Specialization!.Equals(specialization) && !c.IsDeleted && c.Active)
                .Select(c => new CounselorRequestDetailsDto
                {
                    // user date
                    FullName = c.User.FirstName + " " + c.User.LastName,
                    NationalId = c.User.NationalId,
                    Specialization = c.User.Specialization,
                    CountryCode = c.User.CountryCode,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    DateOfBirth = c.User.DOB,

                    // Counselor data
                    Id = c.Id,
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
                    _logger.LogInformation("No counselors for this specialization.");
                    return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "No counselors for this specialization.", null);
                }

                return new GeneralResult<List<CounselorRequestDetailsDto>>(true, "Retrieved counselors by specialization successfully.", counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving counselors by specialization.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "Unexpected error while retrieving counselors by specialization.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<CounselorRequestDetailsDto>>> CounselorCV(int id)
        {
            try
            {
                var counselors = await _dbContext.Counselors
                .Include(c => c.User)
                .Where(c => c.Id == id && !c.IsDeleted && c.Active)
                .Select(c => new CounselorRequestDetailsDto
                {
                    // user date
                    FullName = c.User.FirstName + " " + c.User.LastName,
                    NationalId = c.User.NationalId,
                    Specialization = c.User.Specialization,
                    CountryCode = c.User.CountryCode,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    DateOfBirth = c.User.DOB,

                    // Counselor data
                    Id = c.Id,
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
                    return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "No counselor found.", null);
                }

                _logger.LogInformation("Retrieved counselors successfully.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(true, "Retrieved counselors successfully.", counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving counselors.");
                return new GeneralResult<List<CounselorRequestDetailsDto>>(false, "Unexpected error while retrieving counselors.", null);
            }
        }

        // consultation methods

        /// <inheritdoc />
        public async Task<GeneralResult<ConsultationDetailsDto>> GetConsultationRequestById(int id)
        {
            try
            {
                var counselor = _mapper.Map<ConsultationDetailsDto>(
                    await _dbContext.Consultations.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync());
                if (counselor == null)
                {
                    _logger.LogInformation("No counselor found.");
                    return new GeneralResult<ConsultationDetailsDto>(false, "No counselor found.", null);
                }

                _logger.LogInformation("Retrieved counselor successfully.");
                return new GeneralResult<ConsultationDetailsDto>(true, "Retrieved counselor successfully.", counselor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving counselor.");
                return new GeneralResult<ConsultationDetailsDto>(false, "Unexpected error while retrieving counselor.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<ConsultationDetailsDto>>> GetConsultationForCounselor(int id)
        {
            try
            {
                var counselorConsultation = _mapper.Map<List<ConsultationDetailsDto>>(
                    await _dbContext.Consultations.Where(c => c.CounselorId == id && !c.IsDeleted).ToListAsync());

                if (counselorConsultation == null || counselorConsultation.Count == 0)
                {
                    _logger.LogInformation("No consultation for counselor found.");
                    return new GeneralResult<List<ConsultationDetailsDto>>(false, "No consultation for counselor found.", null);
                }

                _logger.LogInformation("Retrieved consultation for counselor successfully.");
                return new GeneralResult<List<ConsultationDetailsDto>>(true, "Retrieved consultation for counselor successfully.", counselorConsultation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving consultation for counselor.");
                return new GeneralResult<List<ConsultationDetailsDto>>(false, "Unexpected error while retrieving consultation for counselor.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<ConsultationDetailsDto>>> AllConsultationRequest()
        {
            try
            {
                var counselors = _mapper.Map<List<ConsultationDetailsDto>>(
                    await _dbContext.Consultations.Where(c => !c.IsDeleted).ToListAsync());

                if (!counselors.Any())
                {
                    _logger.LogInformation("No Consultations found.");
                    return new GeneralResult<List<ConsultationDetailsDto>>(false, "No Consultations found.", null);
                }

                _logger.LogInformation("Retrieved consultations successfully.");
                return new GeneralResult<List<ConsultationDetailsDto>>(true, "Retrieved consultations successfully.", counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving consultations.");
                return new GeneralResult<List<ConsultationDetailsDto>>(false, "Unexpected error while retrieving consultations.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<ConsultationDetailsDto>>> GetConsultationByType(ConsultationType type)
        {
            try
            {
                var counselors = _mapper.Map<List<ConsultationDetailsDto>>(
                    await _dbContext.Consultations.Where(c => !c.IsDeleted && c.Type == type).ToListAsync());

                if (!counselors.Any())
                {
                    _logger.LogInformation("No Consultations found.");
                    return new GeneralResult<List<ConsultationDetailsDto>>(false, "No Consultations found.", null);
                }

                _logger.LogInformation("Retrieved consultations successfully.");
                return new GeneralResult<List<ConsultationDetailsDto>>(true, "Retrieved consultations successfully.", counselors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving consultations.");
                return new GeneralResult<List<ConsultationDetailsDto>>(false, "Unexpected error while retrieving consultations.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> BookingConsultation(ConsultationCreateDto consultationCreateDto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                if (consultationCreateDto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                var counselor = await _dbContext.Counselors
                    .FirstOrDefaultAsync(c => c.Id == consultationCreateDto.CounselorId && !c.IsDeleted && c.Active);

                if (counselor == null)
                {
                    _logger.LogError($"This counselor {consultationCreateDto.CounselorId} does not exist or is not active.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "This counselor does not exist or is not active.",
                        Data = null
                    };
                }

                var client = _userManager.FindByIdAsync(consultationCreateDto.ClientId);
                if (client == null)
                {
                    _logger.LogError($"No user found with this id: {consultationCreateDto.ClientId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found.",
                        Data = null
                    };
                }

                var consultationTime = await _dbContext.ConsultationTimes
                    .Where(t => t.Id == consultationCreateDto.ConsultationTimeId && !t.IsDeleted && !t.IsBooked).FirstOrDefaultAsync();
                if (consultationTime == null)
                {
                    _logger.LogError($"No consultation time found with this id: {consultationCreateDto.ConsultationTimeId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Consultation time not found or deleted or booked.",
                        Data = null
                    };
                }

                if (consultationTime.IsBooked)
                {
                    _logger.LogError($"Can not booking this consultation time with this id: {consultationCreateDto.ConsultationTimeId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Consultation time not found or booked.",
                        Data = null
                    };
                }

                // check payment

                var consultationRequestMap = _mapper.Map<Consultation>(consultationCreateDto);
                consultationRequestMap.CreatedAt = DateTimeOffset.UtcNow;
                consultationRequestMap.IsDeleted = false;
                consultationRequestMap.Status = ConsultationStatus.Scheduled;
                _dbContext.Consultations.Add(consultationRequestMap);
                _dbContext.SaveChanges();

                // EntreLaunchdate consultation time status
                consultationTime.IsBooked = true;
                _dbContext.ConsultationTimes.EntreLaunchdate(consultationTime);
                _dbContext.SaveChanges();

                await transaction.CommitAsync();

                // send message
                await _emailVerificationExtension.SendEmailAsync(
                    client.Result!.Email!,
                    subject: "Consultation appointment",
                    body: $"Your consultation has been scheduled. Here are the details:\n" +
                          $"- Day: {consultationTime.DateTimeSlot!.Value.UtcDateTime.ToString("dddd")}\n" +
                          $"- Date: {consultationTime.DateTimeSlot.Value.UtcDateTime.ToString("yyyy-MM-dd")}\n" +
                          $"- Time: {consultationTime.DateTimeSlot.Value.UtcDateTime.ToString("hh:mm tt")})\n" +
                          $"- Counselor's name: {counselor!.User!.FirstName} {counselor.User.LastName})");

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Consultation request sent successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,  "Unexpected error while processing consultation request.");
                await transaction.RollbackAsync();
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while processing consultation request.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ProgressConsultationStatus(ProcessConsultationStatusDto processConsultationStatus)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (processConsultationStatus == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                var consultation = _dbContext.Consultations.FirstOrDefault(c => c.Id == processConsultationStatus.Id && !c.IsDeleted);

                if (consultation == null)
                {
                    _logger.LogError($"No consultation with Id {processConsultationStatus.Id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No consultation found.",
                        Data = null
                    };
                }

                if (consultation.Status != ConsultationStatus.Scheduled)
                {
                    _logger.LogError($"The consultation {processConsultationStatus.Id} is not in scheduling mode and its status cannot be modified");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "The consultation is not in scheduling mode and its status cannot be modified",
                        Data = null
                    };
                }

                if (consultation.Status == processConsultationStatus.Status)
                {
                    _logger.LogError($"Counselor request with Id {processConsultationStatus.Id} is already in {processConsultationStatus.Status} status.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"consultation {processConsultationStatus.Id} is already in {processConsultationStatus.Status} status.",
                        Data = null
                    };
                }

                consultation.Status = processConsultationStatus.Status;
                consultation.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.Consultations.EntreLaunchdate(consultation);
                _dbContext.SaveChanges();

                await transaction.CommitAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = $"Counselor request {processConsultationStatus.Id} EntreLaunchdated to {processConsultationStatus.Status} successfully.",
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
                    Message = "Unexpected error while processing counselor request.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> SendTextConsultation(ConsultationCreateDto consultationCreateDto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                if (consultationCreateDto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                var counselor = await _dbContext.Counselors
                    .FirstOrDefaultAsync(c => c.Id == consultationCreateDto.CounselorId && !c.IsDeleted && c.Active);

                if (counselor == null)
                {
                    _logger.LogError($"This counselor {consultationCreateDto.CounselorId} does not exist or is not active.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "This counselor does not exist or is not active.",
                        Data = null
                    };
                }

                var client = _userManager.FindByIdAsync(consultationCreateDto.ClientId);
                if (client == null)
                {
                    _logger.LogError($"No user found with this id: {consultationCreateDto.ClientId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found.",
                        Data = null
                    };
                }

                if (consultationCreateDto.Type != ConsultationType.text)
                {
                    _logger.LogError($"Can not booking this consultation type with this id: {consultationCreateDto.Type}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Consultation type must be text.",
                        Data = null
                    };
                }


                // check payment

                var textConsultationMap = _mapper.Map<Consultation>(consultationCreateDto);
                textConsultationMap.CreatedAt = DateTimeOffset.UtcNow;
                textConsultationMap.IsDeleted = false;
                textConsultationMap.Status = ConsultationStatus.Scheduled;
                _dbContext.Consultations.Add(textConsultationMap);
                _dbContext.SaveChanges();

                await transaction.CommitAsync();

                // send message
                await _emailVerificationExtension.SendEmailAsync(
                    counselor.User.Email!,
                    subject: "Consultation Request",
                    body: $"You have a new counseling request. Here are the details:\n" +
                          $"- Client name: {counselor!.User!.FirstName} {counselor.User.LastName}\n" +
                          $"- Client Email: {counselor!.User!.Email}\n" +
                          $"\n" +
                          $"- Consultation Text: {textConsultationMap.Description} \n" +
                          $"\n" +
                          $"- Consultation Date: {textConsultationMap.CreatedAt.Value.UtcDateTime.ToString("yyyy-MM-dd")}\n" +
                          $"- Consultation Time: {textConsultationMap.CreatedAt.Value.UtcDateTime.ToString("hh:mm tt")}\n");

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Consultation text sent successfully.",
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
                    Message = "Failed to process consultation request.",
                    Data = null
                };
            }
        }

        // ticket methods

        /// <inheritdoc />
        public async Task<GeneralResult> OpenTicket(TicketCreateDto ticketCreateDto)
        {
            try
            {
                if (ticketCreateDto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }


                if ((await IsCounselor(ticketCreateDto.CreatorId)).IsSuccess == false)
                {
                    _logger.LogError($"Counselor with this id {ticketCreateDto.CreatorId} do not have permission to open a ticket.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "you do not have permission to open a ticket.",
                        Data = null
                    };
                }

                var consultationType = await _dbContext.Consultations
                    .AnyAsync(c => c.Id == ticketCreateDto.ConsultationId && !c.IsDeleted && c.Type == ConsultationType.text);
                if (consultationType)
                {
                    _logger.LogError($"Can not open a tecket for this consultations {ticketCreateDto.ConsultationId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Can not open a tecket for this consultations.",
                        Data = null
                    };
                }

                var creator = await _dbContext.Consultations
                    .AnyAsync(c => c.CounselorId == ticketCreateDto.CreatorId && !c.IsDeleted);
                if (!creator)
                {
                    _logger.LogError($"Creator of ticket {ticketCreateDto.CreatorId} does not have permission to open a ticket.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Creator of ticket does not have permission to open a ticket, only consultaion creator can open a ticket.",
                        Data = null
                    };
                }

                var consultationTicket = await _dbContext.ConsultationTickets
                    .AnyAsync(x => x.ConsultationId == ticketCreateDto.ConsultationId && !x.IsDeleted);
                if (consultationTicket)
                {
                    _logger.LogError($"Consultation with this id {ticketCreateDto.ConsultationId} already opened a ticket.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Consultation already has opened a ticket.",
                        Data = null
                    };
                }

                var ticketOpentMap = _mapper.Map<ConsultationTicket>(ticketCreateDto);
                ticketOpentMap.CreatedAt = DateTimeOffset.UtcNow;
                ticketOpentMap.Status = ConsultationTicketStatus.Open;
                ticketOpentMap.IsDeleted = false;
                await _dbContext.ConsultationTickets.AddAsync(ticketOpentMap);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Ticket {ticketOpentMap.Id} opened successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Ticket with Number {ticketOpentMap.Id} opened successfully.",
                    Data = _mapper.Map<TicketDetailsDto>(ticketOpentMap)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while opening new ticket.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while opening new ticket.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ProgressTicket([FromBody] ProcessTicketDto processTicketDto)
        {
            try
            {
                if (processTicketDto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                var ticket = _dbContext.ConsultationTickets.FirstOrDefault(o => o.Id == processTicketDto.Id && !o.IsDeleted);

                if (ticket == null)
                {
                    _logger.LogError($"No ticket found with Id {processTicketDto.Id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No data found.",
                        Data = null
                    };
                }

                if (ticket.Status == processTicketDto.Status)
                {
                    _logger.LogError($"ticket with Id {processTicketDto.Id} is already {processTicketDto.Status}");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"ticket {processTicketDto.Id} is already {processTicketDto.Status}",
                        Data = null
                    };
                }

                ticket.Status = processTicketDto.Status;
                if(processTicketDto.Status == ConsultationTicketStatus.Closed)
                {
                    ticket.EndDate = DateTimeOffset.UtcNow;
                }

                ticket.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.ConsultationTickets.EntreLaunchdate(ticket);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = $"ticket {processTicketDto.Id} EntreLaunchdated to {processTicketDto.Status} successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while processing ticket.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while processing ticket.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<TicketDetailsDto>>> AllConsultationTickets()
        {
            try
            {
                var tickets = _mapper.Map<List<TicketDetailsDto>>(
                    await _dbContext.ConsultationTickets.Where(c => !c.IsDeleted).ToListAsync());

                if (!tickets.Any())
                {
                    _logger.LogInformation("No tickets found.");
                    return new GeneralResult<List<TicketDetailsDto>>(false, "No tickets found.", null);
                }

                _logger.LogInformation("Retrieved tickets successfully.");
                return new GeneralResult<List<TicketDetailsDto>>(true, "Retrieved tickets successfully.", tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving tickets.");
                return new GeneralResult<List<TicketDetailsDto>>(false, "Unexpected error while retrieving tickets.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<TicketDetailsDto>> GetTicketById(int id)
        {
            try
            {
                var ticket = _mapper.Map<TicketDetailsDto>(await _dbContext.ConsultationTickets.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync());

                if (ticket == null)
                {
                    _logger.LogInformation("No ticket found.");
                    return new GeneralResult<TicketDetailsDto>(false, "No ticket found.", null);
                }

                _logger.LogInformation("Retrieved ticket successfully.");
                return new GeneralResult<TicketDetailsDto>(true, "Retrieved ticket successfully.", ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving ticket.");
                return new GeneralResult<TicketDetailsDto>(false, "Unexpected error while retrieving ticket.", null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<TicketDetailsDto>> GetConsultationTicketById(int id)
        {
            try
            {
                var consultationTicket = _mapper.Map<TicketDetailsDto>(await _dbContext.ConsultationTickets
                    .Where(c => c.ConsultationId == id && !c.IsDeleted).FirstOrDefaultAsync());

                if (consultationTicket == null)
                {
                    _logger.LogInformation("No ticket for this consultation.");
                    return new GeneralResult<TicketDetailsDto>(false, "No ticket for this consultation.", null);
                }

                _logger.LogInformation("Retrieved ticket successfully.");
                return new GeneralResult<TicketDetailsDto>(true, "Retrieved ticket successfully.", consultationTicket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving ticket.");
                return new GeneralResult<TicketDetailsDto>(false, "Unexpected error while retrieving ticket.", null);
            }
        }

        // Ticket Message

        /// <inheritdoc />
        public async Task<GeneralResult> SendTicketMessage(TicketMessageCreateDto ticketMessageCreate)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (ticketMessageCreate == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                // get ticket data
                var ticket = await _dbContext.ConsultationTickets
                    .Include(t => t.Consultation)
                    .ThenInclude(c => c.Counselor) // get counselor data
                    .FirstOrDefaultAsync(t => t.Id == ticketMessageCreate.TicketId && !t.IsDeleted);

                if (ticket == null)
                {
                    _logger.LogError($"No ticket found with id {ticketMessageCreate.TicketId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Ticket not found.",
                        Data = null
                    };
                }

                // get user data
                var clientUserId = ticket.Consultation.ClientId;
                var counselorUserId = ticket.Consultation.Counselor.UserId;

                if (ticketMessageCreate.SenderId != clientUserId &&
                    ticketMessageCreate.SenderId != counselorUserId)
                {
                    _logger.LogError("Sender is not authorized for this ticket.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "You are not allowed to send messages to this ticket.",
                        Data = null
                    };
                }

                var messageSender = await _dbContext.Users.Where(u => u.Id == ticketMessageCreate.SenderId && !u.IsDeleted).FirstOrDefaultAsync();
                if (messageSender == null)
                {
                    _logger.LogError($"No user found with this id: {ticketMessageCreate.SenderId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found.",
                        Data = null
                    };
                }

                var messageMap = _mapper.Map<ConsultationTicketMessage>(ticketMessageCreate);
                messageMap.CreatedAt = DateTimeOffset.UtcNow;
                messageMap.SendTime = DateTimeOffset.UtcNow;
                messageMap.IsDeleted = false;


                if (ticketMessageCreate.SenderId == clientUserId)
                {
                    messageMap.IsClientMessage = true;
                }

                await _dbContext.ConsultationTicketMessages.AddAsync(messageMap);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogError($"Ticket message with id {messageMap.Id} sent successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Ticket message sent successfully.",
                    Data = _mapper.Map<TicketMessageDetailsDto>(messageMap)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending ticket message.");
                await transaction.RollbackAsync();
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while sending ticket message.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> EditTicketMessage(int id, TicketMessageEntreLaunchdateDto ticketMessageEntreLaunchdate)
        {
            try
            {
                if (ticketMessageEntreLaunchdate == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                var message = await _dbContext.ConsultationTicketMessages
                    .Where(u => u.Id == id && !u.IsDeleted).FirstOrDefaultAsync();
                if (message == null)
                {
                    _logger.LogError($"No message found with this id: {id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Message not found.",
                        Data = null
                    };
                }

                _mapper.Map(ticketMessageEntreLaunchdate, message);
                message.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.ConsultationTicketMessages.EntreLaunchdate(message);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Ticket message with id {message.Id} EntreLaunchdated successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Ticket message edited successfully.",
                    Data = _mapper.Map<TicketMessageDetailsDto>(message)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while editing ticket message.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while editing ticket message.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteTicketMessage(int id)
        {
            try
            {
                var message = _dbContext.ConsultationTicketMessages.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
                if (message == null)
                {
                    _logger.LogError($"No message found with this id: {id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Message not found.",
                        Data = null
                    };
                }

                message.IsDeleted = true;
                message.DeletedAt = DateTimeOffset.UtcNow;
                _dbContext.ConsultationTicketMessages.EntreLaunchdate(message);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Ticket message with id {message.Id} deleted successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Ticket message deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting ticket message.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while deleting ticket message.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<TicketMessageDetailsDto>>> ShowTicketMessages(int id)
        {
            try
            {
                var messages = _mapper.Map<List<TicketMessageDetailsDto>>(await _dbContext.ConsultationTicketMessages
                    .Where(cm => cm.TicketId == id && !cm.IsDeleted).ToListAsync());
                if (!messages.Any())
                {
                    _logger.LogError($"No message found with this id: {id}.");
                    return new GeneralResult<List<TicketMessageDetailsDto>>(false, "No messages found.", null);
                }

                _logger.LogInformation("Messages found.");
                return new GeneralResult<List<TicketMessageDetailsDto>>(true, "Messages found.", messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting messages.");
                return new GeneralResult<List<TicketMessageDetailsDto>>(false, "Unexpected error while getting messages.", null);
            }
        }

        // consultation time methods.

        /// <inheritdoc />
        public async Task<GeneralResult> CreateCounselorTime(ConsultationTimeCreateDto consultationTimeCreateDto)
        {
            try
            {
                if (consultationTimeCreateDto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                var counselor = await _dbContext.Counselors
                    .FirstOrDefaultAsync(c => c.Id == consultationTimeCreateDto.CounselorId && !c.IsDeleted && c.Active);

                if (counselor == null)
                {
                    _logger.LogError($"This counselor {consultationTimeCreateDto.CounselorId} does not exist or is not active.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "This counselor does not exist or is not active.",
                        Data = null
                    };
                }

                var isTimeExists = await _dbContext.ConsultationTimes.AnyAsync(ct =>
                ct.CounselorId == consultationTimeCreateDto.CounselorId &&
                ct.DateTimeSlot.HasValue &&
                ct.DateTimeSlot.Value.UtcDateTime.Date == consultationTimeCreateDto.DateTimeSlot!.Value.UtcDateTime.Date &&
                 ct.DateTimeSlot.Value.UtcDateTime.Hour == consultationTimeCreateDto.DateTimeSlot.Value.UtcDateTime.Hour);

                if (isTimeExists)
                {
                    _logger.LogError("This time already exists.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "This time already exists.",
                        Data = null
                    };
                }

                var consultationTimeMap = _mapper.Map<ConsultationTime>(consultationTimeCreateDto);
                consultationTimeMap.CreatedAt = DateTimeOffset.UtcNow;
                consultationTimeMap.IsBooked = false;
                consultationTimeMap.IsDeleted = false;
                await _dbContext.ConsultationTimes.AddAsync(consultationTimeMap);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Consultation time with id {consultationTimeMap.Id} added successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Consultation time added successfully.",
                    Data = _mapper.Map<ConsultationTimeDetailsDto>(consultationTimeMap)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating consultation time.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while creating consultation time.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> EditCounselorTimes(int id, ConsultationTimeEntreLaunchdateDto consultationTimeEntreLaunchdateDto)
        {
            try
            {
                if (consultationTimeEntreLaunchdateDto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
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
                        Message = "No data found or this time is already booked.",
                        Data = null
                    };
                }

                var consultationTimeMap = _mapper.Map<ConsultationTime>(consultationTimeEntreLaunchdateDto);
                consultationTimeMap.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.ConsultationTimes.EntreLaunchdate(consultationTimeMap);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Consultation time with id {consultationTimeMap.Id} edited successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Consultation time edited successfully.",
                    Data = _mapper.Map<ConsultationTimeDetailsDto>(consultationTimeMap)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while editing consultation time.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while editing consultation time.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<ConsultationTimeDetailsDto>>> GetAllCounselorTimes(int id)
        {
            try
            {
                var consultationTimes = _mapper.Map<List<ConsultationTimeDetailsDto>>(await _dbContext.ConsultationTimes
                    .Where(ct => ct.CounselorId == id && !ct.IsDeleted).ToListAsync());
                if (!consultationTimes.Any())
                {
                    _logger.LogError($"No consultation time found with this id: {id}.");
                    return new GeneralResult<List<ConsultationTimeDetailsDto>>(false, "No data found.", null);
                }

                return new GeneralResult<List<ConsultationTimeDetailsDto>>(true, "Data found.", consultationTimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting data.");
                return new GeneralResult<List<ConsultationTimeDetailsDto>>(false, "Unexpected error while getting data.", null);
            }
        }

        // Ticket attachment 

        /// <inheritdoc />
        public async Task<GeneralResult> SendTicketAttachment(TicketAttachmentCreateDto ticketAttachmentCreateDto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (ticketAttachmentCreateDto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                // get ticket data
                var ticket = await _dbContext.ConsultationTickets
                    .Include(t => t.Consultation)
                    .ThenInclude(c => c.Counselor) // get counselor data
                    .FirstOrDefaultAsync(t => t.Id == ticketAttachmentCreateDto.TicketId && !t.IsDeleted);

                if (ticket == null)
                {
                    _logger.LogError($"No ticket found with id {ticketAttachmentCreateDto.TicketId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Ticket not found.",
                        Data = null
                    };
                }

                // get user data
                var clientUserId = ticket.Consultation.ClientId;
                var counselorUserId = ticket.Consultation.Counselor.UserId;

                if (ticketAttachmentCreateDto.SenderId != clientUserId &&
                    ticketAttachmentCreateDto.SenderId != counselorUserId)
                {
                    _logger.LogError("Sender is not authorized for this ticket.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "You are not allowed to send attachment to this ticket.",
                        Data = null
                    };
                }


                var attachmentSender = _dbContext.Users.FirstOrDefault(u => u.Id == ticketAttachmentCreateDto.SenderId && !u.IsDeleted);
                if (attachmentSender == null)
                {
                    _logger.LogError($"No user found with this id: {ticketAttachmentCreateDto.SenderId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found.",
                        Data = null
                    };
                }

                var attachmentMap = _mapper.Map<ConsultationTicketAttachment>(ticketAttachmentCreateDto);
                attachmentMap.CreatedAt = DateTimeOffset.UtcNow;
                attachmentMap.SendTime = DateTimeOffset.UtcNow;
                attachmentMap.IsDeleted = false;


                if (ticketAttachmentCreateDto.SenderId == clientUserId)
                {
                    attachmentMap.IsClientMessage = true;
                }

                await _dbContext.ConsultationTicketAttachments.AddAsync(attachmentMap);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogError($"Ticket attachment with id {attachmentMap.Id} sent successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Ticket attachment sent successfully.",
                    Data = _mapper.Map<TicketAttachmentDetailsDto>(attachmentMap)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending ticket attachment.");
                await transaction.RollbackAsync();
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while sending ticket attachment.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteTicketAttachment(int id, string userId)
        {
            try
            {
                var user = await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (user == false)
                {
                    _logger.LogError($"No user found with this id: {userId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found.",
                        Data = null
                    };
                }

                var attachment = await _dbContext.ConsultationTicketAttachments
                    .Where(u => u.Id == id && !u.IsDeleted && u.SenderId == userId).FirstOrDefaultAsync();
                if (attachment == null)
                {
                    _logger.LogError($"No attachment found with this id: {id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "attachment not found.",
                        Data = null
                    };
                }

                attachment.IsDeleted = true;
                attachment.DeletedAt = DateTimeOffset.UtcNow;
                _dbContext.ConsultationTicketAttachments.EntreLaunchdate(attachment);
                await _dbContext.SaveChangesAsync();

                _logger.LogError($"Ticket attachment with id {attachment.Id} deleted successfully.");
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Ticket attachment deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting ticket attachment.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Unexpected error while deleting ticket attachment.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<TicketAttachmentDetailsDto>>> ShowTicketAttachment(int id)
        {
            try
            {
                var attachment = _mapper.Map<List<TicketAttachmentDetailsDto>>(await _dbContext.ConsultationTicketAttachments
                    .Where(cm => cm.TicketId == id && !cm.IsDeleted).ToListAsync());
                if (!attachment.Any())
                {
                    _logger.LogError($"No attachment found with this id: {id}.");
                    return new GeneralResult<List<TicketAttachmentDetailsDto>>(false, "No attachments found.", null);
                }

                _logger.LogInformation("Attachments found.");
                return new GeneralResult<List<TicketAttachmentDetailsDto>>(true, "Attachments found.", attachment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,  "Unexpected error while getting attachments.");
                return new GeneralResult<List<TicketAttachmentDetailsDto>>(false, "Unexpected error while getting attachments.", null);
            }
        }
    }
}

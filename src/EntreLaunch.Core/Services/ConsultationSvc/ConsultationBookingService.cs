using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ConsultationDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.ConsultationSvc
{
    public class ConsultationBookingService(
        ILogger<ConsultationBookingService> logger,
        PgDbContext pgDbContext,
        IMapper mapper,
        UserManager<User> userManager,
        IEmailVerificationExtension emailVerificationExtension,
        ILocalizationManager localizationManager) : IConsultationBookingService
    {
        private readonly ILogger<ConsultationBookingService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly PgDbContext _dbContext = pgDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILocalizationManager _localizationManager = localizationManager;
        private readonly IEmailVerificationExtension _emailVerificationExtension = emailVerificationExtension;

        /// <inheritdoc />
        public async Task<GeneralResult> BookConsultation(OnlineConsultationCreateDto dto)
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

                var purchase = await _dbContext.Purchases
                    .Where(p =>
                        p.UserId == dto.ClientId &&
                        p.ItemType == PurchaseItemType.OnlineConsultation &&
                        !p.IsDeleted && !p.IsRefunded &&
                        !_dbContext.Consultations.Any(c => c.PurchaseId == p.Id))
                    .OrderBy(p => p.CreatedAt)
                    .FirstOrDefaultAsync();

                if (purchase == null)
                {
                    _logger.LogError("No valid consultation purchase found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("YouHaveToBuyOnlineConsultation"),
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
                        Message = _localizationManager.GetLocalizedString("InactiveOrMissingCounselor"),
                        Data = null
                    };
                }

                var client = _userManager.FindByIdAsync(dto.ClientId);
                if (client == null)
                {
                    _logger.LogError($"No user found with this id: {dto.ClientId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                var consultationTime = await _dbContext.ConsultationTimes
                    .Where(t => t.Id == dto.ConsultationTimeId && !t.IsDeleted && !t.IsBooked).FirstOrDefaultAsync();
                if (consultationTime == null)
                {
                    _logger.LogError($"No consultation time found with this id: {dto.ConsultationTimeId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ConsultationTimeNotFoundOrBooked"),
                        Data = null
                    };
                }

                if (consultationTime.IsBooked)
                {
                    _logger.LogError($"Can not booking this consultation time with this id: {dto.ConsultationTimeId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ConsultationTimeAlreadyBooked"),
                        Data = null
                    };
                }

                var hasBuyed = await _dbContext.Purchases.AsNoTracking().Where(p => p.UserId == dto.ClientId &&
                p.ItemType == PurchaseItemType.OnlineConsultation && !p.IsDeleted && !p.IsRefunded).AnyAsync();

                if(!hasBuyed)
                {
                    _logger.LogError($"Can not booking this consultation time with this id: {dto.ConsultationTimeId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("YouHaveToBuyOnlineConsultation"),
                        Data = null
                    };
                }

                var consultation = _mapper.Map<Consultation>(dto);
                consultation.CreatedAt = DateHelper.UtcNow;
                consultation.IsDeleted = false;
                consultation.Status = ConsultationStatus.Scheduled;
                consultation.PurchaseId = purchase.Id;
                _dbContext.Consultations.Add(consultation);
                _dbContext.SaveChanges();

                // update consultation time status
                consultationTime.IsBooked = true;
                _dbContext.ConsultationTimes.Update(consultationTime);
                _dbContext.SaveChanges();

                await transaction.CommitAsync();

                // send message
                await _emailVerificationExtension.SendEmailAsync(
                    client.Result!.Email!,
                    subject: "Consultation appointment",
                    body: $"Your consultation has been scheduled. Here are the details:\n" +
                          $"- Day: {consultationTime.DateTimeSlot.UtcDateTime.ToString("dddd")}\n" +
                          $"- Date: {consultationTime.DateTimeSlot.UtcDateTime.ToString("yyyy-MM-dd")}\n" +
                          $"- Time: {consultationTime.DateTimeSlot.UtcDateTime.ToString("hh:mm tt")})\n" +
                          $"- Counselor's name: {counselor!.User!.FirstName} {counselor.User.LastName})");

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("ConsultationRequestSent"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while processing consultation request.");
                await transaction.RollbackAsync();
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorProcessingConsultation"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> SubmitTextConsultation(TextConsultationCreateDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var counselor = await _dbContext.Counselors
                    .FirstOrDefaultAsync(c => c.Id == dto.CounselorId && !c.IsDeleted && c.Active);

                if (counselor == null)
                {
                    _logger.LogError($"This counselor {dto.CounselorId} does not exist or is not active.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("InactiveOrMissingCounselor"),
                        Data = null
                    };
                }

                var client = _userManager.FindByIdAsync(dto.ClientId);
                if (client == null)
                {
                    _logger.LogError($"No user found with this id: {dto.ClientId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                if (dto.Type != ConsultationType.text)
                {
                    _logger.LogError($"Can not booking this consultation type with this id: {dto.Type}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ConsultationTypeMustBeText"),
                        Data = null
                    };
                }

                var purchase = await _dbContext.Purchases
                    .Where(p =>
                        p.UserId == dto.ClientId &&
                        p.ItemType == PurchaseItemType.TextConsultation &&
                        !p.IsDeleted && !p.IsRefunded &&
                        !_dbContext.Consultations.Any(c => c.PurchaseId == p.Id))
                    .OrderBy(p => p.CreatedAt)
                    .FirstOrDefaultAsync();

                if (purchase == null)
                {
                    _logger.LogError("No valid text consultation purchase found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("YouHaveToBuyTextConsultation"),
                        Data = null
                    };
                }

                var consultation = _mapper.Map<Consultation>(dto);
                consultation.CreatedAt = DateHelper.UtcNow;
                consultation.IsDeleted = false;
                consultation.Status = ConsultationStatus.Scheduled;
                consultation.PurchaseId = purchase.Id;
                _dbContext.Consultations.Add(consultation);
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
                          $"- Consultation Text: {consultation.Description} \n" +
                          $"\n" +
                          $"- Consultation Date: {consultation.CreatedAt.Value.UtcDateTime.ToString("yyyy-MM-dd")}\n" +
                          $"- Consultation Time: {consultation.CreatedAt.Value.UtcDateTime.ToString("hh:mm tt")}\n");

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("TextConsultationSent"),
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
                    Message = _localizationManager.GetLocalizedString("FailedToProcessConsultation"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateConsultationStatus(ProcessConsultationStatusDto dto)
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

                var consultation = _dbContext.Consultations.FirstOrDefault(c => c.Id == dto.Id && !c.IsDeleted);

                if (consultation == null)
                {
                    _logger.LogError($"No consultation with Id {dto.Id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ConsultationNotFound"),
                        Data = null
                    };
                }

                if (consultation.Status == dto.Status)
                {
                    _logger.LogError($"Counselor request with Id {dto.Id} is already in {dto.Status} status.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("ConsultationAlreadyInStatus"),
                        Data = null
                    };
                }

                consultation.Status = dto.Status;
                consultation.UpdatedAt = DateHelper.UtcNow;
                _dbContext.Consultations.Update(consultation);
                _dbContext.SaveChanges();

                await transaction.CommitAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("ConsultationUpdatedSuccessfully"),
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
                    Message = _localizationManager.GetLocalizedString("UnexpectedErrorProcessingConsultation"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<ConsultationAllData>>> GetAllConsultations(
            PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var allConsultations = await GetConsultationFullDataAsync();

                //  التصفية والتقسيم محليًا داخل الذاكرة
                var totalCount = allConsultations.Count;

                var items = allConsultations
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();

                if (!items.Any())
                {
                    _logger.LogInformation("No consultations found.");
                    return new GeneralResult<PaginatedResult<ConsultationAllData>>(
                        false, _localizationManager.GetLocalizedString("NoConsultationsFound"), null);
                }

                var paginated = new PaginatedResult<ConsultationAllData>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = pagination.Page,
                    PageSize = pagination.PageSize
                };

                _logger.LogInformation("Retrieved consultations successfully.");
                return new GeneralResult<PaginatedResult<ConsultationAllData>>(
                    true, _localizationManager.GetLocalizedString("ConsultationsRetrievedSuccessfully"), paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving consultations.");
                return new GeneralResult<PaginatedResult<ConsultationAllData>>(
                    false, _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingCounselors"), null);
            }
        }


        /// <inheritdoc />
        public async Task<GeneralResult<ConsultationAllData>> GetConsultationById(int id)
        {
            try
            {
                var consultations = await GetConsultationFullDataAsync();
                var consultation = consultations.FirstOrDefault(c => c.Id == id);

                if (consultation == null)
                {
                    _logger.LogInformation("No consultation found with ID {Id}.", id);
                    return new GeneralResult<ConsultationAllData>(false, _localizationManager.GetLocalizedString("NoCounselorFound"), null);
                }

                _logger.LogInformation("Retrieved counselor successfully.");
                return new GeneralResult<ConsultationAllData>(true, _localizationManager.GetLocalizedString("CounselorRetrievedSuccessfully"), consultation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving counselor.");
                return new GeneralResult<ConsultationAllData>(false, _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingCounselors"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<ConsultationAllData>>> GetConsultationsByType(
            ConsultationType type,
            PaginationParams pagination,
            CancellationToken cancellationToken)
        {
            try
            {
                var allConsultations = await GetConsultationFullDataAsync();

                // فلترة داخل الذاكرة
                var filtered = allConsultations.Where(c => c.Type == type).ToList();

                var totalCount = filtered.Count;
                var items = filtered
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();

                if (!items.Any())
                {
                    _logger.LogInformation("No Consultations found for type: {Type}", type);
                    return new GeneralResult<PaginatedResult<ConsultationAllData>>(
                        false,
                        _localizationManager.GetLocalizedString("NoConsultationsFound"),
                        null
                    );
                }

                var paginated = new PaginatedResult<ConsultationAllData>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = pagination.Page,
                    PageSize = pagination.PageSize
                };

                _logger.LogInformation("Retrieved consultations successfully for type: {Type}", type);
                return new GeneralResult<PaginatedResult<ConsultationAllData>>(
                    true,
                    _localizationManager.GetLocalizedString("ConsultationsRetrievedSuccessfully"),
                    paginated
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving consultations.");
                return new GeneralResult<PaginatedResult<ConsultationAllData>>(
                    false,
                    _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingCounselors"),
                    null
                );
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<ConsultationAllData>>> GetConsultationsByCounselorId(int counselorId)
        {
            try
            {
                var allConsultations = await GetConsultationFullDataAsync();
                var counselorConsultations = allConsultations
                    .Where(c => c.counselorData != null && c.counselorData.Id == counselorId)
                    .ToList();

                if (!counselorConsultations.Any())
                {
                    _logger.LogInformation("No consultation for counselor found.");
                    return new GeneralResult<List<ConsultationAllData>>(false, _localizationManager.GetLocalizedString("NoConsultationForCounselor"), null);
                }

                _logger.LogInformation("Retrieved consultation for counselor successfully.");
                return new GeneralResult<List<ConsultationAllData>>(true, _localizationManager.GetLocalizedString("ConsultationForCounselorRetrieved"), counselorConsultations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving consultation for counselor.");
                return new GeneralResult<List<ConsultationAllData>>(false, _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingCounselors"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<ConsultationAllData>>> GetClientHistory(string clientId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clientId))
                {
                    _logger.LogError("ClientId is required.");
                    return new GeneralResult<List<ConsultationAllData>>(
                        false, _localizationManager.GetLocalizedString("UserIdIsRequired"), null);
                }

                var allConsultations = await GetConsultationFullDataAsync();

                var clientConsultations = allConsultations
                    .Where(c => c.customerData != null && c.customerData.Id == clientId) // exclude future ones if needed
                    .OrderByDescending(c => c.ConsultationTimeDate)
                    .ToList();

                if (!clientConsultations.Any())
                {
                    _logger.LogInformation("No consultation history for this client.");
                    return new GeneralResult<List<ConsultationAllData>>(
                        false, _localizationManager.GetLocalizedString("NoConsultationHistory"), null);
                }

                return new GeneralResult<List<ConsultationAllData>>(
                    true, _localizationManager.GetLocalizedString("ConsultationHistoryRetrieved"), clientConsultations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving client consultation history.");
                return new GeneralResult<List<ConsultationAllData>>(
                    false, _localizationManager.GetLocalizedString("UnexpectedErrorRetrievingClientHistory"), null);
            }
        }

        private async Task<List<ConsultationAllData>> GetConsultationFullDataAsync()
        {
            var consultations = await _dbContext.Consultations
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .Include(c => c.Counselor)
                    .ThenInclude(cn => cn.User)
                .Include(c => c.Client)
                .Include(c => c.ConsultationTime)
                .ToListAsync();

            var now = DateHelper.UtcNow;

            return consultations.Select(c => new ConsultationAllData
            {
                Id = c.Id,
                Type = c.Type,
                Status = c.Status,
                Description = c.Description!,
                ConsultationTimeDate = c.ConsultationTime?.DateTimeSlot ?? now,
                counselorData = new CounselorData
                {
                    Id = c.Counselor.Id,
                    FirstName = c.Counselor.User.FirstName,
                    LastName = c.Counselor.User.LastName,
                    Specialization = c.Counselor.User.Specialization ?? "",
                    CountryCode = c.Counselor.User.CountryCode,
                    Email = c.Counselor.User.Email ?? "",
                    Qualification = c.Counselor.Qualification ?? "",
                    City = c.Counselor.City ?? ""
                },
                customerData = new CustomerData
                {
                    Id = c.Client.Id,
                    FirstName = c.Client.FirstName ?? "",
                    LastName = c.Client.LastName ?? "",
                    Specialization = c.Client.Specialization ?? "",
                    CountryCode = c.Client.CountryCode,
                    Email = c.Client.Email ?? "",
                    NationalId = c.Client.NationalId ?? 0,
                    DateOfBirth = c.Client.DOB
                }
            }).ToList();
        }
    }
}

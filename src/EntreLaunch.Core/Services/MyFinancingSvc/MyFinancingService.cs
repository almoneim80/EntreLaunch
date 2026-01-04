using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyOpportunityDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.MyFinancingSvc
{
    public class MyFinancingService(
        ILogger<MyFinancingService> logger,
        IMapper mapper,
        PgDbContext pgDbContext,
        UserManager<User> userManager,
        ILocalizationManager localizationManager) : IMyFinancingService
    {
        private readonly ILogger<MyFinancingService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly PgDbContext _dbContext = pgDbContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult> Filtering([FromBody] OpportunityFilterDto filter)
        {
            try
            {
                var query = _dbContext.Opportunities
                    .Where(i => !i.IsDeleted && i.Type == OpportunityType.Financing)
                    .AsQueryable();

                if (filter.Costs.HasValue)
                    query = query.Where(i => i.Costs == filter.Costs.Value);

                if (!string.IsNullOrWhiteSpace(filter.Sector))
                    query = query.Where(i => i.Sector != null && i.Sector.Contains(filter.Sector));

                if (filter.BrandCountry.HasValue)
                    query = query.Where(i => i.BrandCountry == filter.BrandCountry);

                var opportunities = _mapper.Map<List<OpportunityDetailsDto>>(await query.ToListAsync());

                if (!opportunities.Any())
                {
                    _logger.LogError("No opportunities found for Filtering operation.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoOpportunitiesFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("FilteredSuccessfully"),
                    Data = opportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to filter opportunities.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToFilterOpportunities"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<decimal>>> GetAllCostsAsync()
        {
            try
            {
                var costs = await _dbContext.Opportunities
                    .Where(o => !o.IsDeleted && o.Type == OpportunityType.Financing && o.Costs.HasValue)
                    .Select(o => o.Costs!.Value)
                    .Distinct()
                    .ToListAsync();

                if (!costs.Any())
                {
                    return new GeneralResult<List<decimal>>(false, _localizationManager.GetLocalizedString("NoCostsFound"), null);
                }

                return new GeneralResult<List<decimal>>(true, _localizationManager.GetLocalizedString("CostsRetrieved"), costs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Costs list.");
                return new GeneralResult<List<decimal>>(false, _localizationManager.GetLocalizedString("UnexpectedError"), null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<List<string>>> GetAllSectorsAsync()
        {
            try
            {
                var sectors = await _dbContext.Opportunities
                    .Where(o => !o.IsDeleted && o.Type == OpportunityType.Financing && o.Sector != null)
                    .Select(o => o.Sector!)
                    .Distinct()
                    .ToListAsync();

                if (!sectors.Any())
                {
                    return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("NoSectorsFound"), null);
                }

                return new GeneralResult<List<string>>(true, _localizationManager.GetLocalizedString("SectorsRetrieved"), sectors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Sectors list.");
                return new GeneralResult<List<string>>(false, _localizationManager.GetLocalizedString("UnexpectedError"), null);
            }
        }

        /// <inheritdoc />
        public Task<GeneralResult> SendRequest([FromBody] CreateOpportunityRequestDto request)
        {
            try
            {
                if (request.userId == null || request.OpportunityId <= 0)
                {
                    _logger.LogError("All request data is required.");
                    return Task.FromResult(new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("RequestDataCannotBeNull"),
                        Data = null
                    });
                }

                var user = _userManager.FindByIdAsync(request.userId);
                if (user == null)
                {
                    _logger.LogError($"No user found with this id: {request.userId}.");
                    return Task.FromResult(new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    });
                }

                var opportunity = _dbContext.Opportunities.FirstOrDefault(o => o.Id == request.OpportunityId && !o.IsDeleted);
                if (opportunity == null)
                {
                    _logger.LogError($"No opportunity found with this id: {request.OpportunityId}.");
                    return Task.FromResult(new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("OpportunityNotFound"),
                        Data = null
                    });
                }

                var opportunityRequest = _mapper.Map<OpportunityRequest>(request);
                opportunityRequest.CreatedAt = DateHelper.UtcNow;
                opportunityRequest.Status = OpportunityRequestStatus.Pending;
                opportunityRequest.IsDeleted = false;
                opportunityRequest.Type = OpportunityType.Financing;
                _dbContext.OpportunityRequests.Add(opportunityRequest);
                _dbContext.SaveChanges();

                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("RequestSentSuccessfully"),
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToSendRequest"),
                    Data = null
                });
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> AllRequests(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.OpportunityRequests
                    .AsNoTracking()
                    .Include(r => r.user)
                    .Include(r => r.Opportunity)
                    .Where(r => !r.IsDeleted && r.Type == OpportunityType.Financing)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => _mapper.Map<OpportunityRequestDetailsDto>(r));

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(true,
                    _localizationManager.GetLocalizedString("RequestsRetrievedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("FailedToGetRequests"),
                    null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> PendingRequests(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.OpportunityRequests
                    .AsNoTracking()
                    .Include(r => r.user)
                    .Include(r => r.Opportunity)
                    .Where(o => !o.IsDeleted && o.Status == OpportunityRequestStatus.Pending && o.Type == OpportunityType.Financing)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(r => _mapper.Map<OpportunityRequestDetailsDto>(r));

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(
                    true, _localizationManager.GetLocalizedString("PendingRequestsRetrievedSuccessfully"), paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch pending requests");
                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(false,
                    _localizationManager.GetLocalizedString("FailedToGetPendingRequests"),
                    null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> AcceptedRequests(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.OpportunityRequests
                    .AsNoTracking()
                    .Include(r => r.user)
                    .Include(r => r.Opportunity)
                    .Where(o => !o.IsDeleted && o.Status == OpportunityRequestStatus.Accepted && o.Type == OpportunityType.Financing)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(r => _mapper.Map<OpportunityRequestDetailsDto>(r));

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(true,
                    _localizationManager.GetLocalizedString("AcceptedRequestsRetrievedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch accepted requests");
                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("FailedToGetAcceptedRequests"),
                    null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> RejectedRequests(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.OpportunityRequests
                    .AsNoTracking()
                    .Include(r => r.user)
                    .Include(r => r.Opportunity)
                    .Where(o => !o.IsDeleted && o.Status == OpportunityRequestStatus.Rejected && o.Type == OpportunityType.Financing)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(r => _mapper.Map<OpportunityRequestDetailsDto>(r));

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(true,
                    _localizationManager.GetLocalizedString("RejectedRequestsRetrievedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch rejected requests");
                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(false,
                    _localizationManager.GetLocalizedString("FailedToGetRejectedRequests"),
                    null);
            }
        }

        /// <inheritdoc />
        public Task<GeneralResult> ProcessRequests(ProcessOpportunityRequestDto processOpportunityRequest)
        {
            if (processOpportunityRequest == null)
            {
                _logger.LogError("No data found.");
                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("NoDataFound"),
                    Data = null
                });
            }

            var opportunityRequest = _dbContext.
                OpportunityRequests.FirstOrDefault(o => o.Id == processOpportunityRequest.Id && !o.IsDeleted && o.Type == OpportunityType.Financing);

            if (opportunityRequest == null)
            {
                _logger.LogError("No data found.");
                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("NoDataFound"),
                    Data = null
                });
            }

            if (opportunityRequest.Status == processOpportunityRequest.Status)
            {
                _logger.LogError($"Opportunity request with Id {processOpportunityRequest.Id} is already {opportunityRequest.Status}");
                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("RequestAlreadyProcessed"),
                    Data = null
                });
            }

            opportunityRequest.Status = processOpportunityRequest.Status;
            opportunityRequest.UpdatedAt = DateHelper.UtcNow;
            _dbContext.OpportunityRequests.Update(opportunityRequest);
            _dbContext.SaveChanges();

            return Task.FromResult(new GeneralResult
            {
                IsSuccess = true,
                Message = _localizationManager.GetLocalizedString("RequestProcessedSuccessfully"),
                Data = null
            });
        }

        /// <inheritdoc />
        public async Task<GeneralResult> AllFinancingOpportunities()
        {
            try
            {
                var investmentOpportunities = _mapper.Map<List<OpportunityDetailsDto>>(await
                    _dbContext.Opportunities.Where(o => !o.IsDeleted && o.Type == OpportunityType.Financing).ToListAsync());

                if (!investmentOpportunities.Any())
                {
                    _logger.LogError("No financing Opportunities found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoFinancingOpportunitiesFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("FinancingOpportunitiesRetrievedSuccessfully"),
                    Data = investmentOpportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToGetFinancingOpportunities"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> DeleteUserRequestAsync(int requestId, string userId, CancellationToken cancellationToken)
        {
            try
            {
                var request = await _dbContext.OpportunityRequests
                    .FirstOrDefaultAsync(r =>
                        r.Id == requestId &&
                        r.UserId == userId &&
                        !r.IsDeleted &&
                        r.Status == OpportunityRequestStatus.Pending &&
                        r.Type == OpportunityType.Financing,
                        cancellationToken);

                if (request == null)
                {
                    _logger.LogWarning("User request not found or cannot be deleted. RequestId: {RequestId}, UserId: {UserId}", requestId, userId);
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("UserRequestNotFoundOrNotDeletable"));
                }

                request.IsDeleted = true;
                request.UpdatedAt = DateHelper.UtcNow;
                _dbContext.OpportunityRequests.Update(request);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new GeneralResult(true, _localizationManager.GetLocalizedString("UserRequestDeletedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user request. RequestId: {RequestId}, UserId: {UserId}", requestId, userId);
                return new GeneralResult(false, _localizationManager.GetLocalizedString("ErrorDeletingUserRequest"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>> GetUserRequestsAsync(string userId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogError("User ID is required.");
                    return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("UserIdIsRequired"),
                        null);
                }

                var userExists = await _userManager.FindByIdAsync(userId);
                if (userExists == null)
                {
                    _logger.LogError($"User not found: {userId}");
                    return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("UserNotFound"),
                        null);
                }

                var query = _dbContext.OpportunityRequests
                    .AsNoTracking()
                    .Include(r => r.Opportunity)
                    .Where(r => r.UserId == userId && !r.IsDeleted && r.Type == OpportunityType.Financing)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => _mapper.Map<OpportunityRequestDetailsDto>(r));

                var paginated = await query.ToPagedResultAsync(pagination, cancellationToken);

                if (!paginated.Items.Any())
                {
                    return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("NoRequestsFoundForUser"),
                        null);
                }

                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("UserRequestsFetchedSuccessfully"),
                    paginated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated user opportunity requests");
                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("UnexpectedError"),
                    null);
            }
        }
    }
}

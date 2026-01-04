using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyOpportunityDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.MyOpportunitySvc
{
    public class OpportunityRequestService(
        ILogger<OpportunityRequestService> logger,
        IMapper mapper,
        PgDbContext pgDbContext,
        UserManager<User> userManager,
        ILocalizationManager localizationManager) : IOpportunityRequestService
    {
        private readonly PgDbContext _dbContext = pgDbContext;
        private readonly ILogger<OpportunityRequestService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult> SendRequest(CreateOpportunityRequestDto request)
        {
            try
            {
                if (request.userId == null || request.OpportunityId <= 0)
                {
                    _logger.LogError("All request data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("RequestDataCannotBeNull"),
                        Data = null
                    };
                }

                var user = await _userManager.FindByIdAsync(request.userId);
                if (user == null)
                {
                    _logger.LogInformation($"No user found with this id: {request.userId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                var opportunity = _dbContext.Opportunities.FirstOrDefault(o => o.Id == request.OpportunityId && !o.IsDeleted);
                if (opportunity == null)
                {
                    _logger.LogInformation($"No opportunity found with this id: {request.OpportunityId}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("OpportunityNotFound"),
                        Data = null
                    };
                }

                var opportunityRequest = _mapper.Map<OpportunityRequest>(request);
                opportunityRequest.CreatedAt = DateTimeOffset.UtcNow;
                opportunityRequest.Status = OpportunityRequestStatus.Pending;
                opportunityRequest.IsDeleted = false;
                opportunityRequest.Type = OpportunityType.Investment;
                _dbContext.OpportunityRequests.Add(opportunityRequest);
                _dbContext.SaveChanges();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("RequestSentSuccessfully"),
                    Data = request
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToSendRequest"),
                    Data = null
                };
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
                    .Where(r => !r.IsDeleted && r.Type == OpportunityType.Investment)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => _mapper.Map<OpportunityRequestDetailsDto>(r));

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(true,
                    _localizationManager.GetLocalizedString("RequestsRetrievedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching all investment opportunity requests.");
                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(false,
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
                    .Where(o => !o.IsDeleted && o.Status == OpportunityRequestStatus.Pending && o.Type == OpportunityType.Investment)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => _mapper.Map<OpportunityRequestDetailsDto>(o));

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(true,
                    _localizationManager.GetLocalizedString("PendingRequestsRetrievedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending requests");
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
                    .Where(o => !o.IsDeleted && o.Status == OpportunityRequestStatus.Accepted && o.Type == OpportunityType.Investment)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => _mapper.Map<OpportunityRequestDetailsDto>(o));

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(true,
                    _localizationManager.GetLocalizedString("AcceptedRequestsRetrievedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get accepted requests");
                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(false,
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
                    .Where(o => !o.IsDeleted && o.Status == OpportunityRequestStatus.Rejected && o.Type == OpportunityType.Investment)
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => _mapper.Map<OpportunityRequestDetailsDto>(o));

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(true,
                    _localizationManager.GetLocalizedString("RejectedRequestsRetrievedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get rejected requests");
                return new GeneralResult<PaginatedResult<OpportunityRequestDetailsDto>>(false,
                    _localizationManager.GetLocalizedString("FailedToGetRejectedRequests"),
                    null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ProcessRequest(ProcessOpportunityRequestDto processOpportunityRequest)
        {
            try
            {
                if (processOpportunityRequest == null)
                {
                    _logger.LogInformation("No data found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoDataFound"),
                        Data = null
                    };
                }

                var opportunityRequest = await _dbContext.
                    OpportunityRequests
                    .FirstOrDefaultAsync(o => o.Id == processOpportunityRequest.Id && !o.IsDeleted && o.Type ==
                    OpportunityType.Investment);

                if (opportunityRequest == null)
                {
                    _logger.LogInformation("No data found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoDataFound"),
                        Data = null
                    };
                }

                if (opportunityRequest.Status == processOpportunityRequest.Status)
                {
                    _logger.LogError($"Opportunity request with Id {processOpportunityRequest.Id} is already {opportunityRequest.Status}");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("RequestAlreadyHasSameStatus"),
                        Data = null
                    };
                }

                opportunityRequest.Status = processOpportunityRequest.Status;
                opportunityRequest.UpdatedAt = DateTimeOffset.UtcNow;
                _dbContext.OpportunityRequests.Update(opportunityRequest);
                _dbContext.SaveChanges();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("RequestProcessedSuccessfully"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToProcessRequest"),
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
                        r.Type == OpportunityType.Investment,
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
                    .Where(r => r.UserId == userId && !r.IsDeleted && r.Type == OpportunityType.Investment)
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

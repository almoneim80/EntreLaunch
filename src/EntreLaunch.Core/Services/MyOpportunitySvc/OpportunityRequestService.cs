namespace EntreLaunch.Services.MyOpportunitySvc
{
    public class OpportunityRequestService : IOpportunityRequestService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<OpportunityRequestService> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public OpportunityRequestService(
            ILogger<OpportunityRequestService> logger,
            IMapper mapper,
            PgDbContext pgDbContext,
            UserManager<User> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = pgDbContext;
            _userManager = userManager;
        }

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
                        Message = "Request data can not be null.",
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
                        Message = "User not found.",
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
                        Message = "Opportunity not found.",
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
                    Message = "Request sent successfully.",
                    Data = request
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to send request.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> AllRequests()
        {
            try
            {
                var opportunities = _mapper.Map<List<OpportunityRequestDetailsDto>>(
                   await _dbContext.OpportunityRequests
                    .Where(r => !r.IsDeleted && r.Type == OpportunityType.Investment).
                    Include(r => r.user)
                    .Include(r => r.Opportunity)
                    .ToListAsync());

                if (!opportunities.Any())
                {
                    _logger.LogInformation("No data found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No data found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Requests retrieved successfully.",
                    Data = opportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get requests.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> PendingRequests()
        {
            try
            {
                var pendingOpportunities = _mapper.Map<List<OpportunityRequestDetailsDto>>(await
                    _dbContext.OpportunityRequests
                    .Where(o => !o.IsDeleted && o.Status ==
                    OpportunityRequestStatus.Pending && o.Type == OpportunityType.Investment)
                    .ToListAsync());

                if (!pendingOpportunities.Any())
                {
                    _logger.LogInformation("No pending Opportunities requests found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No pending Opportunities requests found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Pending requests retrieved successfully.",
                    Data = pendingOpportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get pending requests.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> AcceptedRequests()
        {
            try
            {
                var pendingOpportunities = _mapper.Map<List<OpportunityRequestDetailsDto>>(await
                    _dbContext.OpportunityRequests
                    .Where(o => !o.IsDeleted && o.Status ==
                    OpportunityRequestStatus.Accepted && o.Type == OpportunityType.Investment)
                    .ToListAsync());

                if (!pendingOpportunities.Any())
                {
                    _logger.LogInformation("No accepted Opportunities requests found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No accepted Opportunities requests found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Accepted requests retrieved successfully.",
                    Data = pendingOpportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get accepted requests.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> RejectedRequests()
        {
            try
            {
                var pendingOpportunities = _mapper.Map<List<OpportunityRequestDetailsDto>>(await
                    _dbContext.OpportunityRequests
                    .Where(o => !o.IsDeleted && o.Status ==
                    OpportunityRequestStatus.Rejected && o.Type == OpportunityType.Investment)
                    .ToListAsync());

                if (!pendingOpportunities.Any())
                {
                    _logger.LogInformation("No rejected Opportunities requests found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No rejected Opportunities requests found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Rejected requests retrieved successfully.",
                    Data = pendingOpportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get rejected requests.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ProgressRequests(ProcessOpportunityRequestDto processOpportunityRequest)
        {
            try
            {
                if (processOpportunityRequest == null)
                {
                    _logger.LogInformation("No data found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No data found.",
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
                        Message = "No data found.",
                        Data = null
                    };
                }

                if (opportunityRequest.Status == processOpportunityRequest.Status)
                {
                    _logger.LogError($"Opportunity request with Id {processOpportunityRequest.Id} is already {opportunityRequest.Status}");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = $"Opportunity request is already {opportunityRequest.Status}",
                        Data = null
                    };
                }

                opportunityRequest.Status = processOpportunityRequest.Status;
                opportunityRequest.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                _dbContext.OpportunityRequests.EntreLaunchdate(opportunityRequest);
                _dbContext.SaveChanges();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = $"Request processed to {processOpportunityRequest.Status} successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to process request.",
                    Data = null
                };
            }
        }
    }
}

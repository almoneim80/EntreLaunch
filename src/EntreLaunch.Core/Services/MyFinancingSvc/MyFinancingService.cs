namespace EntreLaunch.Services.MyFinancingSvc
{
    public class MyFinancingService : IMyFinancingService
    {
        private readonly ILogger<MyFinancingService> _logger;
        private readonly IMapper _mapper;
        private readonly PgDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        public MyFinancingService(
            ILogger<MyFinancingService> logger,
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
        public async Task<GeneralResult> Filtering([FromBody] OpportunityFilterDto filter)
        {
            try
            {
                var validator = new FilterOpportunityValidator();
                var validationResult = validator.Validate(filter);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogError("Validation failed: " + string.Join(", ", errors));

                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Validation failed",
                        Data = errors
                    };
                }

                var opportunities = _mapper.Map<List<OpportunityDetailsDto>>(await _dbContext.Opportunities
                    .Where(i => i.Costs.Equals(filter.Costs)
                    && i.Sector!.Contains(filter.Sector!)
                    && i.BrandCountry.Equals(filter.BrandCountry)
                    && !i.IsDeleted && i.Type == OpportunityType.Financing)
                    .ToListAsync());

                if (!opportunities.Any())
                {
                    _logger.LogError("No opportunities found for Filtering operation.");
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
                    Message = "Filtered Successfully.",
                    Data = opportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to filter opportunities.",
                    Data = null
                };
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
                        Message = "Request data can not be null.",
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
                        Message = "User not found.",
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
                        Message = "Opportunity not found.",
                        Data = null
                    });
                }

                var opportunityRequest = _mapper.Map<OpportunityRequest>(request);
                opportunityRequest.CreatedAt = DateTimeOffset.UtcNow;
                opportunityRequest.Status = OpportunityRequestStatus.Pending;
                opportunityRequest.IsDeleted = false;
                opportunityRequest.Type = OpportunityType.Financing;
                _dbContext.OpportunityRequests.Add(opportunityRequest);
                _dbContext.SaveChanges();

                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Request sent successfully.",
                    Data = request
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to send request.",
                    Data = null
                });
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> AllRequests()
        {
            try
            {
                var opportunities = _mapper.Map<List<OpportunityRequestDetailsDto>>(
                   await _dbContext.OpportunityRequests
                    .Where(r => !r.IsDeleted && r.Type == OpportunityType.Financing).
                    Include(r => r.user)
                    .Include(r => r.Opportunity)
                    .ToListAsync());

                if (!opportunities.Any())
                {
                    _logger.LogError("No data found.");
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
        public Task<GeneralResult> PendingRequests()
        {
            try
            {
                var pendingOpportunities = _mapper.Map<List<OpportunityRequestDetailsDto>>(
                    _dbContext.OpportunityRequests
                    .Where(o => !o.IsDeleted && o.Status == OpportunityRequestStatus.Pending && o.Type == OpportunityType.Financing).ToList());

                if (pendingOpportunities == null)
                {
                    _logger.LogError("No pending Opportunities requests found.");
                    return Task.FromResult(new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No pending Opportunities requests found.",
                        Data = null
                    });
                }

                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Pending requests retrieved successfully.",
                    Data = pendingOpportunities
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get pending requests.",
                    Data = null
                });
            }
        }

        /// <inheritdoc />
        public Task<GeneralResult> AcceptedRequests()
        {
            try
            {
                var pendingOpportunities = _mapper.Map<List<OpportunityRequestDetailsDto>>(
                    _dbContext.OpportunityRequests
                    .Where(o => !o.IsDeleted && o.Status == OpportunityRequestStatus.Accepted && o.Type == OpportunityType.Financing).ToList());

                if (pendingOpportunities == null)
                {
                    _logger.LogError("No accepted Opportunities requests found.");
                    return Task.FromResult(new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No accepted Opportunities requests found.",
                        Data = null
                    });
                }

                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Accepted requests retrieved successfully.",
                    Data = pendingOpportunities
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get accepted requests.",
                    Data = null
                });
            }
        }

        /// <inheritdoc />
        public Task<GeneralResult> RejectedRequests()
        {
            try
            {
                var pendingOpportunities = _mapper.Map<List<OpportunityRequestDetailsDto>>(
                    _dbContext.OpportunityRequests
                    .Where(o => !o.IsDeleted && o.Status == OpportunityRequestStatus.Rejected && o.Type == OpportunityType.Financing).ToList());

                if (pendingOpportunities == null)
                {
                    _logger.LogError("No rejected Opportunities requests found.");
                    return Task.FromResult(new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No rejected Opportunities requests found.",
                        Data = null
                    });
                }

                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Rejected requests retrieved successfully.",
                    Data = pendingOpportunities
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get rejected requests.",
                    Data = null
                });
            }
        }

        /// <inheritdoc />
        public Task<GeneralResult> ProgressRequests(ProcessOpportunityRequestDto processOpportunityRequest)
        {
            if (processOpportunityRequest == null)
            {
                _logger.LogError("No data found.");
                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = false,
                    Message = "No data found.",
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
                    Message = "No data found.",
                    Data = null
                });
            }

            if (opportunityRequest.Status == processOpportunityRequest.Status)
            {
                _logger.LogError($"Opportunity request with Id {processOpportunityRequest.Id} is already {opportunityRequest.Status}");
                return Task.FromResult(new GeneralResult
                {
                    IsSuccess = false,
                    Message = $"Opportunity request {processOpportunityRequest.Id} is already {opportunityRequest.Status}",
                    Data = null
                });
            }

            opportunityRequest.Status = processOpportunityRequest.Status;
            opportunityRequest.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
            _dbContext.OpportunityRequests.EntreLaunchdate(opportunityRequest);
            _dbContext.SaveChanges();

            return Task.FromResult(new GeneralResult
            {
                IsSuccess = true,
                Message = $"Request processed to {processOpportunityRequest.Status} successfully.",
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
                        Message = "No financing Opportunities found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Financing Opportunities retrieved successfully.",
                    Data = investmentOpportunities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get financing opportunities.",
                    Data = null
                };
            }
        }
    }
}

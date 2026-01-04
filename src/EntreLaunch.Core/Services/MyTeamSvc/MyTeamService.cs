using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyTeamDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Interfaces.MyTeamIntf;
namespace EntreLaunch.Services.MyTeamSvc
{
    public class MyTeamService(PgDbContext dbContext, IMapper mapper, ILogger<MyTeamService> logger, UserManager<User> userManager, ILocalizationManager localizationManager) : IMyTeamService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<MyTeamService> _logger = logger;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult> CreateEmployeeWithPortfolio(EmployeeCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                {
                    _logger.LogError("All data is required.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("EmployeeDataRequired"),
                        Data = null
                    };
                }

                var user = await _userManager.FindByIdAsync(createDto.UserId);
                if (user == null)
                {
                    _logger.LogError("User not found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                var employee = _mapper.Map<Employee>(createDto);
                employee.Status = EmployeeStaus.Pending;
                employee.IsDeleted = false;
                employee.CreatedAt = DateTimeOffset.UtcNow;


                if (createDto.EmployeePortfolio != null && createDto.EmployeePortfolio.Any())
                {
                    var portfolios = new List<EmployeePortfolio>();
                    foreach (var portfolioDto in createDto.EmployeePortfolio)
                    {
                        var portfolioEntity = _mapper.Map<EmployeePortfolio>(portfolioDto);
                        portfolioEntity.Employee = employee;

                        if (portfolioDto.PortfolioAttachments != null && portfolioDto.PortfolioAttachments.Any())
                        {
                            var attachments = new List<PortfolioAttachment>();
                            foreach (var attachDto in portfolioDto.PortfolioAttachments)
                            {
                                var attachEntity = _mapper.Map<PortfolioAttachment>(attachDto);
                                attachEntity.Portfolio = portfolioEntity;
                                attachments.Add(attachEntity);
                            }
                            portfolioEntity.PortfolioAttachments = attachments;
                        }

                        portfolios.Add(portfolioEntity);
                    }
                    employee.Portfolios = portfolios;
                }

                await _dbContext.Employees.AddAsync(employee);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("EmployeeCreatedSuccessfully"),
                    Data = _mapper.Map<EmployeeDetailsDto>(employee)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create employee with portfolio.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToCreateEmployee"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<EmployeeDetailsDto>>> AllEmployeeRequest(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Employees
                    .Where(e => !e.IsDeleted)
                    .AsNoTracking();

                var totalCount = await query.CountAsync(cancellationToken);

                var employees = await query
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                if (!employees.Any())
                {
                    _logger.LogInformation("No employees found.");
                    return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("NoEmployeesFound"),
                        null
                    );
                }

                var result = new PaginatedResult<EmployeeDetailsDto>
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount,
                    Items = _mapper.Map<List<EmployeeDetailsDto>>(employees)
                };

                return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("EmployeesRetrievedSuccessfully"),
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all employees.");
                return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("FailedToGetEmployees"),
                    null
                );
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ProcessEmployeeRequestStatus(EmployeeRequestDto employeeRequestDto)
        {
            if (employeeRequestDto == null)
            {
                _logger.LogError("No data found.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("AllEmployeeDataRequired"),
                    Data = null
                };
            }

            var project = await _dbContext.Employees
                .FirstOrDefaultAsync(p => p.Id == employeeRequestDto.ProjectId && !p.IsDeleted);

            if (project == null)
            {
                _logger.LogError($"No Employee request found with this id {employeeRequestDto.ProjectId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("EmployeeRequestNotFound"),
                    Data = null
                };
            }

            if (project.Status == employeeRequestDto.Status)
            {
                _logger.LogError($"Employee request with Id {project.Id} is already {employeeRequestDto.Status}");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("EmployeeRequestAlreadyInStatus"),
                    Data = null
                };
            }

            project.Status = employeeRequestDto.Status;
            project.UpdatedAt = DateTimeOffset.UtcNow;
            _dbContext.Employees.Update(project);
            _dbContext.SaveChanges();

            return new GeneralResult
            {
                IsSuccess = true,
                Message = _localizationManager.GetLocalizedString("EmployeeRequestProcessedSuccessfully"),
                Data = null
            };
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<EmployeeDetailsDto>>> PendingEmployees(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Employees
                    .Where(e => !e.IsDeleted && e.Status == EmployeeStaus.Pending)
                    .AsNoTracking();

                var totalCount = await query.CountAsync(cancellationToken);

                var pagedEmployees = await query
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                if (!pagedEmployees.Any())
                {
                    _logger.LogInformation("No pending employees found.");
                    return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("NoPendingEmployeesFound"),
                        null
                    );
                }

                var result = new PaginatedResult<EmployeeDetailsDto>
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount,
                    Items = _mapper.Map<List<EmployeeDetailsDto>>(pagedEmployees)
                };

                return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("PendingEmployeesRetrievedSuccessfully"),
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending employees.");
                return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("FailedToGetPendingEmployees"),
                    null
                );
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<EmployeeDetailsDto>>> AcceptedEmployees(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Employees
                    .Where(e => !e.IsDeleted && e.Status == EmployeeStaus.Accepted)
                    .AsNoTracking();

                var totalCount = await query.CountAsync(cancellationToken);

                var pagedEmployees = await query
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                if (!pagedEmployees.Any())
                {
                    _logger.LogInformation("No accepted employees found.");
                    return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("NoAcceptedEmployeesFound"),
                        null
                    );
                }

                var result = new PaginatedResult<EmployeeDetailsDto>
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount,
                    Items = _mapper.Map<List<EmployeeDetailsDto>>(pagedEmployees)
                };

                return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("AcceptedEmployeesRetrievedSuccessfully"),
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get accepted employees.");
                return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("FailedToGetAcceptedEmployees"),
                    null
                );
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<EmployeeDetailsDto>>> RejectedEmployees(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Employees
                    .Where(e => !e.IsDeleted && e.Status == EmployeeStaus.Rejected)
                    .AsNoTracking();

                var totalCount = await query.CountAsync(cancellationToken);

                var pagedEmployees = await query
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                if (!pagedEmployees.Any())
                {
                    _logger.LogInformation("No rejected employees found.");
                    return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("NoRejectedEmployeesFound"),
                        null
                    );
                }

                var result = new PaginatedResult<EmployeeDetailsDto>
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount,
                    Items = _mapper.Map<List<EmployeeDetailsDto>>(pagedEmployees)
                };

                return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("RejectedEmployeesRetrievedSuccessfully"),
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get rejected employees.");
                return new GeneralResult<PaginatedResult<EmployeeDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("FailedToGetRejectedEmployees"),
                    null
                );
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> FilterAcceptedByWorkField(string workField)
        {
            if (workField == null)
            {
                _logger.LogError("Work field is null");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("WorkFieldCannotBeNull"),
                    Data = null
                };
            }

            var employees = _mapper.Map<List<EmployeeDetailsDto>>(await _dbContext.Employees
                .Where(i => i.WorkField!.Equals(workField)
                && !i.IsDeleted
                && i.Status == EmployeeStaus.Accepted).ToListAsync());

            if (!employees.Any())
            {
                _logger.LogError("No employees found for Filtering operation.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("NoEmployeesFound"),
                    Data = null
                };
            }

            return new GeneralResult
            {
                IsSuccess = true,
                Message = _localizationManager.GetLocalizedString("EmployeesFilteredSuccessfully"),
                Data = employees
            };
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = _mapper.Map<EmployeeDetailsDto>(await _dbContext.Employees
                    .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted && e.Status == EmployeeStaus.Accepted));

                if (employee == null)
                {
                    _logger.LogError($"Employee with id {id} not found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("EmployeeNotFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("EmployeeRetrievedSuccessfully"),
                    Data = employee
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get employee {id}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToGetEmployee"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetPortfoliosByEmployeeId(int employeeId)
        {
            try
            {
                var portfolios = _mapper.Map<List<EmployeePortfolioDetailsDto>>(await _dbContext.EmployeePortfolios
                    .Where(p => p.EmployeeId == employeeId && !p.IsDeleted)
                    .Include(p => p.PortfolioAttachments)
                    .ToListAsync());

                if (!portfolios.Any())
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoPortfoliosFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("PortfoliosRetrievedSuccessfully"),
                    Data = portfolios
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get portfolios for employee {employeeId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToGetPortfolios"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateEmployee(int employeeId, EmployeeUpdateDto updateDto)
        {
            try
            {
                var employee = await _dbContext.Employees
                    .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted);

                if (employee == null)
                {
                    _logger.LogError($"Employee with id {employeeId} not found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("EmployeeNotFound"),
                        Data = null
                    };
                }

                _mapper.Map(updateDto, employee);
                employee.UpdatedAt = DateTimeOffset.UtcNow;

                _dbContext.Employees.Update(employee);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("EmployeeUpdatedSuccessfully"),
                    Data = _mapper.Map<EmployeeDetailsDto>(employee)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update employee {employeeId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToUpdateEmployee"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateEmployeePortfolio(int portfolioId, EmployeePortfolioUpdateDto updateDto)
        {
            try
            {
                var portfolio = await _dbContext.EmployeePortfolios
                    .FirstOrDefaultAsync(p => p.Id == portfolioId && !p.IsDeleted);

                if (portfolio == null)
                {
                    _logger.LogError($"Portfolio with id {portfolioId} not found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("PortfolioNotFound"),
                        Data = null
                    };
                }

                _mapper.Map(updateDto, portfolio);
                portfolio.UpdatedAt = DateTimeOffset.UtcNow;

                _dbContext.EmployeePortfolios.Update(portfolio);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("PortfolioUpdatedSuccessfully"),
                    Data = _mapper.Map<EmployeePortfolioDetailsDto>(portfolio)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update portfolio {portfolioId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToUpdatePortfolio"),
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdatePortfolioAttachment(int attachmentId, PortfolioAttachmentUpdateDto updateDto)
        {
            try
            {
                var attachment = await _dbContext.PortfolioAttachments
                    .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

                if (attachment == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("AttachmentNotFound")
                    };
                }

                _mapper.Map(updateDto, attachment);
                attachment.UpdatedAt = DateTimeOffset.UtcNow;

                _dbContext.PortfolioAttachments.Update(attachment);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("AttachmentUpdatedSuccessfully"),
                    Data = _mapper.Map<PortfolioAttachmentDetailsDto>(attachment)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update attachment {attachmentId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToUpdateAttachment"),
                    Data = null
                };
            }
        }
    }
}

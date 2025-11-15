using EntreLaunch.Interfaces.MyTeamIntf;
namespace EntreLaunch.Services.MyTeamSvc
{
    public class MyTeamService : IMyTeamService
    {
        private readonly PgDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<MyTeamService> _logger;
        private readonly UserManager<User> _userManager;
        public MyTeamService(PgDbContext dbContext, IMapper mapper, ILogger<MyTeamService> logger, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

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
                        Message = "Data is required.",
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
                        Message = "User not found.",
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
                    Message = "Employee (request) created successfully.",
                    Data = _mapper.Map<EmployeeDetailsDto>(employee)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create employee with portfolio.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to create employee.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> AllEmployeeRequest()
        {
            try
            {
                var employees = await _dbContext.Employees.Where(e => !e.IsDeleted).ToListAsync();

                if (!employees.Any())
                {
                    _logger.LogInformation("No employees found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No employees found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Employees retrieved successfully.",
                    Data = _mapper.Map<List<EmployeeDetailsDto>>(employees)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all employees.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get employees.",
                    Data = null
                };
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
                    Message = "all data is required.",
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
                    Message = "No Employee request found.",
                    Data = null
                };
            }

            if (project.Status == employeeRequestDto.Status)
            {
                _logger.LogError($"Employee request with Id {project.Id} is already {employeeRequestDto.Status}");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = $"This Employee request is already in {employeeRequestDto.Status} status.",
                    Data = null
                };
            }

            project.Status = employeeRequestDto.Status;
            project.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
            _dbContext.Employees.EntreLaunchdate(project);
            _dbContext.SaveChanges();

            return new GeneralResult
            {
                IsSuccess = true,
                Message = $"Employee request processed to {employeeRequestDto.Status} status successfully.",
                Data = null
            };
        }

        /// <inheritdoc />
        public async Task<GeneralResult> PendingEmployees()
        {
            try
            {
                var pendingEmployees = _mapper.Map<List<EmployeeDetailsDto>>(
                   await _dbContext.Employees
                    .Where(p => !p.IsDeleted && p.Status == EmployeeStaus.Pending)
                    .ToArrayAsync());

                if (pendingEmployees == null)
                {
                    _logger.LogError("No pending employees found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No pending employees found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Pending employees retrieved successfully.",
                    Data = pendingEmployees
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get pending employees.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> AcceptedEmployees()
        {
            try
            {
                var acceptedEmployees = _mapper.Map<List<EmployeeDetailsDto>>(
                   await _dbContext.Employees
                    .Where(p => !p.IsDeleted && p.Status == EmployeeStaus.Accepted)
                    .ToArrayAsync());

                if (acceptedEmployees == null)
                {
                    _logger.LogError("No accepted employees found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No accepted employees found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Accepted employees retrieved successfully.",
                    Data = acceptedEmployees
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get accepted employees.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> RejectedEmployees()
        {
            try
            {
                var rejectedEmployees = _mapper.Map<List<EmployeeDetailsDto>>(
                   await _dbContext.Employees
                    .Where(p => !p.IsDeleted && p.Status == EmployeeStaus.Rejected)
                    .ToArrayAsync());

                if (rejectedEmployees == null)
                {
                    _logger.LogError("No rejected employees found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No rejected employees found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Rejected employees retrieved successfully.",
                    Data = rejectedEmployees
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get rejected employees.",
                    Data = null
                };
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
                    Message = "Work field can not be null",
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
                    Message = "No employees found.",
                    Data = null
                };
            }

            return new GeneralResult
            {
                IsSuccess = true,
                Message = "Employees filtered Successfully.",
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
                        Message = $"Employee not found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Employee retrieved successfully.",
                    Data = employee
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get employee {id}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get employee.",
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
                        Message = "No portfolios found for this employee.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Portfolios retrieved successfully.",
                    Data = portfolios
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get portfolios for employee {employeeId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get portfolios.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> EntreLaunchdateEmployee(int employeeId, EmployeeEntreLaunchdateDto EntreLaunchdateDto)
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
                        Message = $"Employee not found.",
                        Data = null
                    };
                }

                _mapper.Map(EntreLaunchdateDto, employee);
                employee.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                _dbContext.Employees.EntreLaunchdate(employee);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Employee EntreLaunchdated successfully.",
                    Data = _mapper.Map<EmployeeDetailsDto>(employee)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to EntreLaunchdate employee {employeeId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to EntreLaunchdate employee.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> EntreLaunchdateEmployeePortfolio(int portfolioId, EmployeePortfolioEntreLaunchdateDto EntreLaunchdateDto)
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
                        Message = "Portfolio not found.",
                        Data = null
                    };
                }

                _mapper.Map(EntreLaunchdateDto, portfolio);
                portfolio.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                _dbContext.EmployeePortfolios.EntreLaunchdate(portfolio);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Portfolio EntreLaunchdated successfully.",
                    Data = _mapper.Map<EmployeePortfolioDetailsDto>(portfolio)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to EntreLaunchdate portfolio {portfolioId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to EntreLaunchdate portfolio.",
                    Data = null
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> EntreLaunchdatePortfolioAttachment(int attachmentId, PortfolioAttachmentEntreLaunchdateDto EntreLaunchdateDto)
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
                        Message = "Attachment not found."
                    };
                }

                _mapper.Map(EntreLaunchdateDto, attachment);
                attachment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                _dbContext.PortfolioAttachments.EntreLaunchdate(attachment);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Attachment EntreLaunchdated successfully.",
                    Data = _mapper.Map<PortfolioAttachmentDetailsDto>(attachment)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to EntreLaunchdate attachment {attachmentId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to EntreLaunchdate attachment.",
                    Data = null
                };
            }
        }
    }
}

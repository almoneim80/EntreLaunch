namespace EntreLaunch.Services.MyPartnerSvc
{
    public class MyPartnerProjectService : IMyPartnerProjectService
    {
        private readonly PgDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<MyPartnerService> _logger;
        public MyPartnerProjectService(
            PgDbContext dbContext,
            IMapper mapper,
            ILogger<MyPartnerService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create a new MyPartner project with attachments.
        /// </summary>
        public async Task<GeneralResult> CreateProjectWithAttachments(MyPartnerCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                {
                    _logger.LogError("MyPartnerCreateDto (All data is required)");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "All data is required.",
                        Data = null
                    };
                }

                var myPartnerEntity = _mapper.Map<Entities.MyPartner>(createDto);
                myPartnerEntity.Status = MyPartnerStatus.Pending;
                myPartnerEntity.IsDeleted = false;
                myPartnerEntity.CreatedAt = DateTimeOffset.UtcNow;

                if (createDto.Attachments != null && createDto.Attachments.Any())
                {
                    var attachments = _mapper.Map<List<MyPartnerAttachment>>(createDto.Attachments);
                    foreach (var att in attachments)
                    {
                        att.Project = myPartnerEntity;
                    }

                    myPartnerEntity.ProjectAttachments = attachments;
                }

                await _dbContext.MyPartners.AddAsync(myPartnerEntity);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "MyPartner with attachments created successfully.",
                    Data = _mapper.Map<MyPartnerDetailsDto>(myPartnerEntity)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create MyPartner.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to create MyPartner.",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Get all projects.
        /// </summary>
        public async Task<GeneralResult> AllProjects()
        {
            try
            {
                var projects = _mapper.Map<List<MyPartnerDetailsDto>>(
                   await _dbContext.MyPartners
                    .Where(p => !p.IsDeleted).
                    Include(p => p.ProjectAttachments)
                    .Include(p => p.ProjectAttachments)
                    .ToListAsync());

                if (!projects.Any())
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
                    Message = "Projects data retrieved successfully.",
                    Data = projects
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get prokects.",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Progress Projects status (Accepted, Rejected).
        /// </summary>
        public async Task<GeneralResult> ProgressProjects(ProcessProjectsDto processDto)
        {
            if (processDto == null)
            {
                _logger.LogError("No data found.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "all data is required.",
                    Data = null
                };
            }

            var project = await _dbContext.MyPartners
                .FirstOrDefaultAsync(p => p.Id == processDto.ProjectId && !p.IsDeleted);

            if (project == null)
            {
                _logger.LogError($"No project found with this id {processDto.ProjectId}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "No project found.",
                    Data = null
                };
            }

            if (project.Status == processDto.Status)
            {
                _logger.LogError($"Project with Id {project.Id} is already {processDto.Status}");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = $"This Project is already in {processDto.Status} status.",
                    Data = null
                };
            }

            project.Status = processDto.Status;
            project.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
            _dbContext.MyPartners.EntreLaunchdate(project);
            _dbContext.SaveChanges();

            return new GeneralResult
            {
                IsSuccess = true,
                Message = $"Project processed to {processDto.Status} status successfully.",
                Data = null
            };
        }

        /// <summary>
        /// Get all pending projects.
        /// </summary>
        public async Task<GeneralResult> PendingProjects()
        {
            try
            {
                var pendingProjects = _mapper.Map<List<MyPartnerDetailsDto>>(
                   await _dbContext.MyPartners
                    .Where(p => !p.IsDeleted && p.Status == MyPartnerStatus.Pending)
                    .Include(p => p.ProjectAttachments)
                    .ToListAsync());

                if (!pendingProjects.Any())
                {
                    _logger.LogError("No pending projects found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No pending projects found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Pending projects retrieved successfully.",
                    Data = pendingProjects
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get pending projects.",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Get all accepted projects.
        /// </summary>
        public async Task<GeneralResult> AcceptedProjects()
        {
            try
            {
                var acceptedProjects = _mapper.Map<List<MyPartnerDetailsDto>>(
                   await _dbContext.MyPartners
                    .Where(p => !p.IsDeleted && p.Status == MyPartnerStatus.Accepted)
                    .Include(p => p.ProjectAttachments)
                    .ToListAsync());

                if (!acceptedProjects.Any())
                {
                    _logger.LogError("No accepted projects found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No accepted projects found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Accepted project retrieved successfully.",
                    Data = acceptedProjects
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get accepted projects.",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Get all rejected projects.
        /// </summary>
        public async Task<GeneralResult> RejectedProjects()
        {
            try
            {
                var rejectedProjects = _mapper.Map<List<MyPartnerDetailsDto>>(
                   await _dbContext.MyPartners
                    .Where(p => !p.IsDeleted && p.Status == MyPartnerStatus.Rejected)
                    .Include(p => p.ProjectAttachments)
                    .ToListAsync());

                if (!rejectedProjects.Any())
                {
                    _logger.LogError("No rejected projects found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No rejected projects found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Rejected project retrieved successfully.",
                    Data = rejectedProjects
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get rejected projects.",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Get one project by its id.
        /// </summary>
        public async Task<GeneralResult> GetProjectById(int id)
        {
            try
            {
                var project = _mapper.Map<MyPartnerDetailsDto>(
                   await _dbContext.MyPartners
                   .Where(p => !p.IsDeleted && p.Id == id)
                   .Include(p => p.ProjectAttachments)
                   .FirstOrDefaultAsync());

                if (project == null)
                {
                    _logger.LogError($"No project found with this id {id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No project found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Project retrieved successfully.",
                    Data = project
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get project.",
                    Data = null
                };
            }
        }


        /// <summary>
        /// EntreLaunchdate project.
        /// </summary>>
        public async Task<GeneralResult> EntreLaunchdateProject(int id, MyPartnerEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var project = await _dbContext.MyPartners
                    .Include(p => p.ProjectAttachments)
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (project == null)
                {
                    _logger.LogError($"Project with id {id} not found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Project not found."
                    };
                }

                _mapper.Map(EntreLaunchdateDto, project);
                project.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                _dbContext.MyPartners.EntreLaunchdate(project);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Project EntreLaunchdated successfully.",
                    Data = _mapper.Map<MyPartnerDetailsDto>(project)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to EntreLaunchdate project {id}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to EntreLaunchdate project.",
                    Data = null
                };
            }
        }
    }
}

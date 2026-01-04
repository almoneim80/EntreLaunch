using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyPartnerDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.MyPartnerSvc
{
    public class MyPartnerProjectService(
        PgDbContext dbContext,
        IMapper mapper,
        ILogger<MyPartnerService> logger,
        ILocalizationManager localizationManager) : IMyPartnerProjectService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<MyPartnerService> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localizationManager;

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
                        Message = _localizationManager.GetLocalizedString("AllDataIsRequired"),
                        Data = null
                    };
                }

                var myPartnerEntity = _mapper.Map<Entities.MyPartner>(createDto);
                myPartnerEntity.Status = MyPartnerStatus.Pending;
                myPartnerEntity.IsDeleted = false;
                myPartnerEntity.CreatedAt = DateHelper.UtcNow;

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
                    Message = _localizationManager.GetLocalizedString("ProjectCreatedSuccessfully"),
                    Data = _mapper.Map<MyPartnerDetailsDto>(myPartnerEntity)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create MyPartner.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToCreateProject"),
                    Data = null
                };
            }
        }

        /// <summary>
        /// Get all projects.
        /// </summary>
        public async Task<GeneralResult<PaginatedResult<MyPartnerDetailsDto>>> AllProjects(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.MyPartners
                    .AsNoTracking()
                    .Include(p => p.ProjectAttachments)
                    .Where(p => !p.IsDeleted)
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => _mapper.Map<MyPartnerDetailsDto>(p));

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(true,
                    _localizationManager.GetLocalizedString("ProjectsRetrievedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch partner projects");
                return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(false,
                    _localizationManager.GetLocalizedString("FailedToGetProjects"),
                    null);
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
                    Message = _localizationManager.GetLocalizedString("AllProjectDataRequired"),
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
                    Message = _localizationManager.GetLocalizedString("ProjectNotFound"),
                    Data = null
                };
            }

            if (project.Status == processDto.Status)
            {
                _logger.LogError($"Project with Id {project.Id} is already {processDto.Status}");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("ProjectAlreadyInStatus"),
                    Data = null
                };
            }

            project.Status = processDto.Status;
            project.UpdatedAt = DateHelper.UtcNow;
            _dbContext.MyPartners.Update(project);
            _dbContext.SaveChanges();

            return new GeneralResult
            {
                IsSuccess = true,
                Message = _localizationManager.GetLocalizedString("ProjectStatusUpdatedSuccessfully"),
                Data = null
            };
        }

        /// <summary>
        /// Get all pending projects.
        /// </summary>
        public async Task<GeneralResult<PaginatedResult<MyPartnerDetailsDto>>> PendingProjects(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.MyPartners
                    .Where(p => !p.IsDeleted && p.Status == MyPartnerStatus.Pending)
                    .Include(p => p.ProjectAttachments)
                    .AsNoTracking();

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                if (!items.Any())
                {
                    _logger.LogInformation("No pending projects found.");
                    return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("NoPendingProjectsFound"),
                        null);
                }

                var result = new PaginatedResult<MyPartnerDetailsDto>
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount,
                    Items = _mapper.Map<List<MyPartnerDetailsDto>>(items)
                };

                return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("PendingProjectsRetrievedSuccessfully"),
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending projects.");
                return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("FailedToGetPendingProjects"),
                    null);
            }
        }

        /// <summary>
        /// Get all accepted projects.
        /// </summary>
        public async Task<GeneralResult<PaginatedResult<MyPartnerDetailsDto>>> AcceptedProjects(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.MyPartners
                    .Where(p => !p.IsDeleted && p.Status == MyPartnerStatus.Accepted)
                    .Include(p => p.ProjectAttachments)
                    .AsNoTracking();

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                if (!items.Any())
                {
                    _logger.LogInformation("No accepted projects found.");
                    return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("NoAcceptedProjectsFound"),
                        null);
                }

                var result = new PaginatedResult<MyPartnerDetailsDto>
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount,
                    Items = _mapper.Map<List<MyPartnerDetailsDto>>(items)
                };

                return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("AcceptedProjectsRetrievedSuccessfully"),
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get accepted projects.");
                return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("FailedToGetAcceptedProjects"),
                    null);
            }
        }

        /// <summary>
        /// Get all rejected projects.
        /// </summary>
        public async Task<GeneralResult<PaginatedResult<MyPartnerDetailsDto>>> RejectedProjects(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.MyPartners
                    .Where(p => !p.IsDeleted && p.Status == MyPartnerStatus.Rejected)
                    .Include(p => p.ProjectAttachments)
                    .AsNoTracking();

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                if (!items.Any())
                {
                    _logger.LogInformation("No rejected projects found.");
                    return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("NoRejectedProjectsFound"),
                        null);
                }

                var result = new PaginatedResult<MyPartnerDetailsDto>
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount,
                    Items = _mapper.Map<List<MyPartnerDetailsDto>>(items)
                };

                return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("RejectedProjectsRetrievedSuccessfully"),
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get rejected projects.");
                return new GeneralResult<PaginatedResult<MyPartnerDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("FailedToGetRejectedProjects"),
                    null);
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
                   .Where(p => !p.IsDeleted && p.Id == id && p.Status == MyPartnerStatus.Accepted)
                   .Include(p => p.ProjectAttachments)
                   .FirstOrDefaultAsync());

                if (project == null)
                {
                    _logger.LogError($"No project found with this id {id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoDataFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("ProjectRetrievedSuccessfully"),
                    Data = project
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToGetProject"),
                    Data = null
                };
            }
        }

        /// <summary>
        /// update project.
        /// </summary>>
        public async Task<GeneralResult> UpdateProject(int id, MyPartnerUpdateDto updateDto)
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
                        Message = _localizationManager.GetLocalizedString("ProjectNotFoundForUpdate")
                    };
                }

                _mapper.Map(updateDto, project);
                project.UpdatedAt = DateHelper.UtcNow;

                _dbContext.MyPartners.Update(project);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("ProjectUpdatedSuccessfully"),
                    Data = _mapper.Map<MyPartnerDetailsDto>(project)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update project {id}.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToUpdateProject"),
                    Data = null
                };
            }
        }
    }
}

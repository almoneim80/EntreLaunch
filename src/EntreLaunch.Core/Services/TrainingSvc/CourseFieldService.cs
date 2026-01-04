using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.TrainingSvc
{
    public class CourseFieldService(
        ILocalizationManager localizationManager,
        ILogger<CourseFieldService> logger,
        PgDbContext dbContext,
        ICacheService cacheService,
        IOptions<CacheSettings> cacheSettings) : ICourseFieldService
    {
        private readonly ILocalizationManager _localizationManager = localizationManager;
        private readonly ILogger<CourseFieldService> _logger = logger;
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ICacheService _cacheService = cacheService;
        private readonly CacheSettings _cacheSettings = cacheSettings.Value;

        /// <inheritdoc/>
        public async Task<GeneralResult<CourseFieldDetailsDto>> CreateAsync(CourseFieldCreateDto dto, CancellationToken cancellationToken)
        {
            const string method = nameof(CreateAsync);

            if (dto == null)
            {
                _logger.LogWarning("{Method}: Input DTO is null.", method);
                return new GeneralResult<CourseFieldDetailsDto>(false, _localizationManager.GetLocalizedString("DtoIsNull"), null, ErrorType.Validation);
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("{Method}: Course field name is required.", method);
                return new GeneralResult<CourseFieldDetailsDto>(false, _localizationManager.GetLocalizedString("CourseFieldNameRequired"), null, ErrorType.Validation);
            }

            try
            {
                var entity = new CourseField
                {
                    Name = dto.Name!.Trim(),
                    Description = dto.Description?.Trim(),
                    CreatedAt = dto.CreatedAt,
                    IsDeleted = false
                };

                await _dbContext.CourseFields.AddAsync(entity, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("{Method}: Course field created successfully with ID {Id}.", method, entity.Id);

                return new GeneralResult<CourseFieldDetailsDto>(true, _localizationManager.GetLocalizedString("EntityCreatedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: An error occurred while creating CourseField.", method);
                return new GeneralResult<CourseFieldDetailsDto>(false, _localizationManager.GetLocalizedString("ErrorCreatingEntity"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<CourseFieldDetailsDto>> UpdateAsync(int id, CourseFieldUpdateDto dto, CancellationToken cancellationToken)
        {
            const string method = nameof(UpdateAsync);

            if (dto == null)
            {
                _logger.LogWarning("{Method}: Update DTO is null.", method);
                return new GeneralResult<CourseFieldDetailsDto>(false, _localizationManager.GetLocalizedString("DtoIsNull"), null, ErrorType.Validation);
            }

            try
            {
                var now = DateHelper.UtcNow;
                var entity = await _dbContext.CourseFields.FirstOrDefaultAsync(
                    c => c.Id == id && !c.IsDeleted,
                    cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("{Method}: Course field with ID {Id} not found.", method, id);
                    return new GeneralResult<CourseFieldDetailsDto>(false, _localizationManager.GetLocalizedString("EntityNotFound"), null, ErrorType.NotFound);
                }

                var hasChanges = false;

                if (dto.Name != null && dto.Name.Trim() != entity.Name)
                {
                    entity.Name = dto.Name.Trim();
                    hasChanges = true;
                }

                if (dto.Description != null && dto.Description.Trim() != entity.Description)
                {
                    entity.Description = dto.Description.Trim();
                    hasChanges = true;
                }

                if (!hasChanges)
                {
                    _logger.LogInformation("{Method}: No changes detected for CourseField with ID {Id}.", method, id);
                    return new GeneralResult<CourseFieldDetailsDto>(true, _localizationManager.GetLocalizedString("NoChangesDetected"), new CourseFieldDetailsDto
                    {
                        Id = entity.Id,
                        Name = entity.Name!,
                        Description = entity.Description!,
                        CreatedAt = entity.CreatedAt ?? now,
                        UpdatedAt = entity.UpdatedAt ?? entity.CreatedAt ?? now,
                    });
                }

                entity.UpdatedAt = dto.UpdatedAt;

                await _dbContext.SaveChangesAsync(cancellationToken);

                var resultDto = new CourseFieldDetailsDto
                {
                    Id = entity.Id,
                    Name = entity.Name!,
                    Description = entity.Description!,
                    CreatedAt = entity.CreatedAt ?? now,
                    UpdatedAt = entity.UpdatedAt ?? entity.CreatedAt ?? now
                };

                _logger.LogInformation("{Method}: Course field with ID {Id} updated successfully.", method, id);

                return new GeneralResult<CourseFieldDetailsDto>(true, _localizationManager.GetLocalizedString("EntityUpdatedSuccessfully"), resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method}: Error while updating CourseField with ID {Id}.", method, id);
                return new GeneralResult<CourseFieldDetailsDto>(false, _localizationManager.GetLocalizedString("ErrorUpdatingEntity"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<PaginatedResult<CourseFieldDetailsDto>>> GetAllAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            if (pagination == null)
            {
                _logger.LogWarning("Pagination parameters are null.");
                return new GeneralResult<PaginatedResult<CourseFieldDetailsDto>>(false, _localizationManager.GetLocalizedString("PaginationParamsNull"), null);
            }

            var cacheKey = $"coursefield_all_p{pagination.Page}_s{pagination.PageSize}";
            var cachedData = await _cacheService.GetAsync<PaginatedResult<CourseFieldDetailsDto>>(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("Retrieved CourseField data from cache: {CacheKey}", cacheKey);
                return new GeneralResult<PaginatedResult<CourseFieldDetailsDto>>(true, _localizationManager.GetLocalizedString("DataRetrievedFromCache"), cachedData);
            }

            try
            {
                var baseQuery = _dbContext.CourseFields
                    .AsNoTracking()
                    .Where(c => !c.IsDeleted);

                var totalCount = await baseQuery.CountAsync(cancellationToken);

                var pagedItems = await baseQuery
                    .OrderByDescending(c => c.UpdatedAt)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .Select(c => new CourseFieldDetailsDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        UpdatedAt = c.UpdatedAt ?? DateTimeOffset.UtcNow
                    })
                    .ToListAsync(cancellationToken);

                var result = new PaginatedResult<CourseFieldDetailsDto>
                {
                    Items = pagedItems,
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount
                };

                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes));

                _logger.LogInformation("CourseField data retrieved from database.");
                return new GeneralResult<PaginatedResult<CourseFieldDetailsDto>>(true, _localizationManager.GetLocalizedString("DataRetrievedFromDatabase"), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving CourseField data.");
                return new GeneralResult<PaginatedResult<CourseFieldDetailsDto>>(false, _localizationManager.GetLocalizedString("ErrorRetrievingAllRecords"), null);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<CourseFieldDetailsDto>> GetOneAsync(int id, CancellationToken cancellationToken)
        {
            var cacheKey = $"coursefield_one_{id}";
            var cachedData = await _cacheService.GetAsync<CourseFieldDetailsDto>(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("Retrieved CourseField with ID {Id} from cache.", id);
                return new GeneralResult<CourseFieldDetailsDto>(true, _localizationManager.GetLocalizedString("DataRetrievedFromCache"), cachedData);
            }

            try
            {
                var entity = await _dbContext.CourseFields
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("CourseField with ID {Id} not found.", id);
                    return new GeneralResult<CourseFieldDetailsDto>(false, _localizationManager.GetLocalizedString("EntityNotFound"), null);
                }

                var dto = new CourseFieldDetailsDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    UpdatedAt = entity.UpdatedAt ?? DateTimeOffset.UtcNow
                };

                await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(_cacheSettings.CacheExpirationMinutes));

                _logger.LogInformation("Successfully retrieved CourseField with ID {Id}.", id);
                return new GeneralResult<CourseFieldDetailsDto>(true, _localizationManager.GetLocalizedString("DataRetrievedFromDatabase"), dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving CourseField with ID {Id}.", id);
                return new GeneralResult<CourseFieldDetailsDto>(false, _localizationManager.GetLocalizedString("ErrorRetrievingOneRecord"), null);
            }
        }
    }
}

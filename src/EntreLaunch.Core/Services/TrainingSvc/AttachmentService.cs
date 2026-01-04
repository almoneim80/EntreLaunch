using EntreLaunch.DTOs.LessonDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.TrainingSvc
{
    public class AttachmentService(
        PgDbContext dbContext,
        ILogger<AttachmentService> logger,
        IMapper mapper,
        ILocalizationManager localizationManager,
        IConfiguration configuration) : IAttachmentService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<AttachmentService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _configuration = configuration;

        /// <inheritdoc />
        public async Task<GeneralResult> IncrementAttachmentOpenCountAsync(int attachmentId)
        {
            try
            {
                // Search for the desired facility
                var attachment = await _dbContext.LessonAttachments
                    .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

                if (attachment == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = localizationManager.GetLocalizedString("AttachmentNotFound")
                    };
                }

                // Increase counter
                attachment.OpenCount++;
                _dbContext.LessonAttachments.Update(attachment);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = localizationManager.GetLocalizedString("AttachmentCountIncremented")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing open count for attachment ID {AttachmentId}", attachmentId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = localizationManager.GetLocalizedString("ErrorIncrementingAttachmentCount")
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<AttachmentStatsDto?>> GetAttachmentStatsAsync(int attachmentId)
        {
            try
            {
                // Facility Search
                var attachment = await _dbContext.LessonAttachments.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == attachmentId && !a.IsDeleted);

                if (attachment == null)
                {
                    return new GeneralResult<AttachmentStatsDto?>(false, localizationManager.GetLocalizedString("AttachmentNotFound"), null);
                }

                // Set up statistics as a DTO object
                return new GeneralResult<AttachmentStatsDto?>(true, localizationManager.GetLocalizedString("AttachmentStatsRetrieved"),
                    new AttachmentStatsDto
                    {
                        AttachmentId = attachment.Id,
                        FileName = attachment.FileName,
                        OpenCount = attachment.OpenCount,
                        CreatedAt = attachment.CreatedAt ?? DateTimeOffset.UtcNow,
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for attachment ID {AttachmentId}", attachmentId);
                return new GeneralResult<AttachmentStatsDto?>(false, localizationManager.GetLocalizedString("ErrorRetrievingAttachmentStats"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> IsValidFile(string filePath)
        {
            try
            {
                // Extracting the extension from the path
                var fileExtension = await Task.Run(() => Path.GetExtension(filePath).ToLowerInvariant());

                // Read the settings for all categories from appsettings.json
                var allCategories = await Task.Run(() => _configuration.GetSection("FileUploadSettings").Get<Dictionary<string, FileCategorySettings>>());
                if (allCategories == null || allCategories.Count == 0)
                {
                    return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("NoFileCategoriesConfigured"), false);
                }

                // Find the category matching the extension
                var matchingCategory = allCategories.FirstOrDefault(c => c.Value.Extensions.Contains(fileExtension));
                if (matchingCategory.Value == null)
                {
                    return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("FileTypeNotAllowed"), false);
                }

                // Check file size based on category
                var maxSize = matchingCategory.Value.MaxSizePerExtension.ContainsKey(fileExtension)
                    ? matchingCategory.Value.MaxSizePerExtension[fileExtension]
                    : matchingCategory.Value.MaxSizePerExtension["default"];

                var fileInfo = new FileInfo(filePath);
                if (!IsFileSizeValid(fileInfo.Length, maxSize))
                {
                    return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("FileSizeExceeded"), false);
                }

                return new GeneralResult<bool>(true, localizationManager.GetLocalizedString("FileIsValid"), true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating the file.");
                return new GeneralResult<bool>(false, localizationManager.GetLocalizedString("ErrorValidatingFile"), false);
            }
        }

        /// <summary>
        /// Checks if the file size is within the allowed range.
        /// </summary>
        private bool IsFileSizeValid(long fileSizeInBytes, string maxSize)
        {
            var sizeUnit = maxSize[^2..].ToUpper(); // Last two characters (KB, MB, GB)
            var sizeValue = double.Parse(maxSize[..^2]); // Digital Value

            var sizeInBytes = sizeUnit switch
            {
                "KB" => sizeValue * 1024,
                "MB" => sizeValue * 1024 * 1024,
                "GB" => sizeValue * 1024 * 1024 * 1024,
                _ => throw new ArgumentException($"Invalid size unit: {sizeUnit}")
            };

            return fileSizeInBytes <= sizeInBytes;
        }
    }
}

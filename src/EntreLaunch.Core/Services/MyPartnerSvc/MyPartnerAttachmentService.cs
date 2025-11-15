namespace EntreLaunch.Services.MyPartnerSvc
{
    public class MyPartnerAttachmentService : IMyPartnerAttachmentService
    {
        private readonly PgDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<MyPartnerService> _logger;
        public MyPartnerAttachmentService(
            PgDbContext dbContext,
            IMapper mapper,
            ILogger<MyPartnerService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get attachments of project by project id.
        /// </summary>
        public async Task<GeneralResult> GetProjectAttachments(int id)
        {
            try
            {
                var attachments = _mapper.Map<List<ProjectAttachmentDetailsDto>>(
                   await _dbContext.MyPartnerAttachments.Where(p => !p.IsDeleted && p.ProjectId == id).ToListAsync());

                if (!attachments.Any())
                {
                    _logger.LogError($"No attachments found with this id {id}.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No attachments found.",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Project attachments retrieved successfully.",
                    Data = attachments
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get project attachments.",
                    Data = null
                };
            }
        }

        /// <summary>
        /// EntreLaunchdate attachments.
        /// </summary>>
        public async Task<GeneralResult> EntreLaunchdateAttachments(int id, ProjectAttachmentEntreLaunchdateDto EntreLaunchdateDto)
        {
            try
            {
                var attachment = await _dbContext.MyPartnerAttachments
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (attachment == null)
                {
                    _logger.LogError($"Attachment with id {id} not found.");
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Attachment not found."
                    };
                }

                _mapper.Map(EntreLaunchdateDto, attachment);
                attachment.EntreLaunchdatedAt = DateTimeOffset.UtcNow;

                _dbContext.MyPartnerAttachments.EntreLaunchdate(attachment);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Attachment EntreLaunchdated successfully.",
                    Data = _mapper.Map<ProjectAttachmentDetailsDto>(attachment)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to EntreLaunchdate attachment {id}.");
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

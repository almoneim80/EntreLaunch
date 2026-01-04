using EntreLaunch.DTOs.MyPartnerDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Services.MyPartnerSvc
{
    public class MyPartnerAttachmentService(
        PgDbContext dbContext,
        IMapper mapper,
        ILogger<MyPartnerService> logger,
        ILocalizationManager localizationManager) : IMyPartnerAttachmentService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<MyPartnerService> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localizationManager;

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
                        Message = _localizationManager.GetLocalizedString("NoAttachmentsFound"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("AttachmentsRetrievedSuccessfully"),
                    Data = attachments
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("FailedToGetAttachments"),
                    Data = null
                };
            }
        }

        /// <summary>
        /// update attachments.
        /// </summary>>
        public async Task<GeneralResult> UpdateAttachments(int id, ProjectAttachmentUpdateDto updateDto)
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
                        Message = _localizationManager.GetLocalizedString("AttachmentNotFound")
                    };
                }

                _mapper.Map(updateDto, attachment);
                attachment.UpdatedAt = DateTimeOffset.UtcNow;

                _dbContext.MyPartnerAttachments.Update(attachment);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("AttachmentUpdatedSuccessfully"),
                    Data = _mapper.Map<ProjectAttachmentDetailsDto>(attachment)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update attachment {id}.");
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

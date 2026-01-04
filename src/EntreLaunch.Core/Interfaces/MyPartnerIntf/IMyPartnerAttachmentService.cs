using EntreLaunch.DTOs.MyPartnerDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.MyPartnerIntf
{
    public interface IMyPartnerAttachmentService
    {
        /// <summary>
        /// Get attachments of project by project id.
        /// </summary>
        Task<GeneralResult> GetProjectAttachments(int id);

        /// <summary>
        /// update attachments.
        /// </summary>>
        Task<GeneralResult> UpdateAttachments(int id, ProjectAttachmentUpdateDto updateDto);
    }
}

using EntreLaunch.DTOs.LessonDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface IAttachmentService
    {
        /// <summary>
        /// Increment attachment open count.
        /// </summary>
        Task<GeneralResult> IncrementAttachmentOpenCountAsync(int attachmentId);

        /// <summary>
        /// Get attachment stats.
        /// </summary>
        Task<GeneralResult<AttachmentStatsDto?>> GetAttachmentStatsAsync(int attachmentId);

        /// <summary>
        /// Validates the file is valid.
        /// </summary>
        Task<GeneralResult<bool>> IsValidFile(string filePath);
    }
}

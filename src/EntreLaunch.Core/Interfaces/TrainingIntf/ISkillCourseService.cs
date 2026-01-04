using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface ISkillCourseService
    {
        /// <summary>
        /// Retrieves all skills courses by related field.
        /// </summary>
        Task<GeneralResult<List<SkillCourseDetailsDto>>> GetByFieldAsync(int fieldId);
    }
}

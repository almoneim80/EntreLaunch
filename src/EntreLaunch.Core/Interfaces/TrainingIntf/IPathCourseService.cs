using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface IPathCourseService
    {
        /// <summary>
        /// Retrieves all skills courses by related field.
        /// </summary>
        Task<GeneralResult<List<PathCourseDetailsDto>>> GetByPathAsync(int pathId);
    }
}

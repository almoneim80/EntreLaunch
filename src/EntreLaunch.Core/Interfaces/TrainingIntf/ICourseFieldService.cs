using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface ICourseFieldService
    {
        /// <summary>
        /// Creates a new CourseField record.
        /// </summary>
        /// <param name="dto">The data transfer object containing CourseField creation data.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>A result wrapping the created CourseField details.</returns>
        Task<GeneralResult<CourseFieldDetailsDto>> CreateAsync(CourseFieldCreateDto dto, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing CourseField record by ID.
        /// </summary>
        /// <param name="id">The ID of the CourseField to update.</param>
        /// <param name="dto">The data transfer object containing updated values.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>A result wrapping the updated CourseField details.</returns>
        Task<GeneralResult<CourseFieldDetailsDto>> UpdateAsync(int id, CourseFieldUpdateDto dto, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a single CourseField record by ID.
        /// </summary>
        /// <param name="id">The ID of the CourseField to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>A result wrapping the CourseField details.</returns>
        Task<GeneralResult<CourseFieldDetailsDto>> GetOneAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a paginated list of CourseField records.
        /// </summary>
        /// <param name="pagination">Pagination parameters including page number and size.</param>
        /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
        /// <returns>A result wrapping the paginated CourseField list.</returns>
        Task<GeneralResult<PaginatedResult<CourseFieldDetailsDto>>> GetAllAsync(PaginationParams pagination, CancellationToken cancellationToken);
    }
}

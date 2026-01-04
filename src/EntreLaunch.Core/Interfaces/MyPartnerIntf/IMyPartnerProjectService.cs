using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyPartnerDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.MyPartnerIntf
{
    public interface IMyPartnerProjectService
    {
        /// <summary>
        /// create nwe project.
        /// </summary>>
        Task<GeneralResult> CreateProjectWithAttachments(MyPartnerCreateDto createDto);

        /// <summary>
        /// update project.
        /// </summary>>
        Task<GeneralResult> UpdateProject(int id, MyPartnerUpdateDto updateDto);

        /// <summary>
        /// Progress Projects status (Accepted, Rejected).
        /// </summary>
        Task<GeneralResult> ProgressProjects([FromBody] ProcessProjectsDto processDto);

        /// <summary>
        /// Get one project by its id.
        /// </summary>
        Task<GeneralResult> GetProjectById(int id);

        /// <summary>
        /// Get all pending projects.
        /// </summary>
        Task<GeneralResult<PaginatedResult<MyPartnerDetailsDto>>> PendingProjects(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all accepted projects.
        /// </summary>
        Task<GeneralResult<PaginatedResult<MyPartnerDetailsDto>>> AcceptedProjects(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all rejected projects.
        /// </summary>
        Task<GeneralResult<PaginatedResult<MyPartnerDetailsDto>>> RejectedProjects(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Get all projects.
        /// </summary>
        Task<GeneralResult<PaginatedResult<MyPartnerDetailsDto>>> AllProjects(PaginationParams pagination, CancellationToken cancellationToken);
    }
}

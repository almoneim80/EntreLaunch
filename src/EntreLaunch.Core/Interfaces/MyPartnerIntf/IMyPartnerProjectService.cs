namespace EntreLaunch.Interfaces.MyPartnerIntf
{
    public interface IMyPartnerProjectService
    {
        /// <summary>
        /// create nwe project.
        /// </summary>>
        Task<GeneralResult> CreateProjectWithAttachments(MyPartnerCreateDto createDto);

        /// <summary>
        /// EntreLaunchdate project.
        /// </summary>>
        Task<GeneralResult> EntreLaunchdateProject(int id, MyPartnerEntreLaunchdateDto EntreLaunchdateDto);

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
        Task<GeneralResult> PendingProjects();

        /// <summary>
        /// Get all accepted projects.
        /// </summary>
        Task<GeneralResult> AcceptedProjects();

        /// <summary>
        /// Get all rejected projects.
        /// </summary>
        Task<GeneralResult> RejectedProjects();

        /// <summary>
        /// Get all projects.
        /// </summary>
        Task<GeneralResult> AllProjects();
    }
}

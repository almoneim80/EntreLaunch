namespace EntreLaunch.Interfaces.MyTeamIntf
{
    public interface IMyTeamService
    {
        /// <summary>
        /// Create new employee request.
        /// </summary>
        Task<GeneralResult> CreateEmployeeWithPortfolio(EmployeeCreateDto createDto);

        /// <summary>
        /// show all employee request.
        /// </summary>
        Task<GeneralResult> AllEmployeeRequest();

        /// <summary>
        /// process employee request (accept, reject).
        /// </summary>
        Task<GeneralResult> ProcessEmployeeRequestStatus(EmployeeRequestDto employeeRequestDto);

        /// <summary>
        /// show accepted employees.
        /// </summary>
        Task<GeneralResult> AcceptedEmployees();

        /// <summary>
        /// show pending employees.
        /// </summary>
        Task<GeneralResult> PendingEmployees();

        /// <summary>
        /// show rejected employees.
        /// </summary>
        Task<GeneralResult> RejectedEmployees();

        /// <summary>
        /// get projects with portfolio.
        /// </summary>
        Task<GeneralResult> GetEmployeeById(int id);

        /// <summary>
        /// get portfolio for aspecific project.
        /// </summary>
        Task<GeneralResult> GetPortfoliosByEmployeeId(int employeeId);

        /// <summary>
        /// EntreLaunchdate project.
        /// </summary>
        Task<GeneralResult> EntreLaunchdateEmployee(int employeeId, EmployeeEntreLaunchdateDto EntreLaunchdateDto);

        /// <summary>
        /// EntreLaunchdate portfolio.
        /// </summary>
        Task<GeneralResult> EntreLaunchdateEmployeePortfolio(int portfolioId, EmployeePortfolioEntreLaunchdateDto EntreLaunchdateDto);

        /// <summary>
        /// EntreLaunchdatre attachment.
        /// </summary>
        Task<GeneralResult> EntreLaunchdatePortfolioAttachment(int attachmentId, PortfolioAttachmentEntreLaunchdateDto EntreLaunchdateDto);

        /// <summary>
        /// filtering projects.
        /// </summary>
        Task<GeneralResult> FilterAcceptedByWorkField(string workField);
    }
}

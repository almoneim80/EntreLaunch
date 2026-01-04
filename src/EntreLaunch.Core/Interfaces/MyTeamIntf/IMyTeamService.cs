using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.MyTeamDtos;
using EntreLaunch.DTOs.TrainingDtos;

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
        Task<GeneralResult<PaginatedResult<EmployeeDetailsDto>>> AllEmployeeRequest(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// process employee request (accept, reject).
        /// </summary>
        Task<GeneralResult> ProcessEmployeeRequestStatus(EmployeeRequestDto employeeRequestDto);

        /// <summary>
        /// show accepted employees.
        /// </summary>
        Task<GeneralResult<PaginatedResult<EmployeeDetailsDto>>> AcceptedEmployees(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// show pending employees.
        /// </summary>
        Task<GeneralResult<PaginatedResult<EmployeeDetailsDto>>> PendingEmployees(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// show rejected employees.
        /// </summary>
        Task<GeneralResult<PaginatedResult<EmployeeDetailsDto>>> RejectedEmployees(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// get projects with portfolio.
        /// </summary>
        Task<GeneralResult> GetEmployeeById(int id);

        /// <summary>
        /// get portfolio for aspecific project.
        /// </summary>
        Task<GeneralResult> GetPortfoliosByEmployeeId(int employeeId);

        /// <summary>
        /// update project.
        /// </summary>
        Task<GeneralResult> UpdateEmployee(int employeeId, EmployeeUpdateDto updateDto);

        /// <summary>
        /// update portfolio.
        /// </summary>
        Task<GeneralResult> UpdateEmployeePortfolio(int portfolioId, EmployeePortfolioUpdateDto updateDto);

        /// <summary>
        /// updatre attachment.
        /// </summary>
        Task<GeneralResult> UpdatePortfolioAttachment(int attachmentId, PortfolioAttachmentUpdateDto updateDto);

        /// <summary>
        /// filtering projects.
        /// </summary>
        Task<GeneralResult> FilterAcceptedByWorkField(string workField);
    }
}

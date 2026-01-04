using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.CertificateDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.TrainingIntf
{
    public interface ICertificateService
    {
        /// <summary>
        /// Issue certificate.
        /// </summary>
        Task<GeneralResult> IssuePathCertificateAsync(int pathId, string userId);

        /// <summary>
        /// Issue certificate.
        /// </summary>
        Task<GeneralResult> IssueCourseCertificateAsync(int courseId, string userId);

        /// <summary>
        /// Update certificate.
        /// </summary>
        Task<GeneralResult> ShippingCertificateAsync(int id, string shippingAddress, string userId);

        /// <summary>
        /// Get user certificates.
        /// </summary>
        Task<GeneralResult<List<CertificateDetailsDto>>> GetUserCertificatesAsync(string userId);

        /// <summary>
        /// Get all certificates.
        /// </summary>
        Task<GeneralResult<PaginatedResult<CertificateDetailsDto>>> GetAllAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Delete certificate.
        /// </summary>
        Task<GeneralResult> DeleteAsync(int id);

        /// <summary>
        /// Get one certificate.
        /// </summary>
        Task<GeneralResult<CertificateDetailsDto>> GetOneAsync(int certificateId);

        /// <summary>
        /// Get all shipping certificates.
        /// </summary>
        Task<GeneralResult<List<CertificateDetailsDto>>> GetAllShippingCertificatesAsync();

        /// <summary>
        /// Update shipping status.
        /// </summary>
        Task<GeneralResult> UpdateShippingStatusAsync(int certificateId, ShippingStatus newStatus);
    }
}

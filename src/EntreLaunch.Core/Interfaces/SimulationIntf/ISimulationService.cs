using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.SimulationDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.SimulationIntf
{
    public interface ISimulationService
    {
        #region Simulation Management

        /// <summary>
        /// Create a new simulation with all its data at once.
        /// </summary>
        Task<GeneralResult> CreateSimulationAsync(SimulationCreateDto dto);

        /// <summary>
        /// Fetch all simulations regardless of status.
        /// </summary>
        Task<GeneralResult<PaginatedResult<SimulationDetails>>> GetAllSimulationsAsync(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Fetch all simulations by status (Pending, Approved, Rejected).
        /// </summary>
        Task<GeneralResult> GetSimulationsByStatusAsync(SimulationStatus status);

        /// <summary>
        /// Fetch a specific simulation in detail based on the ID.
        /// </summary>
        Task<GeneralResult> GetSimulationByIdAsync(int simulationId);

        /// <summary>
        /// Get all ads for a specific simulation.
        /// </summary>
        Task<GeneralResult> GetSimulationAdsAsync(int simulationId, string userId);

        /// <summary>
        /// Update simulation status (accepted, rejected, pending).
        /// </summary>
        Task<GeneralResult> UpdateSimulationStatusAsync(int simulationId, SimulationStatus newStatus);
        #endregion

        #region ads and guest

        /// <summary>
        /// Register a new guest.
        /// </summary>
        Task<GeneralResult> RegisterGuestAsync(GuestRegisterDto dto);

        /// <summary>
        /// Like a product ad.
        /// </summary>
        Task<GeneralResult> LikeProductAdAsync(int adId, string userId);

        /// <summary>
        /// get ad like count by ad id.
        /// </summary>
        Task<GeneralResult> GetAdLikeCountAsync(int adId);
        #endregion

        /*
        #region Report

        /// <summary>
        /// Generate the final report for a given simulation.
        /// </summary>
        Task<GeneralResult> GenerateFinalReportAsync(int simulationId, string userId);

        #endregion
        */
    }
}

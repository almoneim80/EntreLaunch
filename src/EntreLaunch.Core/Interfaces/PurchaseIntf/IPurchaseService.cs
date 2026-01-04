using EntreLaunch.DTOs.PaymentDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.PurchaseIntf
{
    public interface IPurchaseService
    {
        /// <summary>
        /// Registers a new purchase after successful payment.
        /// </summary>
        /// <param name="dto">The purchase creation data including type, reference, price, etc.</param>
        Task<GeneralResult> CreatePurchaseAsync(PurchaseCreateDto dto);

        /// <summary>
        /// Retrieves all purchases made by a specific user.
        /// Optionally filtered by purchase type.
        /// </summary>
        Task<GeneralResult<List<PurchaseDetailsDto>>> GetUserPurchasesAsync(string userId, PurchaseItemType? type = null);

        /// <summary>
        /// Retrieves a specific purchase record by ID.
        /// </summary>
        Task<GeneralResult<PurchaseDetailsDto>> GetByIdAsync(int purchaseId);

        /// <summary>
        /// Marks a purchase as refunded and records the time.
        /// </summary>
        Task<GeneralResult> RefundPurchaseAsync(int purchaseId, string reason);

        /// <summary>
        /// Returns the number of purchases and total value for a specific item (e.g. course or certificate).
        /// </summary>
        Task<GeneralResult<PurchaseStatsDto>> GetPurchaseStatsAsync(PurchaseItemType itemType, int referenceId);

        /// <summary>
        /// Checks if the user has purchased a specific item.
        /// </summary>
        Task<GeneralResult<bool>> HasUserPurchasedAsync(string userId, PurchaseItemType itemType, int referenceId);
    }
}

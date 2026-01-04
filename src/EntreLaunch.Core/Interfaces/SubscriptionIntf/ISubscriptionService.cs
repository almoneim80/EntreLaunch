using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.PaymentDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.SubscriptionIntf
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Create a new subscription (manual or at checkout).
        /// </summary>
        Task<GeneralResult<SubscriptionDto>> CreateSubscriptionAsync(SubscriptionCreateDto dto);

        /// <summary>
        /// Manually renew an existing subscription (e.g. via a second payment).
        /// </summary>
        Task<GeneralResult<SubscriptionDto>> RenewSubscriptionAsync(int subscriptionId, int paymentId);

        /// <summary>
        /// Cancel an active subscription.
        /// </summary>
        Task<GeneralResult> CancelSubscriptionAsync(int subscriptionId, string reason = "");

        /// <summary>
        /// Query a user's subscription to a particular service.
        /// </summary>
        Task<GeneralResult<SubscriptionDto?>> GetUserSubscriptionAsync(string userId, SubscriptionType type, int referenceId);

        /// <summary>
        /// Query all user subscriptions.
        /// </summary>
        Task<GeneralResult<PaginatedResult<SubscriptionDto>>> GetUserSubscriptionsAsync(string userId, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Enquire about expired, active or canceled subscriptions to follow up on renewal.
        /// </summary>
        Task<GeneralResult<PaginatedResult<SubscriptionDto>>> GetSubscriptionsByStatusAsync(SubscriptionStatus status, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Check whether a user has current access to a particular subscription.
        /// </summary>
        Task<GeneralResult<bool>> HasActiveAccessAsync(string userId, SubscriptionType type, int? referenceId = null);

        /// <summary>
        /// Query subscriptions that are about to expire (for reminder or notification).
        /// </summary>
        Task<List<SubscriptionDto>> GetExpiringSoonAsync(TimeSpan within);

        /// <summary>
        /// Subscription upgrade (e.g. from Qualifying to Enabled).
        /// </summary>
        Task<GeneralResult<SubscriptionDto>> UpgradeSubscriptionAsync(int currentSubscriptionId, int newReferenceId, decimal additionalPrice);

        /// <summary>
        /// Subscription extension (e.g. tech support gave the user an extra 3 days).
        /// </summary>
        Task<GeneralResult> ExtendSubscriptionAsync(int subscriptionId, TimeSpan extraDuration);

        /// <summary>
        /// Record an automatic end-of-period cancellation (e.g. by Job).
        /// </summary>
        Task MarkAsExpiredAsync();

        /// <summary>
        /// Subscription-based payment case management.
        /// </summary>
        Task<GeneralResult> LinkPaymentToSubscriptionAsync(int subscriptionId, int paymentId);

        /// <summary>
        /// Trial Subscription Support.
        /// </summary>
        Task<GeneralResult> StartTrialSubscriptionAsync(string userId, SubscriptionType type, int referenceId);

        /// <summary>
        /// Bulk subscription management (e.g. an organization with 5 employees).
        /// </summary>
        Task<GeneralResult> CreateChildSubscriptionAsync(int parentSubscriptionId, string childUserId);

        /// <summary>
        /// View general statistics on subscriptions (for the admin panel).
        /// </summary>
        Task<SubscriptionStatsDto> GetSubscriptionStatisticsAsync(DateTimeOffset? fromDate = null);
    }
}

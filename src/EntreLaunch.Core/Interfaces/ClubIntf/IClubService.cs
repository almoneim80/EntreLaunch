namespace EntreLaunch.Interfaces.ClubIntf
{
    public interface IClubService
    {
        /// <summary>
        /// Allows a user to subscribe to a club.
        /// </summary>
        Task<GeneralResult> SubscribeToClubAsync(string userId);

        /// <summary>
        /// renews a user's subscription to the club.
        /// </summary>
        Task<GeneralResult> RenewClubSubscriptionAsync(string userId);

        /// <summary>
        /// Allows a user to subscribe to a specific event.
        /// </summary>
        Task<GeneralResult> SubscribeToEventAsync(ClubSubscribeCreateDto dto);

        /// <summary>
        /// Allows a user to unsubscribe from a specific event.
        /// </summary>
        Task<GeneralResult> UnsubscribeFromEventAsync(int subscriptionId, string userId);

        /// <summary>
        /// Retrieves all subscribers for a given event.
        /// </summary>
        Task<GeneralResult> GetEventSubscribersAsync(int eventId);

        /// <summary>
        /// Retrieves all events that a specific user has subscribed to.
        /// </summary>
        Task<GeneralResult> GetUserSubscriptionsAsync(string userId);

        /// <summary>
        /// Expires all subscriptions to the club.
        /// </summary>
        Task<GeneralResult> ExpireClubSubscriptionsAsync();
    }
}

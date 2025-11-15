namespace EntreLaunch.Services.ClubSvc
{
    public class ClubService : IClubService
    {
        private readonly PgDbContext _dbContext;
        private readonly ILogger<ClubService> _logger;
        public ClubService(PgDbContext dbContext, ILogger<ClubService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<GeneralResult> SubscribeToClubAsync(string userId)
        {
            try
            {
                // check if user exists
                var userExists = await _dbContext.Users
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (!userExists)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found or deleted",
                        Data = null
                    };
                }

                // check if user already has an active club subscription
                var existingClubSubscription = await _dbContext.ClubSubscribers
                    .FirstOrDefaultAsync(s => s.UserId == userId
                                              && s.SubscribeFor == SubscribeType.Club
                                              && !s.IsDeleted
                                              && s.IsActive
                                              && s.SubscriptionEnd > DateTimeOffset.UtcNow);

                if (existingClubSubscription != null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User already has an active club subscription.",
                        Data = null
                    };
                }

                // add club subscription
                var subscriptionStart = DateTimeOffset.UtcNow;
                var subscriptionEnd = subscriptionStart.AddMonths(1);

                var newSubscription = new ClubSubscriber
                {
                    UserId = userId,
                    SubscriptionDate = subscriptionStart,
                    SubscriptionEnd = subscriptionEnd,
                    SubscribeFor = SubscribeType.Club,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _dbContext.ClubSubscribers.Add(newSubscription);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Successfully subscribed to the club for 1 month (120 SAR).",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing user {UserId} to club", userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = $"Failed to subscribe to club",
                    Data = ex
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> RenewClubSubscriptionAsync(string userId)
        {
            try
            {
                // get last subscription of the user
                var lastSub = await _dbContext.ClubSubscribers
                    .Where(s => s.UserId == userId
                             && s.SubscribeFor == SubscribeType.Club
                             && !s.IsDeleted &&
                             s.IsActive)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefaultAsync();

                if (lastSub == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No previous club subscription found. User must subscribe first.",
                        Data = null
                    };
                }

                // renew the subscription by 1 month 
                var now = DateTimeOffset.UtcNow;
                var oldEnd = lastSub.SubscriptionEnd ?? now;
                var nextStart = oldEnd > now ? oldEnd : now;
                var nextEnd = nextStart.AddMonths(1);

                // EntreLaunchdate the last subscription
                var newSubscription = new ClubSubscriber
                {
                    UserId = userId,
                    SubscriptionDate = nextStart,
                    SubscriptionEnd = nextEnd,
                    SubscribeFor = SubscribeType.Club,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _dbContext.ClubSubscribers.Add(newSubscription);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = $"Successfully renewed the club subscription from {nextStart} to {nextEnd}",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing club subscription for user {UserId}", userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = $"Failed to renew club subscription",
                    Data = ex.ToString()
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> SubscribeToEventAsync(ClubSubscribeCreateDto dto)
        {
            try
            {
                var userExists = await _dbContext.Users
                    .AnyAsync(u => u.Id == dto.UserId && !u.IsDeleted);
                if (!userExists)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found or deleted",
                        Data = null
                    };
                }

                var hasActiveClub = await _dbContext.ClubSubscribers
                    .AnyAsync(s => s.UserId == dto.UserId
                      && s.SubscribeFor == SubscribeType.Club
                      && !s.IsDeleted
                      && s.SubscriptionEnd > DateTimeOffset.UtcNow && s.IsActive);

                if (!hasActiveClub)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User must have an active club subscription to join an event.",
                        Data = null
                    };
                }

                var clubEvent = await _dbContext.ClubEvents
                    .FirstOrDefaultAsync(e => e.Id == dto.EventId && !e.IsDeleted);
                if (clubEvent == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Event not found or deleted",
                        Data = null
                    };
                }

                var existingSubscription = await _dbContext.ClubSubscribers
                    .FirstOrDefaultAsync(
                    s => s.UserId == dto.UserId &&
                    s.EventId == dto.EventId &&
                    !s.IsDeleted &&
                    s.SubscribeFor == SubscribeType.Event &&
                    s.IsActive);

                if (existingSubscription != null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User already subscribed to this event.",
                        Data = null
                    };
                }

                var subscription = new ClubSubscriber
                {
                    UserId = dto.UserId,
                    EventId = dto.EventId,
                    SubscriptionDate = DateTimeOffset.UtcNow,
                    SubscriptionEnd = null,
                    SubscribeFor = SubscribeType.Event,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                };

                _dbContext.ClubSubscribers.Add(subscription);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Successfully subscribed.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to event {EventId} by user {UserId}", dto.EventId, dto.UserId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to subscribe to event",
                    Data = ex
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UnsubscribeFromEventAsync(int subscriptionId, string userId)
        {
            try
            {
                var subscription = await _dbContext.ClubSubscribers
                    .FirstOrDefaultAsync(s => s.Id == subscriptionId && !s.IsDeleted && s.IsActive);
                if (subscription == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Subscription not found or already deleted.",
                        Data = null
                    };
                }

                if (!string.Equals(subscription.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Not authorized to unsubscribe another user.",
                        Data = null
                    };
                }

                subscription.IsDeleted = true;
                subscription.IsActive = false;
                subscription.DeletedAt = DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "Successfully unsubscribed",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing from event. SubscriptionId={SubscriptionId}, userId={UserId}", subscriptionId, userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to unsubscribe from event",
                    Data = ex
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetEventSubscribersAsync(int eventId)
        {
            try
            {
                var clubEvent = await _dbContext.ClubEvents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted);

                if (clubEvent == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "Event not found or deleted",
                        Data = null
                    };
                }

                var subscribers = await _dbContext.ClubSubscribers
                    .Where(s => s.EventId == eventId && !s.IsDeleted && s.IsActive)
                    .Select(s => new ClubEventSubscribeDetailsDto
                    {
                        Id = s.Id,
                        SubscriberData = new UserData
                        {
                            FirstName = s.User.FirstName,
                            LastName = s.User.LastName,
                            Email = s.User.Email
                        },
                        EventName = s.Event!.Name,
                        EventCity = s.Event.City
                    }).ToListAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = $"Found {subscribers.Count} subscribers.",
                    Data = subscribers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscribers for event {EventId}", eventId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get event subscribers",
                    Data = ex
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetUserSubscriptionsAsync(string userId)
        {
            try
            {
                var userExists = await _dbContext.Users
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (!userExists)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "User not found or deleted",
                        Data = null
                    };
                }

                var subscriptions = await _dbContext.ClubSubscribers
                    .Where(s => s.UserId == userId && !s.IsDeleted && s.IsActive)
                    .Include(s => s.Event)
                    .Select(s => new
                    {
                        s.Id,
                        s.EventId,
                        s.SubscriptionDate,
                        EventName = s.Event!.Name,
                        s.Event.City,
                        s.Event.StartDate,
                        s.Event.EndDate
                    }).ToListAsync();

                if (!subscriptions.Any())
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = "No subscriptions found for this user",
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = "User subscriptions retrieved successfully",
                    Data = subscriptions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions for user {UserId}", userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to get user subscriptions",
                    Data = ex
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ExpireClubSubscriptionsAsync()
        {
            try
            {
                var now = DateTimeOffset.UtcNow;

                // fetch all active subscriptions that are due to expire
                var subscriptions = await _dbContext.ClubSubscribers
                    .Where(s => s.SubscribeFor == SubscribeType.Club
                             && s.IsActive
                             && !s.IsDeleted
                             && s.SubscriptionEnd.HasValue
                             && s.SubscriptionEnd.Value <= now).ToListAsync();

                if (!subscriptions.Any())
                {
                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = "No subscriptions to expire.",
                        Data = null
                    };
                }

                // deactivate the subscriptions
                foreach (var sub in subscriptions)
                {
                    sub.IsActive = false;
                    sub.EntreLaunchdatedAt = DateTimeOffset.UtcNow;
                }

                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = $"Expired {subscriptions.Count} club subscriptions.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error expiring club subscriptions.");
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = "Failed to expire club subscriptions.",
                    Data = ex
                };
            }
        }
    }
}

using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ClubDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;

namespace EntreLaunch.Services.ClubSvc
{
    public class ClubService(PgDbContext dbContext, ILogger<ClubService> logger, ILocalizationManager localizationManager) : IClubService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<ClubService> _logger = logger;
        private readonly ILocalizationManager _localizationManager = localizationManager;

        /// <inheritdoc />
        public async Task<GeneralResult> RegisterToEventAsync(ClubEventRegistrationCreateDto dto)
        {
            try
            {
                // check if user exists.
                var userExists = await _dbContext.Users
                    .AnyAsync(u => u.Id == dto.UserId && !u.IsDeleted && u.IsActive);
                if (!userExists)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                // check if user has active club subscription.
                var hasActiveClubSubscription = await _dbContext.Subscriptions
                    .AnyAsync(s => s.UserId == dto.UserId
                      && s.Type == SubscriptionType.Club
                      && !s.IsDeleted
                      && s.EndDate > DateTimeOffset.UtcNow
                      && s.Status == SubscriptionStatus.Active);

                if (!hasActiveClubSubscription)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserMustHaveClubSubscription"),
                        Data = null
                    };
                }

                // check if event exists.
                var clubEvent = await _dbContext.ClubEvents
                    .FirstOrDefaultAsync(e => e.Id == dto.EventId && !e.IsDeleted);
                if (clubEvent == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("EventNotFoundOrDeleted"),
                        Data = null
                    };
                }

                // check if user is already registered to event
                var alreadyRegistered = await _dbContext.ClubEventRegistrations
                    .AnyAsync(
                    r => r.UserId == dto.UserId
                    && r.EventId == dto.EventId
                    && !r.IsDeleted
                    && !r.IsCancelled);

                if (alreadyRegistered)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserAlreadySubscribedToEvent"),
                        Data = null
                    };
                }

                var registration = new ClubEventRegistration
                {
                    UserId = dto.UserId,
                    EventId = dto.EventId,
                    Notes = dto.Notes,
                    RegisteredAt = dto.RegisteredAt,
                    CreatedAt = dto.CreatedAt
                };

                _dbContext.ClubEventRegistrations.Add(registration);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("EventSubscriptionSuccess"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to event {EventId} by user {UserId}", dto.EventId, dto.UserId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("EventSubscriptionFailed"),
                    Data = ex
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CancelEventRegistrationAsync(int registrationId, string userId)
        {
            try
            {
                // check if event registration exists
                var registration = await _dbContext.ClubEventRegistrations
                    .FirstOrDefaultAsync(r => r.Id == registrationId && !r.IsDeleted && !r.IsCancelled);
                if (registration == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("RegistrationNotFoundOrDeleted"),
                        Data = null
                    };
                }

                // check if user is authorized to cancel registration
                if (!string.Equals(registration.UserId, userId, StringComparison.OrdinalIgnoreCase))
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UnauthorizedToRegistration"),
                        Data = null
                    };
                }

                registration.IsCancelled = true;
                registration.CancelledAt = DateTimeOffset.UtcNow;
                registration.UpdatedAt = DateTimeOffset.UtcNow;
                await _dbContext.SaveChangesAsync();
                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("CancelRegistrationSuccess"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing from event. SubscriptionId={SubscriptionId}, userId={UserId}", registrationId, userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("CancelRegistrationFailed"),
                    Data = ex
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<ClubEventRegistrationDetailsDto>>> GetEventRegistrationsAsync(int eventId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the event exists
                var clubEvent = await _dbContext.ClubEvents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted);

                if (clubEvent == null)
                {
                    return new GeneralResult<PaginatedResult<ClubEventRegistrationDetailsDto>>(
                        false,
                        _localizationManager.GetLocalizedString("EventNotFoundOrDeleted"),
                        null);
                }

                // Get the registrations and apply pagination
                var query = _dbContext.ClubEventRegistrations
                    .Where(r => r.EventId == eventId && !r.IsDeleted && !r.IsCancelled)
                    .Include(r => r.User)
                    .Include(r => r.Event)
                    .OrderByDescending(r => r.RegisteredAt)
                    .Select(r => new ClubEventRegistrationDetailsDto
                    {
                        RegisteredAt = r.RegisteredAt,
                        IsCancelled = r.IsCancelled,
                        CancelledAt = r.CancelledAt,
                        Notes = r.Notes ?? string.Empty,
                        UserData = new UserData
                        {
                            Id = r.User.Id,
                            FirstName = r.User.FirstName ?? string.Empty,
                            LastName = r.User.LastName ?? string.Empty,
                            Email = r.User.Email ?? string.Empty
                        },
                        clubEventDetailsDto = new ClubEventDetails
                        {
                            Id = clubEvent.Id,
                            Name = clubEvent.Name ?? string.Empty,
                            City = clubEvent.City ?? string.Empty,
                            Description = clubEvent.Description ?? string.Empty,
                            StartDate = clubEvent.StartDate,
                            EndDate = clubEvent.EndDate,
                            EventSubscribers = new List<UserData>() // Will be populated later if needed
                        }
                    });

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);
                return new GeneralResult<PaginatedResult<ClubEventRegistrationDetailsDto>>(
                    true,
                    _localizationManager.GetLocalizedString("SubscribersFound"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscribers for event {EventId}", eventId);
                return new GeneralResult<PaginatedResult<ClubEventRegistrationDetailsDto>>(
                    false,
                    _localizationManager.GetLocalizedString("EventSubscribersFetchFailed"),
                    null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> GetUserEventRegistrationsAsync(string userId)
        {
            try
            {
                // check if user exists.
                var userExists = await _dbContext.Users
                    .AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (!userExists)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("UserNotFound"),
                        Data = null
                    };
                }

                // check if user has any registrations
                var registrations = await _dbContext.ClubEventRegistrations
                    .Where(r => r.UserId == userId && !r.IsDeleted)
                    .Include(r => r.Event)
                    .ToListAsync();

                if (!registrations.Any())
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localizationManager.GetLocalizedString("NoSubscriptionsFoundForUser"),
                        Data = null
                    };
                }

                var result = registrations.Select(r => new ClubEventRegistrationDetailsDto
                {
                    RegisteredAt = r.RegisteredAt,
                    IsCancelled = r.IsCancelled,
                    CancelledAt = r.CancelledAt,
                    Notes = r.Notes ?? "",
                    UserData = new UserData
                    {
                        Id = userId,
                        FirstName = r.User?.FirstName ?? "",
                        LastName = r.User?.LastName ?? "",
                        Email = r.User?.Email ?? ""
                    },
                    clubEventDetailsDto = new ClubEventDetails
                    {
                        Id = r.Event.Id,
                        Name = r.Event.Name ?? "",
                        City = r.Event.City ?? "",
                        Description = r.Event.Description ?? "",
                        StartDate = r.Event.StartDate,
                        EndDate = r.Event.EndDate,
                        EventSubscribers = []
                    }
                }).ToList();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localizationManager.GetLocalizedString("UserSubscriptionsRetrieved"),
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions for user {UserId}", userId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localizationManager.GetLocalizedString("UserSubscriptionsFetchFailed"),
                    Data = ex
                };
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> AddClubEventAsync(ClubEventCreateDto dto)
        {
            try
            {
                var newEvent = new ClubEvent
                {
                    Name = dto.Name,
                    City = dto.City,
                    Description = dto.Description,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    CreatedAt = dto.CreatedAt
                };

                _dbContext.ClubEvents.Add(newEvent);
                await _dbContext.SaveChangesAsync();

                return new GeneralResult(true, _localizationManager.GetLocalizedString("EventCreatedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating club event");
                return new GeneralResult(false, _localizationManager.GetLocalizedString("EventCreationFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> UpdateClubEventAsync(int eventId, ClubEventUpdateDto dto)
        {
            try
            {
                var clubEvent = await _dbContext.ClubEvents.FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted);
                if (clubEvent == null)
                {
                    return new GeneralResult(false, _localizationManager.GetLocalizedString("EventNotFoundOrDeleted"));
                }

                clubEvent.Name = dto.Name ?? clubEvent.Name;
                clubEvent.City = dto.City ?? clubEvent.City;
                clubEvent.Description = dto.Description ?? clubEvent.Description;
                clubEvent.StartDate = dto.StartDate ?? clubEvent.StartDate;
                clubEvent.EndDate = dto.EndDate ?? clubEvent.EndDate;
                clubEvent.UpdatedAt = dto.UpdatedAt ?? DateTimeOffset.UtcNow.ToUniversalTime();

                await _dbContext.SaveChangesAsync();

                return new GeneralResult(true, _localizationManager.GetLocalizedString("EventUpdatedSuccessfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating club event {EventId}", eventId);
                return new GeneralResult(false, _localizationManager.GetLocalizedString("EventUpdateFailed"));
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<ClubEventDetails>>> AllEventsAsync(PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.ClubEvents
                    .AsNoTracking()
                    .Include(e => e.Registrations!)
                        .ThenInclude(r => r.User)
                    .Where(e => !e.IsDeleted)
                    .OrderByDescending(e => e.StartDate)
                    .Select(e => new ClubEventDetails
                    {
                        Id = e.Id,
                        Name = e.Name!,
                        City = e.City!,
                        Description = e.Description!,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        EventSubscribers = e.Registrations!
                            .Where(r => !r.IsCancelled)
                            .Select(r => new UserData
                            {
                                Id = r.User.Id,
                                FirstName = r.User.FirstName ?? string.Empty,
                                LastName = r.User.LastName ?? string.Empty,
                                Email = r.User.Email ?? string.Empty
                            })
                            .ToList()
                    });

                var paginatedResult = await query.ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<ClubEventDetails>>(
                    true,
                    _localizationManager.GetLocalizedString("AllEventsFetchedSuccessfully"),
                    paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching club events");
                return new GeneralResult<PaginatedResult<ClubEventDetails>>(
                    false,
                    _localizationManager.GetLocalizedString("FailedEventsFetched"),
                    null);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<ClubEventDetails>> OneEventAsync(int eventId, CancellationToken cancellationToken)
        {
            try
            {
                var clubEvent = await _dbContext.ClubEvents
                    .Include(e => e.Registrations!)
                        .ThenInclude(r => r.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted, cancellationToken);

                if (clubEvent == null)
                {
                    return new GeneralResult<ClubEventDetails>(
                        false,
                        _localizationManager.GetLocalizedString("NoEventFound"),
                        null);
                }

                var dto = new ClubEventDetails
                {
                    Id = clubEvent.Id,
                    Name = clubEvent.Name!,
                    City = clubEvent.City!,
                    Description = clubEvent.Description!,
                    StartDate = clubEvent.StartDate,
                    EndDate = clubEvent.EndDate,
                    EventSubscribers = clubEvent.Registrations!
                                .Where(r => !r.IsCancelled)
                                .Select(r => new UserData
                                {
                                    Id = r.User.Id,
                                    FirstName = r.User.FirstName ?? string.Empty,
                                    LastName = r.User.LastName ?? string.Empty,
                                    Email = r.User.Email ?? string.Empty
                                })
                                .ToList()
                };

                return new GeneralResult<ClubEventDetails>(true, _localizationManager.GetLocalizedString("AllEventsFetchedSuccessfully"), dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching blogs");
                return new GeneralResult<ClubEventDetails>(false, _localizationManager.GetLocalizedString("FailedEventsFetched"), null);
            }
        }

        public async Task<GeneralResult<bool>> CanRegisterToEventAsync(int eventId, string userId)
        {
            try
            {
                // check user
                var userExists = await _dbContext.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted && u.IsActive);
                if (!userExists)
                {
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("UserNotFound"), false);
                }

                // check event
                var eventExists = await _dbContext.ClubEvents.AnyAsync(e => e.Id == eventId && !e.IsDeleted);
                if (!eventExists)
                {
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("EventNotFoundOrDeleted"), false);
                }

                // check subscription
                var hasActiveClubSubscription = await _dbContext.Subscriptions.AnyAsync(s =>
                    s.UserId == userId &&
                    s.Type == SubscriptionType.Club &&
                    s.Status == SubscriptionStatus.Active &&
                    !s.IsDeleted &&
                    s.EndDate > DateTimeOffset.UtcNow);
                if (!hasActiveClubSubscription)
                {
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("UserMustHaveClubSubscription"), false);
                }

                // check if already registered
                var alreadyRegistered = await _dbContext.ClubEventRegistrations.AnyAsync(r =>
                    r.UserId == userId &&
                    r.EventId == eventId &&
                    !r.IsDeleted &&
                    !r.IsCancelled);
                if (alreadyRegistered)
                {
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("UserAlreadySubscribedToEvent"), false);
                }

                return new GeneralResult<bool>(true, "User can register to event.", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking registration permission for user {UserId} and event {EventId}", userId, eventId);
                return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("UnexpectedError"), false);
            }
        }
    }
}

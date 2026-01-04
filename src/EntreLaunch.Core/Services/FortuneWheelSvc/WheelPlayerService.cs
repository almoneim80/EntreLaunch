using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.DTOs.WheelDtos;
using EntreLaunch.Interfaces.FortuneWheelIntf;
namespace EntreLaunch.Services.FortuneWheelSvc
{
    public class WheelPlayerService(
        PgDbContext dbContext,
        ILocalizationManager localization,
        ILogger<WheelPlayerService> logger,
        ILoyaltyPointsService loyaltyPointsService,
        IHttpContextHelper httpContextHelper) : IWheelPlayerService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILocalizationManager _localization = localization;
        private readonly ILogger<WheelPlayerService> _logger = logger;

        /// <inheritdoc/>
        public async Task<GeneralResult> CanPlayTodayAsync(string playerId)
        {
            try
            {
                var today = DateHelper.UtcToday;
                var tomorrow = DateHelper.UtcTomorrow;

                // Count number of plays the user made today
                var todayPlays = await _dbContext.WheelPlayers
                    .CountAsync(x =>
                        x.PlayerId == playerId &&
                        x.PlayedAt != null &&
                        x.PlayedAt.Value >= today &&
                        x.PlayedAt.Value < tomorrow);

                // Count total number of valid purchases (no date filter)
                var totalRetries = await _dbContext.Purchases
                    .CountAsync(p =>
                        p.UserId == playerId &&
                        p.ItemType == PurchaseItemType.SpinWheelRetry &&
                        !p.IsDeleted &&
                        !p.IsRefunded);

                // Allow one free play per day + one per purchase
                var allowedPlays = 1 + totalRetries;
                var canPlay = todayPlays < allowedPlays;

                return new GeneralResult
                {
                    IsSuccess = canPlay,
                    Message = canPlay
                        ? _localization.GetLocalizedString("CanPlayToday")
                        : _localization.GetLocalizedString("NoMorePlaysAllowed"),
                    Data = canPlay
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking play permission for user {PlayerId}", playerId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("PlayCheckFailed"),
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> SpinAsync(string playerId, int awardId)
        {
            try
            {
                // check if the player has played today.
                var today = DateHelper.UtcToday;
                await EnsurePlayerSpinStateAsync(playerId);

                var state = await _dbContext.WheelPlayerStates
                    .FirstAsync(x => x.PlayerId == playerId && x.Date == today);
                if (state.HasUsedFreeSpin && !state.AllowPaidSpin)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localization.GetLocalizedString("AlreadyPlayedToday"),
                        Data = null
                    };
                }

                var device = httpContextHelper.UserAgent;
                var ip = httpContextHelper.IpAddressV4;

                var duplicateAccounts = await _dbContext.WheelPlayers
                    .Where(x => x.IpAddress == ip || x.DeviceInfo == device)
                    .Select(x => x.PlayerId)
                    .Distinct()
                    .ToListAsync();

                if (duplicateAccounts.Count > 1)
                {
                    _logger.LogWarning("Suspicious spin: multiple accounts using same device/IP. PlayerId: {PlayerId}, IP: {Ip}, Device: {Device}", playerId, ip, device);
                }

                // check if the award exists
                var existsAward = await _dbContext.WheelAwards.FirstOrDefaultAsync(x => x.Id == awardId && !x.IsDeleted);
                if (existsAward == null)
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localization.GetLocalizedString("NoAwardsAvailable"),
                        Data = null
                    };
                }

                var isFree = !state.HasUsedFreeSpin;
                var entry = new WheelPlayer
                {
                    PlayerId = playerId,
                    AwardId = awardId,
                    PlayedAt = DateHelper.UtcNow,
                    IsFree = isFree,
                    DeviceInfo = httpContextHelper.UserAgent,
                    IpAddress = httpContextHelper.IpAddressV4
                };

                // add the player
                _dbContext.WheelPlayers.Add(entry);

                if (isFree)
                    state.HasUsedFreeSpin = true;
                else
                    state.AllowPaidSpin = false;

                await _dbContext.SaveChangesAsync();

                if (existsAward.Type == AwardType.Points)
                {
                    var result = await loyaltyPointsService.AddBonusPointsAsync(playerId, existsAward.PointsAmount ?? 0, "WheelSpin");
                    if (result.IsSuccess)
                    {
                        return new GeneralResult
                        {
                            IsSuccess = true,
                            Message = _localization.GetLocalizedString("SpinSuccess"),
                            Data = null
                        };
                    }

                    return new GeneralResult
                    {
                        IsSuccess = true,
                        Message = _localization.GetLocalizedString("SpinFailed"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localization.GetLocalizedString("SpinSuccess"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Spin error for user {PlayerId}", playerId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("SpinFailed"),
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> GetPlayerHistoryAsync(string playerId)
        {
            try
            {
                var history = await _dbContext.WheelPlayers
                    .Where(x => x.PlayerId == playerId && !x.IsDeleted)
                    .Include(x => x.Award)
                    .OrderByDescending(x => x.PlayedAt)
                    .Select(x => new
                    {
                        x.AwardId,
                        AwardName = x.Award.Name,
                        x.PlayedAt,
                        x.IsFree,
                        x.DeviceInfo,
                        x.IpAddress
                    }).ToListAsync();

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localization.GetLocalizedString("PlayerHistoryLoaded"),
                    Data = history
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching spin history for {PlayerId}", playerId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("PlayerHistoryFailed"),
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> GetTodaySpinAsync(string playerId)
        {
            try
            {
                var today = DateHelper.UtcToday;
                var tomorrow = DateHelper.UtcTomorrow;

                var spins = await _dbContext.WheelPlayers
                    .Include(x => x.Award)
                    .Where(x =>
                        x.PlayerId == playerId &&
                        x.PlayedAt >= today &&
                        x.PlayedAt < tomorrow &&
                        !x.IsDeleted)
                    .Select(x => new
                    {
                        x.AwardId,
                        AwardName = x.Award.Name,
                        x.PlayedAt,
                        x.IsFree,
                        x.DeviceInfo,
                        x.IpAddress
                    }).ToListAsync();

                if (!spins.Any())
                {
                    return new GeneralResult
                    {
                        IsSuccess = false,
                        Message = _localization.GetLocalizedString("NoSpinToday"),
                        Data = null
                    };
                }

                return new GeneralResult
                {
                    IsSuccess = true,
                    Message = _localization.GetLocalizedString("TodaySpinLoaded"),
                    Data = spins
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching today's spin for {PlayerId}", playerId);
                return new GeneralResult
                {
                    IsSuccess = false,
                    Message = _localization.GetLocalizedString("TodaySpinFailed"),
                    Data = null
                };
            }
        }

        /// <inheritdoc/>
        public async Task EnsurePlayerSpinStateAsync(string playerId)
        {
            var today = DateHelper.UtcToday;

            var state = await _dbContext.WheelPlayerStates
                .FirstOrDefaultAsync(x => x.PlayerId == playerId && x.Date == today);

            if (state == null)
            {
                _dbContext.WheelPlayerStates.Add(new WheelPlayerState
                {
                    PlayerId = playerId,
                    Date = today,
                    HasUsedFreeSpin = false,
                    AllowPaidSpin = false
                });

                await _dbContext.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task ActivatePaidSpinAsync(string playerId)
        {
            var today = DateHelper.UtcToday;
            var state = await _dbContext.WheelPlayerStates
                .FirstOrDefaultAsync(x => x.PlayerId == playerId && x.Date == today);

            if (state != null)
            {
                state.AllowPaidSpin = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> MarkPlayDeliveredAsync(int playId, bool isDelivered)
        {
            var play = await _dbContext.WheelPlayers
                .Include(x => x.Award)
                .FirstOrDefaultAsync(x => x.Id == playId);

            if (play == null)
            {
                _logger.LogWarning("MarkPlayDeliveredAsync: Play not found. ID={PlayId}", playId);
                return new GeneralResult(false, _localization.GetLocalizedString("PlayNotFound"));
            }

            if (play.Award.Type != AwardType.PhysicalItem)
            {
                _logger.LogWarning("MarkPlayDeliveredAsync: Attempted to mark non-physical award as delivered. ID={PlayId}", playId);
                return new GeneralResult(false, _localization.GetLocalizedString("InvalidAwardType"));
            }

            if (play.IsDelivered == isDelivered)
            {
                return new GeneralResult(true, _localization.GetLocalizedString("PlayStatusUnchanged"));
            }

            play.IsDelivered = isDelivered;
            play.UpdatedAt = DateHelper.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("MarkPlayDeliveredAsync: Updated delivery status for PlayId={PlayId} to {Status}", playId, isDelivered);

            return new GeneralResult(true, _localization.GetLocalizedString("PlayStatusUpdated"));
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<PaginatedResult<WheelPlayDto>>> GetPlaysByDeliveryStatusAsync(bool delivered, PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.WheelPlayers
                .Include(x => x.Award)
                .Include(x => x.Player)
                .Where(x => !x.IsDeleted && x.IsDelivered == delivered && x.Award.Type == AwardType.PhysicalItem)
                .OrderByDescending(x => x.PlayedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(x => new WheelPlayDto
                {
                    Id = x.Id,
                    AwardName = x.Award.Name,
                    PlayedAt = x.PlayedAt,
                    IsFree = x.IsFree,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    PlayerData = new PlayerData
                    {
                        FirstName = x.Player.FirstName,
                        LastName = x.Player.LastName,
                        Email = x.Player.Email
                    }
                })
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<WheelPlayDto>
            {
                Items = items,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount
            };

            return new GeneralResult<PaginatedResult<WheelPlayDto>>(
                true,
                _localization.GetLocalizedString("PlaysByDeliveryLoaded"),
                result
            );
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<PaginatedResult<WheelPlayDto>>> GetAllUserPlaysAsync(PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.WheelPlayers
                .Include(x => x.Award)
                .Include(x => x.Player)
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.PlayedAt);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(x => new WheelPlayDto
                {
                    Id = x.Id,
                    AwardName = x.Award.Name,
                    PlayedAt = x.PlayedAt,
                    IsFree = x.IsFree,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    PlayerData = new PlayerData
                    {
                        FirstName = x.Player.FirstName,
                        LastName = x.Player.LastName,
                        Email = x.Player.Email
                    }
                })
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<WheelPlayDto>
            {
                Items = items,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount
            };

            return new GeneralResult<PaginatedResult<WheelPlayDto>>(
                true,
                _localization.GetLocalizedString("AllPlaysLoaded"),
                result
            );
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<PaginatedResult<WheelPlayDto>>> GetPhysicalItemPlaysByDeliveryStatusAsync(bool? isDelivered, PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _dbContext.WheelPlayers
                    .Include(x => x.Award)
                    .Include(x => x.Player)
                    .Where(x => !x.IsDeleted && x.Award.Type == AwardType.PhysicalItem);

                if (isDelivered.HasValue)
                {
                    query = query.Where(x => x.IsDelivered == isDelivered);
                }

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .OrderByDescending(x => x.PlayedAt)
                    .Skip((pagination.Page - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .Select(x => new WheelPlayDto
                    {
                        Id = x.Id,
                        AwardName = x.Award.Name,
                        PlayedAt = x.PlayedAt,
                        IsFree = x.IsFree,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                        PlayerData = new PlayerData
                        {
                            FirstName = x.Player.FirstName,
                            LastName = x.Player.LastName,
                            Email = x.Player.Email
                        }
                    })
                    .ToListAsync(cancellationToken);

                var result = new PaginatedResult<WheelPlayDto>
                {
                    Items = items,
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalCount = totalCount
                };

                return new GeneralResult<PaginatedResult<WheelPlayDto>>(
                    true,
                    _localization.GetLocalizedString("PhysicalItemPlaysLoaded"),
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving physical item plays with delivery filter: {IsDelivered}", isDelivered);
                return new GeneralResult<PaginatedResult<WheelPlayDto>>(
                    false,
                    _localization.GetLocalizedString("PhysicalItemPlaysLoadFailed"),
                    null!,
                    ErrorType.InternalServerError
                );
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> UpdatePhysicalItemDeliveryStatusAsync(int playId, bool isDelivered, CancellationToken cancellationToken = default)
        {
            try
            {
                var play = await _dbContext.WheelPlayers
                    .Include(x => x.Award)
                    .FirstOrDefaultAsync(x => x.Id == playId, cancellationToken);

                if (play == null)
                {
                    _logger.LogWarning("UpdatePhysicalItemDeliveryStatusAsync: Play not found. ID={PlayId}", playId);
                    return new GeneralResult(false, _localization.GetLocalizedString("PlayNotFound"), ErrorType.NotFound);
                }

                if (play.Award.Type != AwardType.PhysicalItem)
                {
                    _logger.LogWarning("UpdatePhysicalItemDeliveryStatusAsync: Invalid award type. ID={PlayId}", playId);
                    return new GeneralResult(false, _localization.GetLocalizedString("InvalidAwardType"), ErrorType.Validation);
                }

                if (play.IsDelivered == isDelivered)
                {
                    return new GeneralResult(true, _localization.GetLocalizedString("PlayStatusUnchanged"));
                }

                play.IsDelivered = isDelivered;
                play.UpdatedAt = DateHelper.UtcNow;

                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("UpdatePhysicalItemDeliveryStatusAsync: Updated delivery status. PlayId={PlayId}, IsDelivered={IsDelivered}", playId, isDelivered);

                return new GeneralResult(true, _localization.GetLocalizedString("PlayStatusUpdated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdatePhysicalItemDeliveryStatusAsync: Unexpected error. PlayId={PlayId}", playId);
                return new GeneralResult(false, _localization.GetLocalizedString("UpdateDeliveryStatusFailed"), ErrorType.InternalServerError);
            }
        }
    }
}

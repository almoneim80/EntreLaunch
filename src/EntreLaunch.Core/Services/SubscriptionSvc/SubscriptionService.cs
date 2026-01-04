using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.PaymentDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Extensions;
using EntreLaunch.Interfaces.SubscriptionIntf;
namespace EntreLaunch.Services.SubscriptionSvc
{
    public class SubscriptionService(PgDbContext dbContext, ILogger<SubscriptionService> logger,
        ILocalizationManager localizationManager,
        IHttpContextHelper httpContextHelper,
        IRoleService roleService,
        IOptions<SubscriptionSettings> subscriptionSettings) : ISubscriptionService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<SubscriptionService> _logger = logger;
        private readonly IRoleService _roleService = roleService;
        private readonly ILocalizationManager _localizationManager = localizationManager;
        private readonly IHttpContextHelper _httpContextHelper = httpContextHelper;
        private readonly SubscriptionSettings _subscriptionSettings = subscriptionSettings.Value;

        /// <inheritdoc />
        public async Task<GeneralResult<SubscriptionDto>> CreateSubscriptionAsync(SubscriptionCreateDto dto)
        {
            try
            {
                // check if the user already has an active subscription
                var existing = await _dbContext.Subscriptions.FirstOrDefaultAsync(s =>
                    s.UserId == dto.UserId &&
                    s.Type == dto.Type &&
                    s.Status == SubscriptionStatus.Active &&
                    !s.IsDeleted &&
                    ((!s.ReferenceId.HasValue && !dto.ReferenceId.HasValue) ||
                    (s.ReferenceId.HasValue && dto.ReferenceId.HasValue && s.ReferenceId.Value == dto.ReferenceId.Value)));

                if (existing != null)
                {
                    _logger.LogWarning("User {UserId} already has an active subscription of type {Type} for reference {ReferenceId}.",
                        dto.UserId, dto.Type, dto.ReferenceId);

                    return new GeneralResult<SubscriptionDto>(
                        false, _localizationManager.GetLocalizedString("SubscriptionAlreadyActive"),
                        null, ErrorType.InvalidData);
                }

                // check if the payment is valid.
                Payment? payment = null;
                if (dto.PaymentId.HasValue)
                {
                    payment = await _dbContext.Payments.FindAsync(dto.PaymentId.Value);
                    if (payment == null)
                    {
                        _logger.LogWarning("Invalid payment ID: {PaymentId} not found.", dto.PaymentId.Value);
                        return new GeneralResult<SubscriptionDto>(
                            false, _localizationManager.GetLocalizedString("InvalidPayment"),
                            null, ErrorType.InvalidData);
                    }
                }

                // specify the end date.
                var start = DateHelper.UtcNow;
                var end = GetEndDate(dto.Type, start);

                if (dto.TrialPeriodDays.HasValue && dto.TrialPeriodDays > 0)
                {
                    end = start.AddDays(dto.TrialPeriodDays.Value);
                }

                // create the subscription.
                var subscription = new Subscription
                {
                    UserId = dto.UserId,
                    Type = dto.Type,
                    ReferenceId = dto.ReferenceId,
                    StartDate = start,
                    EndDate = end,
                    Price = dto.Price,
                    Status = SubscriptionStatus.Active,
                    IsAutoRenewal = dto.IsAutoRenewal,
                    NextRenewalDate = dto.IsAutoRenewal ? end : null,
                    TrialPeriodDays = dto.TrialPeriodDays,
                    PaymentId = dto.PaymentId ?? 0,
                    Payment = payment!,
                    ByIp = _httpContextHelper.IpAddress,
                    ById = await _httpContextHelper.GetCurrentUserIdAsync(),
                    ByUserAgent = _httpContextHelper.UserAgent,
                    CreatedAt = DateHelper.UtcNow
                };

                await _dbContext.Subscriptions.AddAsync(subscription);

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Subscription created successfully for user {UserId} with type {Type} and ref {ReferenceId}.",
                    dto.UserId, dto.Type, dto.ReferenceId);

                // TODO: send email to user.
                return new GeneralResult<SubscriptionDto>(
                    true, _localizationManager.GetLocalizedString("SubscriptionCreated"),
                    null, ErrorType.Success );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription for user {UserId} with type {Type} and reference {ReferenceId}.",
                    dto.UserId, dto.Type, dto.ReferenceId);

                return new GeneralResult<SubscriptionDto>(
                    false, _localizationManager.GetLocalizedString("UnexpectedError"),
                    null, ErrorType.InternalServerError );
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<SubscriptionDto>> RenewSubscriptionAsync(int subscriptionId, int paymentId)
        {
            try
            {
                // check if the subscription exists.
                var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s =>
                    s.Id == subscriptionId && !s.IsDeleted);

                if (subscription == null)
                {
                    _logger.LogWarning("Cannot renew subscription. Subscription ID {SubscriptionId} not found.", subscriptionId);
                    return new GeneralResult<SubscriptionDto>(
                        false, _localizationManager.GetLocalizedString("SubscriptionNotFound"),
                        null, ErrorType.NotFound);
                }

                // check if the subscription is open-ended.
                if (!subscription.EndDate.HasValue)
                {
                    _logger.LogWarning("Subscription ID {SubscriptionId} has no end date, cannot renew open-ended subscription.", subscriptionId);
                    return new GeneralResult<SubscriptionDto>(
                        false, _localizationManager.GetLocalizedString("SubscriptionCannotBeRenewed"),
                        null, ErrorType.InvalidData);
                }

                // check if the payment is valid.
                var payment = await _dbContext.Payments.FindAsync(paymentId);
                if (payment == null)
                {
                    _logger.LogWarning("Invalid payment ID: {PaymentId} not found for subscription renewal.", paymentId);
                    return new GeneralResult<SubscriptionDto>(
                        false, _localizationManager.GetLocalizedString("InvalidPayment"),
                        null, ErrorType.InvalidData);
                }

                var now = DateHelper.UtcNow;
                var currentEnd = subscription.EndDate.Value;
                var duration = currentEnd - subscription.StartDate;

                // If the subscription is still active, start from the old expiration date.
                // If it's over, start now.
                var newStart = currentEnd > now ? currentEnd : now;
                var newEnd = newStart.Add(duration);

                subscription.StartDate = newStart;
                subscription.EndDate = newEnd;
                subscription.NextRenewalDate = subscription.IsAutoRenewal ? newEnd : null;
                subscription.UpdatedAt = now;
                subscription.RenewalCount += 1;
                subscription.Status = SubscriptionStatus.Active;
                subscription.PaymentId = paymentId;
                subscription.Payment = payment;

                subscription.ByIp = _httpContextHelper.IpAddress;
                subscription.ByUserAgent = _httpContextHelper.UserAgent;
                subscription.ById = await _httpContextHelper.GetCurrentUserIdAsync();

                await _dbContext.SaveChangesAsync();

                // TODO: send email to user.
                _logger.LogInformation("Subscription ID {SubscriptionId} successfully renewed. New end: {EndDate}.", subscriptionId, newEnd);
                return new GeneralResult<SubscriptionDto>(
                    true, _localizationManager.GetLocalizedString("SubscriptionRenewed"),
                    null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing subscription ID {SubscriptionId}.", subscriptionId);
                return new GeneralResult<SubscriptionDto>(
                    false, _localizationManager.GetLocalizedString("UnexpectedError"),
                    null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CancelSubscriptionAsync(int subscriptionId, string reason = "")
        {
            try
            {
                // check if the subscription exists.
                var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s =>
                    s.Id == subscriptionId && !s.IsDeleted);

                if (subscription == null)
                {
                    _logger.LogWarning("Attempted to cancel a non-existent subscription with ID {SubscriptionId}.", subscriptionId);
                    return new GeneralResult(
                        false, _localizationManager.GetLocalizedString("SubscriptionNotFound"),
                        null, ErrorType.NotFound);
                }

                // check if the subscription is active or cancelled.
                if (subscription.Status != SubscriptionStatus.Active)
                {
                    _logger.LogWarning("Cannot cancel subscription ID {SubscriptionId} because its status is {Status}.",
                        subscriptionId, subscription.Status);
                    return new GeneralResult(
                        false, _localizationManager.GetLocalizedString("SubscriptionNotActive"),
                        null, ErrorType.InvalidData);
                }

                subscription.Status = SubscriptionStatus.Cancelled;
                subscription.UpdatedAt = DateHelper.UtcNow;

                // update the user IP and device information.
                subscription.ByIp = _httpContextHelper.IpAddress;
                subscription.ByUserAgent = _httpContextHelper.UserAgent;
                subscription.ById = await _httpContextHelper.GetCurrentUserIdAsync();

                // add the cancellation reason.
                if (!string.IsNullOrWhiteSpace(reason))
                {
                    var metadata = string.IsNullOrWhiteSpace(subscription.MetadataJson)
                        ? new Dictionary<string, object>()
                        : JsonSerializer.Deserialize<Dictionary<string, object>>(subscription.MetadataJson!) ?? new();

                    metadata["cancellationReason"] = reason;
                    metadata["cancelledBy"] = subscription.ById ?? string.Empty;
                    subscription.MetadataJson = JsonSerializer.Serialize(metadata);
                }

                if (subscription.Type == SubscriptionType.TrainingPath)
                {
                    var hasOtherActive = await _dbContext.Subscriptions
                    .AnyAsync(s =>
                            s.UserId == subscription.UserId &&
                            s.Type == SubscriptionType.TrainingPath &&
                            s.Status == SubscriptionStatus.Active &&
                            !s.IsDeleted);

                    if (!hasOtherActive)
                    {
                        await _roleService.RemoveRoleAsync(subscription.UserId, AppRoles.Student);
                        _logger.LogInformation("Removed Student role from user {UserId} after last training path subscription ended.", subscription.UserId);
                    }
                }

                await _dbContext.SaveChangesAsync();

                // TODO: send email to user.
                _logger.LogInformation("Subscription ID {SubscriptionId} was cancelled. Reason: {Reason}", subscriptionId, reason);
                return new GeneralResult(
                    true, _localizationManager.GetLocalizedString("SubscriptionCancelled"),
                    null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription ID {SubscriptionId}.", subscriptionId);
                return new GeneralResult(
                    false, _localizationManager.GetLocalizedString("UnexpectedError"),
                    null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<SubscriptionDto?>> GetUserSubscriptionAsync(string userId, SubscriptionType type, int referenceId)
        {
            try
            {
                var subscription = await _dbContext.Subscriptions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(GetSubscriptionFilter(userId, type, referenceId));

                if (subscription == null)
                {
                    _logger.LogInformation("No active subscription found for user {UserId}, type {Type}, reference {ReferenceId}.",
                        userId, type, referenceId);

                    return new GeneralResult<SubscriptionDto?>(
                        false,
                        _localizationManager.GetLocalizedString("SubscriptionNotFound"),
                        null,
                        ErrorType.NotFound);
                }

                var dto = new SubscriptionDto
                {
                    Id = subscription.Id,
                    Type = subscription.Type,
                    ReferenceId = subscription.ReferenceId ?? -1,
                    ReferenceName = await GetReferenceNameAsync(subscription), // get the reference name.
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate,
                    IsAutoRenewal = subscription.IsAutoRenewal,
                    Status = subscription.Status,
                    Price = subscription.Price
                };

                return new GeneralResult<SubscriptionDto?>(
                    true, _localizationManager.GetLocalizedString("SubscriptionRetrieved"),
                    dto, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription for user {UserId}, type {Type}, ref {ReferenceId}.",
                    userId, type, referenceId);

                return new GeneralResult<SubscriptionDto?>(
                    false, _localizationManager.GetLocalizedString("UnexpectedError"),
                    null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<SubscriptionDto>>> GetUserSubscriptionsAsync(string userId, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                var query = _dbContext.Subscriptions
                    .AsNoTracking()
                    .Where(s => s.UserId == userId && !s.IsDeleted)
                    .OrderByDescending(s => s.CreatedAt);

                if (!await query.AnyAsync(cancellationToken))
                {
                    _logger.LogInformation("No subscriptions found for user {UserId}.", userId);
                    return new GeneralResult<PaginatedResult<SubscriptionDto>>(
                        false, _localizationManager.GetLocalizedString("NoSubscriptionsFound"), null, ErrorType.NotFound);
                }

                var trainingPathIds = await query
                    .Where(s => s.Type == SubscriptionType.TrainingPath && s.ReferenceId.HasValue)
                    .Select(s => s.ReferenceId!.Value)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var referenceNames = await _dbContext.TrainingPaths
                    .Where(t => trainingPathIds.Contains(t.Id) && !t.IsDeleted)
                    .ToDictionaryAsync(t => t.Id, t => t.Name ?? "", cancellationToken);

                var pagedResult = await query
                    .Select(sub => new SubscriptionDto
                    {
                        Id = sub.Id,
                        Type = sub.Type,
                        ReferenceId = sub.ReferenceId ?? -1,
                        ReferenceName = GetReferenceName(sub, referenceNames),
                        StartDate = sub.StartDate,
                        EndDate = sub.EndDate,
                        IsAutoRenewal = sub.IsAutoRenewal,
                        Status = sub.Status,
                        Price = sub.Price
                    })
                    .ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<SubscriptionDto>>(
                    true, _localizationManager.GetLocalizedString("SubscriptionsRetrieved"), pagedResult, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscriptions for user {UserId}.", userId);
                return new GeneralResult<PaginatedResult<SubscriptionDto>>(
                    false, _localizationManager.GetLocalizedString("UnexpectedError"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<PaginatedResult<SubscriptionDto>>> GetSubscriptionsByStatusAsync(SubscriptionStatus status, PaginationParams pagination, CancellationToken cancellationToken)
        {
            try
            {
                // get the subscriptions.
                var query = _dbContext.Subscriptions
                    .AsNoTracking()
                    .Where(s => s.Status == status && !s.IsDeleted)
                    .OrderByDescending(s => s.CreatedAt);

                if (!await query.AnyAsync(cancellationToken))
                {
                    _logger.LogInformation("No subscriptions found with status {Status}.", status);
                    return new GeneralResult<PaginatedResult<SubscriptionDto>>(
                        false, _localizationManager.GetLocalizedString("NoSubscriptionsFoundByStatus"), null, ErrorType.NotFound);
                }

                var trainingPathIds = await query
                    .Where(s => s.Type == SubscriptionType.TrainingPath && s.ReferenceId.HasValue)
                    .Select(s => s.ReferenceId!.Value)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var referenceNames = await _dbContext.TrainingPaths
                    .Where(t => trainingPathIds.Contains(t.Id) && !t.IsDeleted)
                    .ToDictionaryAsync(t => t.Id, t => t.Name ?? "", cancellationToken);

                var pagedResult = await query
                    .Select(sub => new SubscriptionDto
                    {
                        Id = sub.Id,
                        Type = sub.Type,
                        ReferenceId = sub.ReferenceId ?? -1,
                        ReferenceName = GetReferenceName(sub, referenceNames),
                        StartDate = sub.StartDate,
                        EndDate = sub.EndDate,
                        IsAutoRenewal = sub.IsAutoRenewal,
                        Status = sub.Status,
                        Price = sub.Price
                    })
                    .ToPagedResultAsync(pagination, cancellationToken);

                return new GeneralResult<PaginatedResult<SubscriptionDto>>(
                    true, _localizationManager.GetLocalizedString("SubscriptionsByStatusRetrieved"), pagedResult, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscriptions by status {Status}.", status);
                return new GeneralResult<PaginatedResult<SubscriptionDto>>(
                    false, _localizationManager.GetLocalizedString("UnexpectedError"), null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult<bool>> HasActiveAccessAsync(string userId, SubscriptionType type, int? referenceId = null)
        {
            var now = DateHelper.UtcNow;

            var query = _dbContext.Subscriptions
                .AsNoTracking()
                .Where(s =>
                    s.UserId == userId &&
                    s.Type == type &&
                    s.Status == SubscriptionStatus.Active &&
                    !s.IsDeleted);

            // check if this is a training path subscription.
            if (type == SubscriptionType.TrainingPath)
            {
                if (!referenceId.HasValue)
                {
                    return new GeneralResult<bool>(false, _localizationManager.GetLocalizedString("ErrorInTrainingPathSubscription"), false);
                }

                query = query.Where(s => s.ReferenceId == referenceId);
            }

            var result = await Task.Run(() =>
                query.AsEnumerable().Any(s => s.EndDate.HasValue && s.EndDate > now));
            if (result)
            {
                    return new GeneralResult<bool>(
                    true,
                    _localizationManager.GetLocalizedString("UserHasActiveAccess"),
                    true,
                    ErrorType.Success);
            }

            return new GeneralResult<bool>(
                false,
                _localizationManager.GetLocalizedString("UserHasNoActiveAccess"),
                false,
                ErrorType.NotFound);
        }

        /// <inheritdoc />
        public async Task<List<SubscriptionDto>> GetExpiringSoonAsync(TimeSpan within)
        {
            var now = DateHelper.UtcNow;
            var targetDate = now.Add(within);

            // get the subscriptions.
            var subscriptions = await _dbContext.Subscriptions
                .AsNoTracking()
                .Where(s =>
                    s.Status == SubscriptionStatus.Active &&
                    s.EndDate.HasValue &&
                    s.EndDate.Value <= targetDate &&
                    s.EndDate.Value > now &&
                    !s.IsDeleted)
                .OrderBy(s => s.EndDate)
                .ToListAsync();

            _logger.LogInformation("Found {Count} subscriptions expiring within {WithinDays} days.", subscriptions.Count, within.TotalDays);

            if (!subscriptions.Any())
                return [];

            var referenceNames = new Dictionary<int, string>();
            var trainingPathIds = subscriptions
                .Where(s => s.Type == SubscriptionType.TrainingPath && s.ReferenceId.HasValue)
                .Select(s => s.ReferenceId!.Value)
                .Distinct()
                .ToList();

            if (trainingPathIds.Any())
            {
                var names = await _dbContext.TrainingPaths
                    .Where(t => trainingPathIds.Contains(t.Id) && !t.IsDeleted)
                    .ToDictionaryAsync(t => t.Id, t => t.Name ?? "");

                foreach (var kv in names)
                    referenceNames[kv.Key] = kv.Value;
            }

            var result = subscriptions.Select(sub => new SubscriptionDto
            {
                Id = sub.Id,
                Type = sub.Type,
                ReferenceId = sub.ReferenceId ?? -1,
                ReferenceName = GetReferenceName(sub, referenceNames),
                StartDate = sub.StartDate,
                EndDate = sub.EndDate,
                IsAutoRenewal = sub.IsAutoRenewal,
                Status = sub.Status,
                Price = sub.Price
            }).ToList();

            return result;
        }

        /// <inheritdoc />
        public async Task<GeneralResult<SubscriptionDto>> UpgradeSubscriptionAsync(int currentSubscriptionId, int newReferenceId, decimal additionalPrice)
        {
            try
            {
                // get the subscription.
                var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s =>
                    s.Id == currentSubscriptionId && !s.IsDeleted);

                if (subscription == null)
                {
                    _logger.LogWarning("Upgrade failed. Subscription ID {Id} not found.", currentSubscriptionId);
                    return new GeneralResult<SubscriptionDto>(
                        false, _localizationManager.GetLocalizedString("SubscriptionNotFound"),
                        null, ErrorType.NotFound);
                }

                // check if the subscription is active.
                if (subscription.Status != SubscriptionStatus.Active)
                {
                    _logger.LogWarning("Cannot upgrade subscription ID {Id} because it is not active.", currentSubscriptionId);
                    return new GeneralResult<SubscriptionDto>(
                        false, _localizationManager.GetLocalizedString("SubscriptionNotActive"),
                        null, ErrorType.InvalidData);
                }

                if (subscription.ReferenceId == newReferenceId)
                {
                    _logger.LogWarning("Subscription ID {Id} is already pointing to reference ID {RefId}.", currentSubscriptionId, newReferenceId);
                    return new GeneralResult<SubscriptionDto>(
                        false, _localizationManager.GetLocalizedString("SubscriptionAlreadyUpgraded"),
                        null, ErrorType.InvalidData);
                }

                if(additionalPrice < 0)
                {
                    _logger.LogWarning("Additional price cannot be negative.");
                    return new GeneralResult<SubscriptionDto>(
                        false, _localizationManager.GetLocalizedString("InvalidAdditionalPrice"),
                        null, ErrorType.InvalidData);
                }

                subscription.ReferenceId = newReferenceId;
                subscription.Price += additionalPrice;
                subscription.UpdatedAt = DateHelper.UtcNow;

                subscription.ById = await _httpContextHelper.GetCurrentUserIdAsync();
                subscription.ByIp = _httpContextHelper.IpAddress;
                subscription.ByUserAgent = _httpContextHelper.UserAgent;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Subscription ID {Id} upgraded to reference ID {RefId}.", currentSubscriptionId, newReferenceId);

                var referenceName = await GetReferenceNameAsync(subscription);
                var dto = new SubscriptionDto
                {
                    Id = subscription.Id,
                    Type = subscription.Type,
                    ReferenceId = subscription.ReferenceId ?? -1,
                    ReferenceName = referenceName,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate,
                    IsAutoRenewal = subscription.IsAutoRenewal,
                    Status = subscription.Status,
                    Price = subscription.Price
                };

                return new GeneralResult<SubscriptionDto>(
                    true, _localizationManager.GetLocalizedString("SubscriptionUpgraded"),
                    dto, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upgrading subscription ID {Id} to reference {RefId}.", currentSubscriptionId, newReferenceId);
                return new GeneralResult<SubscriptionDto>(
                    false, _localizationManager.GetLocalizedString("UnexpectedError"),
                    null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> ExtendSubscriptionAsync(int subscriptionId, TimeSpan extraDuration)
        {
            try
            {
                // get the subscription.
                var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s =>
                    s.Id == subscriptionId && !s.IsDeleted);

                if (subscription == null)
                {
                    _logger.LogWarning("Attempted to extend non-existent subscription ID {SubscriptionId}.", subscriptionId);
                    return new GeneralResult(
                        false, _localizationManager.GetLocalizedString("SubscriptionNotFound"),
                        null, ErrorType.NotFound);
                }

                // check if the subscription is active.
                if (!subscription.EndDate.HasValue)
                {
                    _logger.LogWarning("Cannot extend subscription ID {SubscriptionId} because it has no end date.", subscriptionId);
                    return new GeneralResult(
                        false, _localizationManager.GetLocalizedString("SubscriptionCannotBeExtended"),
                        null, ErrorType.InvalidData);
                }

                subscription.EndDate = subscription.EndDate.Value.Add(extraDuration);
                subscription.NextRenewalDate = subscription.IsAutoRenewal ? subscription.EndDate : null;
                subscription.UpdatedAt = DateHelper.UtcNow;

                subscription.ById = await _httpContextHelper.GetCurrentUserIdAsync();
                subscription.ByIp = _httpContextHelper.IpAddress;
                subscription.ByUserAgent = _httpContextHelper.UserAgent;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Subscription ID {SubscriptionId} was extended by {Days} days.", subscriptionId, extraDuration.TotalDays);
                return new GeneralResult(
                    true, _localizationManager.GetLocalizedString("SubscriptionExtended"),
                    null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extending subscription ID {SubscriptionId}.", subscriptionId);
                return new GeneralResult(
                    false, _localizationManager.GetLocalizedString("UnexpectedError"),
                    null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc />
        public async Task MarkAsExpiredAsync()
        {
            var now = DateHelper.UtcNow;

            var expiredSubscriptions = await _dbContext.Subscriptions
                .Where(s =>
                    s.Status == SubscriptionStatus.Active &&
                    s.EndDate.HasValue &&
                    s.EndDate < now &&
                    !s.IsDeleted)
                .ToListAsync();

            if (!expiredSubscriptions.Any())
            {
                _logger.LogInformation("MarkAsExpiredAsync: No subscriptions to mark as expired.");
                return;
            }

            var userId = await _httpContextHelper.GetCurrentUserIdAsync();
            var ip = _httpContextHelper.IpAddress;
            var agent = _httpContextHelper.UserAgent;

            foreach (var sub in expiredSubscriptions)
            {
                sub.Status = SubscriptionStatus.Expired;
                sub.UpdatedAt = now;
                sub.ById = userId;
                sub.ByIp = ip;
                sub.ByUserAgent = agent;

                if (sub.Type == SubscriptionType.TrainingPath)
                {
                    var hasOtherActive = await _dbContext.Subscriptions
                    .AnyAsync(s =>
                            s.UserId == sub.UserId &&
                            s.Type == SubscriptionType.TrainingPath &&
                            s.Status == SubscriptionStatus.Active &&
                            !s.IsDeleted);

                    if (!hasOtherActive)
                    {
                        await _roleService.RemoveRoleAsync(sub.UserId, AppRoles.Student);
                        _logger.LogInformation("Removed Student role from user {UserId} after last training path subscription ended.", sub.UserId);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            // TODO: send email to user.
            _logger.LogInformation("MarkAsExpiredAsync: {Count} subscriptions marked as expired.", expiredSubscriptions.Count);
        }

        /// <inheritdoc />
        public async Task<GeneralResult> LinkPaymentToSubscriptionAsync(int subscriptionId, int paymentId)
        {
            try
            {
                // get the subscription.
                var subscription = await _dbContext.Subscriptions
                    .Include(s => s.Payment)
                    .FirstOrDefaultAsync(s => s.Id == subscriptionId && !s.IsDeleted);

                if (subscription == null)
                {
                    _logger.LogWarning("Subscription not found. ID = {SubscriptionId}.", subscriptionId);
                    return new GeneralResult(
                        false, _localizationManager.GetLocalizedString("SubscriptionNotFound"),
                        null, ErrorType.NotFound);
                }

                // check if the payment is already linked to the subscription.
                if (subscription.PaymentId == paymentId)
                {
                    _logger.LogInformation("Subscription {SubscriptionId} is already linked to payment {PaymentId}.", subscriptionId, paymentId);
                    return new GeneralResult(
                        false, _localizationManager.GetLocalizedString("PaymentAlreadyLinked"),
                        null, ErrorType.InvalidData);
                }

                // check if the payment exists.
                var payment = await _dbContext.Payments.FindAsync(paymentId);
                if (payment == null)
                {
                    _logger.LogWarning("Invalid payment ID: {PaymentId}.", paymentId);
                    return new GeneralResult(
                        false, _localizationManager.GetLocalizedString("InvalidPayment"),
                        null, ErrorType.InvalidData);
                }

                subscription.PaymentId = paymentId;
                subscription.Payment = payment;
                subscription.UpdatedAt = DateHelper.UtcNow;

                subscription.ById = await _httpContextHelper.GetCurrentUserIdAsync();
                subscription.ByIp = _httpContextHelper.IpAddress;
                subscription.ByUserAgent = _httpContextHelper.UserAgent;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Subscription {SubscriptionId} successfully linked to payment {PaymentId}.", subscriptionId, paymentId);
                return new GeneralResult(
                    true, _localizationManager.GetLocalizedString("PaymentLinkedSuccessfully"),
                    null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking payment {PaymentId} to subscription {SubscriptionId}.", paymentId, subscriptionId);
                return new GeneralResult(
                    false, _localizationManager.GetLocalizedString("UnexpectedError"),
                    null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> StartTrialSubscriptionAsync(string userId, SubscriptionType type, int referenceId)
        {
            try
            {
                // check if the user already has an active subscription.
                var exists = await _dbContext.Subscriptions.AnyAsync(
                    GetSubscriptionFilter(userId, type, referenceId));

                if (exists)
                {
                    _logger.LogInformation("User {UserId} already has an active subscription of type {Type}.", userId, type);
                    return new GeneralResult(
                        false,
                        _localizationManager.GetLocalizedString("SubscriptionAlreadyActive"),
                        null,
                        ErrorType.InvalidData);
                }

                var now = DateHelper.UtcNow;
                var endDate = now.AddDays(_subscriptionSettings.TrialMaxDays);

                var trialSub = new Subscription
                {
                    UserId = userId,
                    Type = type,
                    ReferenceId = referenceId,
                    StartDate = now,
                    EndDate = endDate,
                    Status = SubscriptionStatus.Active,
                    TrialPeriodDays = _subscriptionSettings.TrialMaxDays,
                    IsAutoRenewal = false,
                    Price = 0,
                    ById = await _httpContextHelper.GetCurrentUserIdAsync(),
                    ByIp = _httpContextHelper.IpAddress,
                    ByUserAgent = _httpContextHelper.UserAgent,
                    CreatedAt = now,
                    MetadataJson = JsonSerializer.Serialize(new Dictionary<string, object>
                    {
                        ["isTrial"] = true
                    })
                };

                await _dbContext.Subscriptions.AddAsync(trialSub);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Trial subscription created for user {UserId}, type {Type}, reference {ReferenceId}.", userId, type, referenceId);

                // TODO: send email to user.
                return new GeneralResult(
                    true,
                    _localizationManager.GetLocalizedString("TrialSubscriptionStarted"),
                    null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trial subscription for user {UserId}, type {Type}, reference {ReferenceId}.", userId, type, referenceId);
                return new GeneralResult(
                    false,
                    _localizationManager.GetLocalizedString("UnexpectedError"),
                    null,
                    ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc />
        public async Task<GeneralResult> CreateChildSubscriptionAsync(int parentSubscriptionId, string childUserId)
        {
            try
            {
                // check if the parent subscription exists.
                var parent = await _dbContext.Subscriptions.FirstOrDefaultAsync(s =>
                    s.Id == parentSubscriptionId && !s.IsDeleted);

                if (parent == null)
                {
                    _logger.LogWarning("Parent subscription not found. ID = {ParentId}.", parentSubscriptionId);
                    return new GeneralResult(
                        false, _localizationManager.GetLocalizedString("ParentSubscriptionNotFound"),
                        null, ErrorType.NotFound);
                }

                // check if the parent subscription is active.
                if (parent.Status != SubscriptionStatus.Active)
                {
                    _logger.LogWarning("Parent subscription {Id} is not active.", parentSubscriptionId);
                    return new GeneralResult(
                        false, _localizationManager.GetLocalizedString("ParentSubscriptionNotActive"),
                        null, ErrorType.InvalidData);
                }

                // check if the child user already has an active subscription.
                var hasActive = await _dbContext.Subscriptions.AnyAsync(
                    GetSubscriptionFilter(childUserId, parent.Type, parent.ReferenceId ?? 0));

                if (hasActive)
                {
                    _logger.LogWarning("Child user {UserId} already has an active subscription to the same service.", childUserId);
                    return new GeneralResult(
                        false, _localizationManager.GetLocalizedString("ChildAlreadySubscribed"),
                        null, ErrorType.InvalidData);
                }

                var now = DateHelper.UtcNow;

                var childSub = new Subscription
                {
                    UserId = childUserId,
                    Type = parent.Type,
                    ReferenceId = parent.ReferenceId,
                    StartDate = now,
                    EndDate = parent.EndDate,
                    Status = SubscriptionStatus.Active,
                    IsAutoRenewal = false,
                    TrialPeriodDays = null,
                    Price = 0,
                    IsGifted = true,
                    ParentSubscriptionId = parent.Id,
                    MetadataJson = JsonSerializer.Serialize(new Dictionary<string, object> { ["childOf"] = parent.Id }),
                    CreatedAt = now,
                    ById = await _httpContextHelper.GetCurrentUserIdAsync(),
                    ByIp = _httpContextHelper.IpAddress,
                    ByUserAgent = _httpContextHelper.UserAgent
                };

                await _dbContext.Subscriptions.AddAsync(childSub);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Child subscription created for user {UserId} from parent {ParentId}.", childUserId, parentSubscriptionId);

                return new GeneralResult(
                    true, _localizationManager.GetLocalizedString("ChildSubscriptionCreated"),
                    null, ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating child subscription for user {UserId} from parent {ParentId}.", childUserId, parentSubscriptionId);
                return new GeneralResult(
                    false, _localizationManager.GetLocalizedString("UnexpectedError"),
                    null, ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc />
        public async Task<SubscriptionStatsDto> GetSubscriptionStatisticsAsync(DateTimeOffset? fromDate = null)
        {
            var now = DateHelper.UtcNow;
            var startOfMonth = fromDate ?? new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);

            var query = _dbContext.Subscriptions.AsNoTracking().Where(s => !s.IsDeleted);

            var total = await query.CountAsync();
            var active = await query.CountAsync(s => s.Status == SubscriptionStatus.Active);
            var expired = await query.CountAsync(s => s.Status == SubscriptionStatus.Expired);
            var cancelled = await query.CountAsync(s => s.Status == SubscriptionStatus.Cancelled);
            var trial = await query.CountAsync(s => s.TrialPeriodDays.HasValue && s.TrialPeriodDays > 0);
            var totalRevenue = await query.SumAsync(s => s.Price);
            var monthlyRevenue = await query
                .Where(s => s.CreatedAt >= startOfMonth)
                .SumAsync(s => s.Price);

            var uniqueUsers = await query.Select(s => s.UserId).Distinct().CountAsync();

            var byType = await query
                .GroupBy(s => s.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = new SubscriptionStatsDto
            {
                TotalSubscriptions = total,
                ActiveSubscriptions = active,
                ExpiredSubscriptions = expired,
                CancelledSubscriptions = cancelled,
                TrialSubscriptions = trial,
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                UniqueUsersSubscribed = uniqueUsers,
                SubscriptionsByType = byType.ToDictionary(x => x.Type, x => x.Count)
            };

            _logger.LogInformation("Subscription statistics collected: {@Result}", result);
            return result;
        }

        #region Private methods
        private async Task<string?> GetReferenceNameAsync(Subscription subscription)
        {
            return subscription.Type switch
            {
                SubscriptionType.TrainingPath when subscription.ReferenceId.HasValue => await _dbContext.TrainingPaths
                    .Where(t => t.Id == subscription.ReferenceId.Value && !t.IsDeleted)
                    .Select(t => t.Name ?? "")
                    .FirstOrDefaultAsync(),

                SubscriptionType.MyTeam => _localizationManager.GetLocalizedString("MyTeamSubscription"),
                SubscriptionType.MyFinance => _localizationManager.GetLocalizedString("MyFinanceSubscription"),
                SubscriptionType.MyPartner => _localizationManager.GetLocalizedString("MyPartnerSubscription"),
                SubscriptionType.MyOpportunity => _localizationManager.GetLocalizedString("MyOpportunitySubscription"),
                SubscriptionType.Club => _localizationManager.GetLocalizedString("ClubSubscription"),
                _ => null
            };
        }
        private string? GetReferenceName(Subscription sub, Dictionary<int, string> trainingPathNames)
        {
            return sub.Type switch
            {
                SubscriptionType.TrainingPath => sub.ReferenceId.HasValue && trainingPathNames.ContainsKey(sub.ReferenceId.Value)
                    ? trainingPathNames[sub.ReferenceId.Value]
                    : null,

                SubscriptionType.MyTeam => _localizationManager.GetLocalizedString("MyTeamSubscription"),
                SubscriptionType.MyFinance => _localizationManager.GetLocalizedString("MyFinanceSubscription"),
                SubscriptionType.MyPartner => _localizationManager.GetLocalizedString("MyPartnerSubscription"),
                SubscriptionType.MyOpportunity => _localizationManager.GetLocalizedString("MyOpportunitySubscription"),
                SubscriptionType.Club => _localizationManager.GetLocalizedString("ClubSubscription"),
                _ => null
            };
        }
        private static Expression<Func<Subscription, bool>> GetSubscriptionFilter(string userId, SubscriptionType type, int referenceId)
        {
            return s =>
                s.UserId == userId &&
                s.Type == type &&
                s.Status == SubscriptionStatus.Active &&
                !s.IsDeleted &&
                (
                    type == SubscriptionType.Club ||
                    type == SubscriptionType.MyTeam ||
                    type == SubscriptionType.MyFinance ||
                    type == SubscriptionType.MyPartner ||
                    type == SubscriptionType.MyOpportunity ||
                    (s.ReferenceId.HasValue && s.ReferenceId.Value == referenceId)
                );
        }
        private DateTimeOffset GetEndDate(SubscriptionType type, DateTimeOffset start)
        {
            var end = DateHelper.UtcNow;
            switch (type)
            {
                case SubscriptionType.MyTeam:
                    end = start.AddDays(_subscriptionSettings.MyTeamDurationInDays);
                    break;
                case SubscriptionType.MyFinance:
                    end = start.AddDays(_subscriptionSettings.MyFinanceDurationInDays);
                    break;
                case SubscriptionType.MyPartner:
                    end = start.AddDays(_subscriptionSettings.MyPartnerDurationInDays);
                    break;
                case SubscriptionType.MyOpportunity:
                    end = start.AddDays(_subscriptionSettings.MyOpportunityDurationInDays);
                    break;
                case SubscriptionType.TrainingPath:
                    end = start.AddDays(_subscriptionSettings.TrainingPathDurationInDays);
                    break;
                case SubscriptionType.Club:
                    end = start.AddDays(_subscriptionSettings.ClubDurationInDays);
                    break;
            }

            return end;
        }
        #endregion
    }
}

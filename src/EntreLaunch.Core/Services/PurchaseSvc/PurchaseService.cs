using EntreLaunch.DTOs.PaymentDtos;
using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Interfaces.FortuneWheelIntf;
using EntreLaunch.Interfaces.PurchaseIntf;
using EntreLaunch.Services.FortuneWheelSvc;
namespace EntreLaunch.Services.PurchaseSvc
{
    public class PurchaseService(
        PgDbContext dbContext, ILogger<PurchaseService> logger,
        ILocalizationManager localizationManager,
        IRoleService roleService,
        IWheelPlayerService wheelPlayerService,
        IHttpContextHelper httpContextHelper) : IPurchaseService
    {
        private readonly PgDbContext _dbContext = dbContext;
        private readonly ILogger<PurchaseService> _logger = logger;
        private readonly IRoleService _roleService = roleService;
        private readonly ILocalizationManager _localization = localizationManager;
        private readonly IHttpContextHelper _httpContextHelper = httpContextHelper;
        private readonly IWheelPlayerService _wheelPlayerService = wheelPlayerService;

        /// <inheritdoc/>
        public async Task<GeneralResult> CreatePurchaseAsync(PurchaseCreateDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // check if user exists.
                var userExists = await _dbContext.Users.AnyAsync(u => u.Id == dto.UserId && !u.IsDeleted);
                if (!userExists)
                {
                    _logger.LogWarning("CreatePurchaseAsync: User not found. ID={UserId}", dto.UserId);
                    return new GeneralResult(false, _localization.GetLocalizedString("UserNotFound"));
                }

                // check if payment exists.
                var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == dto.PaymentId && !p.IsDeleted);
                if (payment == null)
                {
                    _logger.LogWarning("CreatePurchaseAsync: Payment not found. ID={PaymentId}", dto.PaymentId);
                    return new GeneralResult(false, _localization.GetLocalizedString("InvalidPayment"));
                }

                var repeatableItemTypes = new[]
                {
                    PurchaseItemType.SpinWheelRetry,
                    PurchaseItemType.OnlineConsultation,
                    PurchaseItemType.TextConsultation
                };

                // check if purchase already exists for user and type and ref id and is not refunded.
                // Only check duplicate for item types that should not be bought more than once per reference
                if (!repeatableItemTypes.Contains(dto.ItemType))
                {
                    var exists = await _dbContext.Purchases.AnyAsync(p =>
                        p.UserId == dto.UserId &&
                        p.ItemType == dto.ItemType &&
                        p.ReferenceId == dto.ReferenceId &&
                        !p.IsRefunded &&
                        !p.IsDeleted);

                    if (exists)
                    {
                        _logger.LogInformation("CreatePurchaseAsync: Purchase already exists for user {UserId}, type {Type}, ref {RefId}.",
                            dto.UserId, dto.ItemType, dto.ReferenceId);

                        return new GeneralResult(false, _localization.GetLocalizedString("PurchaseAlreadyExists"));
                    }
                }

                // create purchase and save it.
                var purchase = new Purchase
                {
                    UserId = dto.UserId,
                    ItemType = dto.ItemType,
                    ReferenceId = dto.ReferenceId,
                    PaymentId = dto.PaymentId,
                    Payment = payment,
                    Price = dto.Price,
                    MetadataJson = dto.MetadataJson,
                    CreatedAt = DateTimeOffset.UtcNow,
                    ById = await _httpContextHelper.GetCurrentUserIdAsync(),
                    ByIp = _httpContextHelper.IpAddress,
                    ByUserAgent = _httpContextHelper.UserAgent
                };

                _dbContext.Purchases.Add(purchase);

                switch (dto.ItemType)
                {
                    case PurchaseItemType.OnlineCourse:
                        if(!(await _roleService.IsUserInRoleAsync(dto.UserId, AppRoles.Student)).Data)
                        {
                            await _roleService.AssignRoleAsync(dto.UserId, AppRoles.Student);
                        }
                        break;

                    case PurchaseItemType.SkillsLibCourse:
                        if (!(await _roleService.IsUserInRoleAsync(dto.UserId, AppRoles.Student)).Data)
                        {
                            await _roleService.AssignRoleAsync(dto.UserId, AppRoles.Student);
                        }
                        break;

                    case PurchaseItemType.SpinWheelRetry:
                        await _wheelPlayerService.ActivatePaidSpinAsync(dto.UserId);
                        break;

                    default:
                        break;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Purchase created successfully for user {UserId}, type {Type}, ref {RefId}.",
                    dto.UserId, dto.ItemType, dto.ReferenceId);

                // TODO: send email to user.
                return new GeneralResult(true, _localization.GetLocalizedString("PurchaseCreatedSuccessfully"));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "CreatePurchaseAsync: Unexpected error for user {UserId}", dto.UserId);
                return new GeneralResult(false, _localization.GetLocalizedString("UnexpectedError"));
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<List<PurchaseDetailsDto>>> GetUserPurchasesAsync(string userId, PurchaseItemType? type = null)
        {
            try
            {
                var user = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

                if (user == null)
                {
                    _logger.LogWarning("GetUserPurchasesAsync: User not found. ID={UserId}", userId);
                    return new GeneralResult<List<PurchaseDetailsDto>>(
                        false,
                        _localization.GetLocalizedString("UserNotFound"),
                        null,
                        ErrorType.NotFound);
                }

                var query = _dbContext.Purchases
                    .AsNoTracking()
                    .Where(p => p.UserId == userId && !p.IsDeleted);

                if (type.HasValue)
                    query = query.Where(p => p.ItemType == type.Value);

                var purchases = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                var result = purchases.Select(p => new PurchaseDetailsDto
                {
                    Id = p.Id,
                    ItemType = p.ItemType,
                    ReferenceId = p.ReferenceId,
                    MetadataJson = p.MetadataJson ?? string.Empty,
                    Price = p.Price,
                    IsRefunded = p.IsRefunded,
                    RefundedAt = p.RefundedAt,
                    CreatedAt = p.CreatedAt ?? DateHelper.UtcNow,
                    userData = new PayingUser
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    }
                }).ToList();

                return new GeneralResult<List<PurchaseDetailsDto>>(
                    true,
                    _localization.GetLocalizedString("PurchasesRetrieved"),
                    result,
                    ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserPurchasesAsync: Error retrieving purchases for user {UserId}", userId);
                return new GeneralResult<List<PurchaseDetailsDto>>(
                    false,
                    _localization.GetLocalizedString("UnexpectedError"),
                    null,
                    ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<PurchaseDetailsDto>> GetByIdAsync(int purchaseId)
        {
            try
            {
                var purchase = await _dbContext.Purchases
                    .AsNoTracking()
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == purchaseId && !p.IsDeleted);

                if (purchase == null)
                {
                    _logger.LogWarning("GetByIdAsync: Purchase not found. ID={PurchaseId}", purchaseId);
                    return new GeneralResult<PurchaseDetailsDto>(
                        false,
                        _localization.GetLocalizedString("PurchaseNotFound"),
                        null,
                        ErrorType.NotFound);
                }

                var dto = new PurchaseDetailsDto
                {
                    Id = purchase.Id,
                    ItemType = purchase.ItemType,
                    ReferenceId = purchase.ReferenceId,
                    MetadataJson = purchase.MetadataJson ?? string.Empty,
                    Price = purchase.Price,
                    IsRefunded = purchase.IsRefunded,
                    RefundedAt = purchase.RefundedAt,
                    CreatedAt = purchase.CreatedAt ?? DateHelper.UtcNow,
                    userData = new PayingUser
                    {
                        Id = purchase.User.Id,
                        FirstName = purchase.User.FirstName,
                        LastName = purchase.User.LastName,
                        Email = purchase.User.Email
                    },
                };

                return new GeneralResult<PurchaseDetailsDto>(
                    true,
                    _localization.GetLocalizedString("PurchaseRetrieved"),
                    dto,
                    ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync: Error retrieving purchase. ID={PurchaseId}", purchaseId);
                return new GeneralResult<PurchaseDetailsDto>(
                    false,
                    _localization.GetLocalizedString("UnexpectedError"),
                    null,
                    ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult> RefundPurchaseAsync(int purchaseId, string reason)
        {
            try
            {
                var purchase = await _dbContext.Purchases
                    .FirstOrDefaultAsync(p => p.Id == purchaseId && !p.IsDeleted);

                if (purchase == null)
                {
                    _logger.LogWarning("Refund failed. Purchase ID {PurchaseId} not found.", purchaseId);
                    return new GeneralResult(
                    false, _localization.GetLocalizedString("PurchaseNotFound"),
                    null, ErrorType.NotFound );
                }

                if (purchase.IsRefunded)
                {
                    _logger.LogWarning("Refund attempt on already refunded purchase. ID: {PurchaseId}", purchaseId);
                    return new GeneralResult(
                    false, _localization.GetLocalizedString("AlreadyRefunded"),
                    null, ErrorType.InvalidData );
                }

                purchase.IsRefunded = true;
                purchase.RefundedAt = DateTimeOffset.UtcNow;

                // save in metadata
                var metadata = string.IsNullOrWhiteSpace(purchase.MetadataJson)
                    ? new Dictionary<string, object>()
                    : JsonSerializer.Deserialize<Dictionary<string, object>>(purchase.MetadataJson!) ?? new();

                metadata["refundReason"] = reason;
                metadata["refundedBy"] = _httpContextHelper.GetCurrentUserIdAsync();
                metadata["refundedAt"] = purchase.RefundedAt;

                purchase.MetadataJson = JsonSerializer.Serialize(metadata);
                purchase.UpdatedAt = DateTimeOffset.UtcNow;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Purchase ID {PurchaseId} refunded successfully.", purchaseId);
                return new GeneralResult(
                true, _localization.GetLocalizedString("PurchaseRefunded"),
                null, ErrorType.Success );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refunding purchase ID {PurchaseId}.", purchaseId);
                return new GeneralResult(
                false, _localization.GetLocalizedString("UnexpectedError"),
                null, ErrorType.InternalServerError );
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<PurchaseStatsDto>> GetPurchaseStatsAsync(PurchaseItemType itemType, int referenceId)
        {
            try
            {
                var query = _dbContext.Purchases
                    .AsNoTracking()
                    .Where(p => p.ItemType == itemType && !p.IsDeleted);

                if (referenceId > 0)
                    query = query.Where(p => p.ReferenceId == -1);

                var purchases = await query.ToListAsync();

                if (!purchases.Any())
                {
                    _logger.LogInformation("No purchases found for item {ItemType} with reference ID {ReferenceId}.", itemType, referenceId);
                    return new GeneralResult<PurchaseStatsDto>(
                        false,
                        _localization.GetLocalizedString("NoPurchasesFound"),
                        null,
                        ErrorType.NotFound);
                }

                var stats = new PurchaseStatsDto
                {
                    TotalPurchases = purchases.Count,
                    TotalRevenue = purchases.Sum(p => p.Price)
                };

                _logger.LogInformation("Stats for item {ItemType} ID {ReferenceId}: {Count} purchases, {Revenue} total.",
                    itemType, referenceId, stats.TotalPurchases, stats.TotalRevenue);

                return new GeneralResult<PurchaseStatsDto>(
                    true,
                    _localization.GetLocalizedString("PurchaseStatsRetrieved"),
                    stats,
                    ErrorType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase stats for item {ItemType}, ref {ReferenceId}.", itemType, referenceId);
                return new GeneralResult<PurchaseStatsDto>(
                    false,
                    _localization.GetLocalizedString("UnexpectedError"),
                    null,
                    ErrorType.InternalServerError);
            }
        }

        /// <inheritdoc/>
        public async Task<GeneralResult<bool>> HasUserPurchasedAsync(string userId, PurchaseItemType itemType, int referenceId)
        {
            try
            {
                var hasPurchased = await _dbContext.Purchases
                    .AsNoTracking()
                    .AnyAsync(p =>
                        p.UserId == userId &&
                        p.ItemType == itemType &&
                        p.ReferenceId == referenceId &&
                        !p.IsDeleted &&
                        !p.IsRefunded );

                if (!hasPurchased)
                {
                    _logger.LogInformation("Purchase check for user {UserId}, item {ItemType}, ref {ReferenceId}: false",
                        userId, itemType, referenceId);
                    return new GeneralResult<bool>(
                    false,
                    _localization.GetLocalizedString("PurchaseCheckSuccess"),
                    false,
                    ErrorType.Success );
                }

                _logger.LogInformation("Purchase check for user {UserId}, item {ItemType}, ref {ReferenceId}: {Result}",
                    userId, itemType, referenceId, hasPurchased);

                return new GeneralResult<bool>(
                    true,
                    _localization.GetLocalizedString("PurchaseCheckSuccess"),
                    hasPurchased,
                    ErrorType.Success );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking purchase for user {UserId}, item {ItemType}, ref {ReferenceId}.",
                    userId, itemType, referenceId);

                return new GeneralResult<bool>(
                    false,
                    _localization.GetLocalizedString("UnexpectedError"),
                    false,
                    ErrorType.InternalServerError );
            }
        }
    }
}

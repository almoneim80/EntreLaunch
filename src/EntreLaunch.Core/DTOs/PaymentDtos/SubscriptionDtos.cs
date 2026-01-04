namespace EntreLaunch.DTOs.PaymentDtos;

/// <summary>
/// DTO for creating a new subscription.
/// </summary>
public class SubscriptionCreateDto
{
#nullable disable
    public string UserId { get; set; }
    public SubscriptionType Type { get; set; }
#nullable enable
    public int? ReferenceId { get; set; }
#nullable disable
    public bool IsAutoRenewal { get; set; }
    public decimal Price { get; set; }
    public int? PaymentId { get; set; }
    public int? TrialPeriodDays { get; set; }
}

public class SubscriptionDto
{
#nullable enable
    public int Id { get; set; }
    public SubscriptionType Type { get; set; }

    public int ReferenceId { get; set; }
    public string? ReferenceName { get; set; }

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }

    public bool IsAutoRenewal { get; set; }
    public SubscriptionStatus Status { get; set; }

    public decimal Price { get; set; }

    public string? UserFullName { get; set; }
    public string? UserEmail { get; set; }
}

/// <summary>
/// DTO for subscription statistics.
/// </summary>
public class SubscriptionStatsDto
{
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int ExpiredSubscriptions { get; set; }
    public int CancelledSubscriptions { get; set; }
    public int TrialSubscriptions { get; set; }

    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }

    public int UniqueUsersSubscribed { get; set; }
    public Dictionary<SubscriptionType, int> SubscriptionsByType { get; set; } = new();
}

/// <summary>
/// DTO for subscription lookup.
/// </summary>
public class SubscriptionLookupDto
{
    [Required]
    public SubscriptionType Type { get; set; }

    [Required]
    public int ReferenceId { get; set; }
}

/// <summary>
/// DTO for extending a subscription.
/// </summary>
public class ExtendSubscriptionDto
{
    [Required]
    public int SubscriptionId { get; set; }

    [Required]
    public int ExtraDays { get; set; }
}

/// <summary>
/// DTO for canceling a subscription.
/// </summary>
public class CancelSubscriptionDto
{
    [Required]
    public int SubscriptionId { get; set; }

    public string? Reason { get; set; }
}

/// <summary>
/// DTO for starting a trial subscription.
/// </summary>
public class StartTrialSubscriptionDto
{
    [Required]
    public SubscriptionType Type { get; set; }

    [Required]
    public int ReferenceId { get; set; }
}

/// <summary>
/// DTO for upgrading a subscription.
/// </summary>
public class UpgradeSubscriptionDto
{
    [Required]
    public int CurrentSubscriptionId { get; set; }

    [Required]
    public int NewReferenceId { get; set; }

    [Required]
    public decimal AdditionalPrice { get; set; }
}

/// <summary>
/// DTO for creating a child subscription.
/// </summary>
public class ChildSubscriptionCreateDto
{
    [Required]
    public int ParentSubscriptionId { get; set; }

    [Required]
    public string ChildUserId { get; set; } = null!;
}

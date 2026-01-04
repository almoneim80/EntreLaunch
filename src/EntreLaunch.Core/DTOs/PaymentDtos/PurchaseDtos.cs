namespace EntreLaunch.DTOs.PaymentDtos;

/// <summary>
/// Data of a purchase create.
/// </summary>
public class PurchaseCreateDto
{
    public string UserId { get; set; } = null!;
    public PurchaseItemType ItemType { get; set; }
    public int ReferenceId { get; set; }
    public int PaymentId { get; set; }
    public decimal Price { get; set; }
    public string? MetadataJson { get; set; }
}

/// <summary>
/// Data of a purchase details.
/// </summary>
public class PurchaseDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public PurchaseItemType ItemType { get; set; }
    public int ReferenceId { get; set; }
    public string MetadataJson { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsRefunded { get; set; }
    public DateTimeOffset? RefundedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public PayingUser userData { get; set; }
}

public class PayingUser
{
#nullable disable
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

/// <summary>
/// Data of purchase stats.
/// </summary>
public class PurchaseStatsDto
{
    public int TotalPurchases { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class RefundPurchaseDto
{
#nullable enable
    public int PurchaseId { get; set; }
    public string? Reason { get; set; }
}
public class PurchaseLookupDto
{
    public PurchaseItemType ItemType { get; set; }
    public int ReferenceId { get; set; }
}

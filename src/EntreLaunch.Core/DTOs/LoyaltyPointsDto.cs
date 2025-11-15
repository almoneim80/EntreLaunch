namespace EntreLaunch.DTOs;

public class BonusPointsRequest
{
    public string UserId { get; set; } = null!;
    public int Points { get; set; }
    public string? Reason { get; set; }
}

public class RedeemPointsRequestDto
{
    public string UserId { get; set; } = null!;
    public int PaymentId { get; set; }
    public int PointsToUse { get; set; }
}

public class PointsTransactionDto
{
    public string UserId { get; set; } = null!;
    public int PointsChanged { get; set; }
    public string Reason { get; set; } = null!;
    public int? PaymentId { get; set; }
}

public class LoyaltyPointsResult
{
    public int Points { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}

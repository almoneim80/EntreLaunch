namespace EntreLaunch.DTOs.PaymentDtos;

public class BonusPointsRequest
{
    public int Points { get; set; }
    public string? Reason { get; set; }
}

public class RedeemPointsRequestDto
{
    public int PaymentId { get; set; }
    public int PointsToUse { get; set; }
}

public class PointsTransactionDto
{
    [JsonIgnore]
    public string? UserId { get; set; }
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

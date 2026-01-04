namespace EntreLaunch.DTOs.PaymentDtos;

public class PaymentCreateDto
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? NetAmount { get; set; }
    public string? Currency { get; set; }
    public string? DiscountCode { get; set; }
    public string? Status { get; set; }
    public PaymentPurpose PaymentPurpose { get; set; }
    public int? TargetId { get; set; }
    public PaymentType TargetType { get; set; }
    [JsonIgnore]
    public DateTimeOffset? PaymentDate { get; set; } = DateTimeOffset.UtcNow;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class PaymentDetailsDto : PaymentCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; }
}

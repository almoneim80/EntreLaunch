using System.Text.Json.Serialization;

namespace EntreLaunch.DTOs;

public class PaymentCreateDto
{
    public string UserId { get; set; } = null!;
    public decimal? Amount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? NetAmount { get; set; }
    public string? Currency { get; set; }
    public string? DiscountCode { get; set; }
    public string Status { get; set; } = "Pending";
    public string? PaymentPurpose { get; set; }
    public int? TargetId { get; set; }
    public string? TargetType { get; set; }
    [JsonIgnore]
    public DateTimeOffset? PaymentDate { get; set; } = DateTimeOffset.UtcNow.DateTime;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class PaymentEntreLaunchdateDto
{
    public string UserId { get; set; } = null!;
    public decimal? Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public string? PaymentPurpose { get; set; }
    public int? TargetId { get; set; }
    public string? TargetType { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class PaymentDetailsDto : PaymentCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

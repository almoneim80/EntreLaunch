namespace EntreLaunch.DTOs.PaymentDtos;

public class RefundCreateDto
{
    public int PaymentId { get; set; }
    public string? Reason { get; set; }

    [JsonIgnore]
    public DateTimeOffset? RefundDate { get; set; } = DateHelper.UtcNow;

    [JsonIgnore]
    public ProcessStatus Status { get; set; } = ProcessStatus.Pending;

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class RefundDetailsDto : RefundCreateDto
{
    public int Id { get; set; }
    public decimal? Amount { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; }
}

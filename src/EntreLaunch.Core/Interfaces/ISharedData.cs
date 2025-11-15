namespace EntreLaunch.Interfaces;

public interface ISharedData
{
    bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset? SoftDeleteExpiration { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

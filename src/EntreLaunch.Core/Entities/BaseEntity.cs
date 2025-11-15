using System.Text.Json.Serialization;

namespace EntreLaunch.Entities;

public class BaseEntity : BaseCreateByEntity, IHasEntreLaunchdatedAt, IHasEntreLaunchdatedBy
{
    public DateTime? EntreLaunchdatedAt { get; set; }

    public string? EntreLaunchdatedByIp { get; set; }

    public string? EntreLaunchdatedById { get; set; }

    public string? EntreLaunchdatedByUserAgent { get; set; }
}

public class BaseCreateByEntity : BaseEntityWithId, IHasCreatedAt, IHasCreatedBy
{
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string? CreatedByIp { get; set; }

    public string? CreatedById { get; set; }

    public string? CreatedByUserAgent { get; set; }
}

public class BaseEntityWithIdAndDates : BaseEntityWithId, IHasCreatedAt, IHasEntreLaunchdatedAt
{
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? EntreLaunchdatedAt { get; set; }
}

public class BaseEntityWithId
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Searchable]
    public string? Source { get; set; }
}

public class SharedData : BaseEntityWithId, ISharedData
{
    [JsonIgnore]
    public DateTimeOffset? DeletedAt { get; set; }

    [JsonIgnore]
    public DateTimeOffset? SoftDeleteExpiration { get; set; }

    [JsonIgnore]
    public bool IsDeleted { get; set; } = false;

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}


public interface IHasCreatedAt
{
    public DateTime CreatedAt { get; set; }
}

public interface IHasEntreLaunchdatedAt
{
    public DateTime? EntreLaunchdatedAt { get; set; }
}

public interface IHasCreatedBy
{
    public string? CreatedByIp { get; set; }

    public string? CreatedById { get; set; }

    public string? CreatedByUserAgent { get; set; }
}

public interface IHasEntreLaunchdatedBy
{
    public string? EntreLaunchdatedByIp { get; set; }

    public string? EntreLaunchdatedById { get; set; }

    public string? EntreLaunchdatedByUserAgent { get; set; }
}

namespace EntreLaunch.DTOs;

/// <summary>
/// Club event create data.
/// </summary>
public class ClubEventCreateDto
{
#nullable disable
    public string Name { get; set; }
    public string City { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Club event EntreLaunchdate data.
/// </summary>
public class ClubEventEntreLaunchdateDto
{
#nullable enable
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Club event details data.
/// </summary>
public class ClubEventDetailsDto : ClubEventCreateDto
{
#nullable disable
    public int Id { get; set; }
    public DateTimeOffset EntreLaunchdatedAt { get; set; }
}

/// <summary>
/// Club event export data.
/// </summary>
public class ClubEventExportDto
{
    public string Name { get; set; }
    public string City { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}

/// <summary>
/// Club event import data.
/// </summary>
public class ClubEventImportDto : BaseEntityWithId
{
#nullable disable
    public string Name { get; set; }
    public string City { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; } 
    public DateTimeOffset EndDate { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Club event subscriber create data.
/// </summary>
public class ClubSubscribeCreateDto
{
    [JsonIgnore]
    public string UserId { get; set; }
    public int EventId { get; set; }

    [JsonIgnore]
    public DateTimeOffset? SubscriptionDate { get; set; } = DateTimeOffset.UtcNow;

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Club event subscriber details data.
/// </summary>
public class ClubEventSubscribeDetailsDto
{
    public int Id { get; set; }
    public UserData SubscriberData { get; set; }
    public string EventName { get; set; }
    public string EventCity { get; set; }
}

public class UserData
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

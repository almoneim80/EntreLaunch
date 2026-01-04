namespace EntreLaunch.DTOs.ClubDtos;

/// <summary>
/// Club event create data.
/// </summary>
public class ClubEventCreateDto
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    [JsonIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateHelper.UtcNow;
}

/// <summary>
/// Club event update data.
/// </summary>
public class ClubEventUpdateDto
{
#nullable enable
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }

    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}

/// <summary>
/// Club event details data.
/// </summary>
public class ClubEventDetails
{
#nullable disable
    public int Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public List<UserData> EventSubscribers { get; set; }
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
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

/************* Club event registration *************/

/// <summary>
/// Club event registration data.
/// </summary>
public class ClubEventRegistrationCreateDto
{
#nullable disable

    [JsonIgnore]
    public string UserId { get; set; }
    public int EventId { get; set; }
    public string Notes { get; set; } = string.Empty;

    [JsonIgnore]
    public DateTimeOffset RegisteredAt { get; set; } = DateHelper.UtcNow;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

/// <summary>
/// Club event subscriber details data.
/// </summary>
public class ClubEventRegistrationDetailsDto
{
#nullable disable
    public UserData UserData { get; set; }
    public ClubEventDetails clubEventDetailsDto { get; set; }
    public DateTimeOffset RegisteredAt { get; set; }
    public bool IsCancelled { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public string Notes { get; set; }
}

public class UserData
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

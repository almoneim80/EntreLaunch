namespace EntreLaunch.DTOs;

/// <summary>
/// Data of Consultation create.
/// </summary>
public class ConsultationCreateDto
{
    public int CounselorId { get; set; }
    public string ClientId { get; set; } = null!;
    public int? ConsultationTimeId { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationStatus? Status { get; set; } = ConsultationStatus.Scheduled;
    public string? Description { get; set; }
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of Consultation EntreLaunchdate.
/// </summary>
public class ConsultationEntreLaunchdateDto
{
    public string? AdvisorId { get; set; }
    public string? ClientId { get; set; }
    public int? ConsultationTimeId { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationStatus? Status { get; set; }
    public string? Description { get; set; }
    public int? TicketId { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of Consultation Details.
/// </summary>
public class ConsultationDetailsDto : ConsultationCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

/// <summary>
/// Data of Consultation Time create.
/// </summary>
public class ConsultationTimeCreateDto
{
    public int CounselorId { get; set; }
    public DateTimeOffset? DateTimeSlot { get; set; }
    public bool IsBooked { get; set; } = false;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of Consultation Time Details.
/// </summary>
public class ConsultationTimeDetailsDto
{
    public int Id { get; set; }
    public int CounselorId { get; set; }
    public DateTimeOffset? DateTimeSlot { get; set; }
    public bool IsBooked { get; set; } = false;
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

public class ConsultationTimeEntreLaunchdateDto
{
    public int CounselorId { get; set; } 
    public DateTimeOffset? DateTimeSlot { get; set; }
    public bool IsBooked { get; set; } = false;
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

public class ConsultationTimeImportDto : BaseEntityWithId
{
    public int CounselorId { get; set; }
    public DateTimeOffset? DateTimeSlot { get; set; }
    public bool IsBooked { get; set; } = false;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of ticket attachment create.
/// </summary>
public class TicketAttachmentCreateDto
{
    public int TicketId { get; set; }
    [JsonIgnore]
    public string? SenderId { get; set; }
    public string? Url { get; set; }

    [JsonIgnore]
    public DateTimeOffset? SendTime { get; set; } = DateTimeOffset.UtcNow.DateTime;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of ticket attachment details.
/// </summary>
public class TicketAttachmentDetailsDto : TicketAttachmentCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

/// <summary>
/// Data of ticket create.
/// </summary>
public class TicketCreateDto
{
    public int CreatorId { get; set; } //counselor
    public int ConsultationId { get; set; }
    public ConsultationTicketStatus Status { get; set; }

    [JsonIgnore]
    public DateTimeOffset? StartDate { get; set; } = DateTimeOffset.UtcNow.DateTime;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of ticket details.
/// </summary>
public class TicketDetailsDto
{
    public int Id { get; set; }
    public int CreatorId { get; set; }
    public int ConsultationId { get; set; }
    public ConsultationTicketStatus Status { get; set; }
    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; }
}

/// <summary>
/// Data of ticket message create.
/// </summary>
public class TicketMessageCreateDto
{
    public int TicketId { get; set; }
    [JsonIgnore]
    public string SenderId { get; set; } = null!;
    public string? Content { get; set; }

    [JsonIgnore]
    public DateTimeOffset? SendTime { get; set; } = DateTimeOffset.UtcNow.DateTime;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of ticket message EntreLaunchdate.
/// </summary>
public class TicketMessageEntreLaunchdateDto
{
    public string? Content { get; set; }

    [JsonIgnore]
    public DateTimeOffset? EntreLaunchdatedAt { get; set; } = DateTimeOffset.UtcNow.DateTime;
}

/// <summary>
/// Data of ticket message details.
/// </summary>
public class TicketMessageDetailsDto
{
    public int Id { get; set; }
    public int? TicketId { get; set; }
    public string? SenderId { get; set; }
    public string? Content { get; set; }
    public bool IsClientMessage { get; set; }
    public DateTimeOffset SendTime { get; set; }
}

/// <summary>
/// Data of processing Ticket status (close , open).
/// </summary>
public class ProcessTicketDto
{
    public int Id { get; set; }
    public ConsultationTicketStatus Status { get; set; }
}

public class CreateCounselorRequestDto
{
    public string UserId { get; set; } = null!;
    public string? Qualification { get; set; }
    public string? City { get; set; }
    public int SpecializationExperience { get; set; }
    public int ConsultingExperience { get; set; }
    public int DailyHours { get; set; }
    public Dictionary<string, string>? SocialMediaAccounts { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class CounselorRequestDetailsDto
{
    // user
    public string? FullName { get; set; }
    public double? NationalId { get; set; }
    public string? Specialization { get; set; }
    public Country CountryCode { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }

    // counselor
    public int Id { get; set; }
    public string? Qualification { get; set; }
    public string? City { get; set; }
    public int SpecializationExperience { get; set; }
    public int ConsultingExperience { get; set; }
    public int DailyHours { get; set; }
    public Dictionary<string, string>? SocialMediaAccounts { get; set; }
    public CounselorRequesttStatus Status { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
}

/// <summary>
/// Data of processing counselor request status (close , open).
/// </summary>
public class ProcessCounselorRequestDto
{
    public int Id { get; set; }
    public CounselorRequesttStatus Status { get; set; }
}

public class ProcessConsultationStatusDto
{
    public int Id { get; set; }
    public ConsultationStatus Status { get; set; }
}

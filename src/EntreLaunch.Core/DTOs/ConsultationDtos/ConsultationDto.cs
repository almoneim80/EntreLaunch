namespace EntreLaunch.DTOs.ConsultationDtos;

/***************************************************** CONSULTATION DTO ******************************************************/
/***************************************************** CONSULTATION DTO ******************************************************/
public class OnlineConsultationCreateDto
{
#nullable disable
    public int CounselorId { get; set; }
    public int ConsultationTimeId { get; set; }
    public string Description { get; set; }

    [JsonIgnore]
    public string ClientId { get; set; }
    [JsonIgnore]
    public ConsultationType Type { get; set; }
    [JsonIgnore]
    public ConsultationStatus Status { get; set; } = ConsultationStatus.Scheduled;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class TextConsultationCreateDto
{
#nullable disable
    public int CounselorId { get; set; }
    public string Description { get; set; }

    [JsonIgnore]
    public string ClientId { get; set; }
    [JsonIgnore]
    public ConsultationType Type { get; set; }
    [JsonIgnore]
    public ConsultationStatus Status { get; set; } = ConsultationStatus.Scheduled;

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class ProcessConsultationStatusDto
{
    public int Id { get; set; }
    public ConsultationStatus Status { get; set; }
}

public class ConsultationUpdateDto
{
#nullable enable
    public string? CounselorId { get; set; }
    public int? ConsultationTimeId { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationStatus? Status { get; set; }
    public string? Description { get; set; }
    public int? TicketId { get; set; }

    [JsonIgnore]
    public string? ClientId { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}

public class ConsultationDetailsDto : OnlineConsultationCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class ConsultationAllData
{
#nullable disable
    public int Id { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationStatus Status { get; set; }
    public string Description { get; set; }

    public CounselorData counselorData { get; set; }

    public DateTimeOffset ConsultationTimeDate { get; set; }
    public CustomerData customerData { get; set; }
}

/***************************************************** CONSULTATION TIME DTO ******************************************************/
/***************************************************** CONSULTATION TIME DTO ******************************************************/

public class ConsultationTimeCreateDto
{
    public int CounselorId { get; set; }
    public DateTimeOffset DateTimeSlot { get; set; }
    public bool IsRecurringDaily { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class ConsultationTimeUpdateDto
{
    public int CounselorId { get; set; }
    public DateTimeOffset? DateTimeSlot { get; set; }

    [JsonIgnore]
    public DateTimeOffset UpdatedAt { get; set; } = DateHelper.UtcNow;
}

public class ConsultationTimeDetailsDto
{
    public int CounselorId { get; set; }
    public DateTimeOffset? DateTimeSlot { get; set; }
    public bool IsBooked { get; set; } = false;
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class ConsultationTimeImportDto : BaseEntityWithId
{
    public int CounselorId { get; set; }
    public DateTimeOffset? DateTimeSlot { get; set; }
    public bool IsBooked { get; set; } = false;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

/***************************************************** CONSULTATION TICKET DTO ******************************************************/
/***************************************************** CONSULTATION TICKET DTO ******************************************************/

public class TicketCreateDto
{
    public int CreatorId { get; set; } //counselor
    public int ConsultationId { get; set; }

    [JsonIgnore]
    public ConsultationTicketStatus Status { get; set; } = ConsultationTicketStatus.Open;

    [JsonIgnore]
    public DateTimeOffset? StartDate { get; set; } = DateHelper.UtcNow;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class TicketAttachmentCreateDto
{
#nullable disable
    public int TicketId { get; set; }
    public string Url { get; set; }

    [JsonIgnore]
    public string SenderId { get; set; }
    [JsonIgnore]
    public DateTimeOffset? SendTime { get; set; } = DateHelper.UtcNow;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class TicketAttachmentDetailsDto : TicketAttachmentCreateDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class TicketMessageCreateDto
{
#nullable disable
    public int TicketId { get; set; }
    [JsonIgnore]
    public string SenderId { get; set; }
    public string Content { get; set; }

    [JsonIgnore]
    public DateTimeOffset? SendTime { get; set; } = DateHelper.UtcNow;
    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class TicketMessageUpdateDto
{
#nullable enable
    public string? Content { get; set; }

    [JsonIgnore]
    public DateTimeOffset? UpdatedAt { get; set; } = DateHelper.UtcNow;
}

public class TicketMessageDetailsDto
{
    public int Id { get; set; }
    public int? TicketId { get; set; }
    public string? SenderId { get; set; }
    public string? Content { get; set; }
    public bool IsClientMessage { get; set; }
    public DateTimeOffset SendTime { get; set; }
}

public class ProcessTicketDto
{
    public int Id { get; set; }
    public ConsultationTicketStatus Status { get; set; }
}

public class TicketFullDetailsDto
{
#nullable disable
    public int Id { get; set; }
    public CounselorData Creator { get; set; }
    public ConsultationAllData Consultation { get; set; }
    public ConsultationTicketStatus Status { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public TicketMessages Textmessages { get; set; }
    public MediaMessages Media { get; set; }
}

public class TicketMessages
{
    public int Id { get; set; }
    public string SenderId { get; set; }
    public string SenderName { get; set; }
    public string Content { get; set; }
    public bool IsClientMessage { get; set; }
    public DateTimeOffset SendTime { get; set; }
}

public class MediaMessages
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string MediaType { get; set; }
    public string SenderId { get; set; }
    public string SenderName { get; set; }
    public bool IsClientMessage { get; set; }
    public DateTimeOffset SendTime { get; set; }
}

/***************************************************** COUNSELOR DTO ******************************************************/
/***************************************************** COUNSELOR DTO ******************************************************/

public class CreateCounselorRequestDto
{
#nullable disable
    [JsonIgnore]
    public string UserId { get; set; }
    public string Qualification { get; set; }
    public string City { get; set; }
    public int SpecializationExperience { get; set; }
    public int ConsultingExperience { get; set; }
    public int DailyHours { get; set; }
    public Dictionary<string, string> SocialMediaAccounts { get; set; }

    [JsonIgnore]
    public DateTimeOffset? CreatedAt { get; set; } = DateHelper.UtcNow;
}

public class CounselorRequestDetailsDto
{
#nullable enable
    // user
    public int Id { get; set; }
    public string? FullName { get; set; }
    public double? NationalId { get; set; }
    public string? Specialization { get; set; }
    public Country CountryCode { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }

    // counselor
    public string? Qualification { get; set; }
    public string? City { get; set; }
    public int SpecializationExperience { get; set; }
    public int ConsultingExperience { get; set; }
    public int DailyHours { get; set; }
    public Dictionary<string, string>? SocialMediaAccounts { get; set; }
    public CounselorRequesttStatus Status { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }

    public List<CounselorTimeDataDto>? counselorTimeData { get; set; }
}

public class ProcessCounselorRequestDto
{
    public int Id { get; set; }
    public CounselorRequesttStatus Status { get; set; }
}

public class CounselorTimeDataDto
{
    public DateTimeOffset? DateTimeSlot { get; set; }
    public bool IsBooked { get; set; }
}

public class CounselorData
{
#nullable disable
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Specialization { get; set; }
    public Country CountryCode { get; set; }
    public string Email { get; set; }
    public string Qualification { get; set; }
    public string City { get; set; }
}

public class CounselorSummaryStatsDto
{
    public int ActiveCounselors { get; set; }
    public int PendingRequests { get; set; }
    public int AvailableHours { get; set; }
}

/***************************************************** OTHER DTO ******************************************************/
/***************************************************** OTHER DTO ******************************************************/

public class CustomerData
{
#nullable enable
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public double NationalId { get; set; } = 0;
    public string Specialization { get; set; } = string.Empty;
    public Country CountryCode { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTimeOffset? DateOfBirth { get; set; } = null;
}

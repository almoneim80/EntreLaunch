namespace EntreLaunch.Interfaces.ConsultationIntf
{
    public interface IConsultation
    {
        /// <summary>
        /// Check if user is a counselor.
        /// </summary>
        Task<GeneralResult<bool>> IsCounselor(int id);

        /// <summary>
        /// Processing Ticket Status (close or open).
        /// </summary>
        Task<GeneralResult> ProgressTicket([FromBody] ProcessTicketDto processTicketDto);

        /// <summary>
        /// Send request to be a Counselor.
        /// </summary>
        Task<GeneralResult> SendCounselorRequest([FromBody] CreateCounselorRequestDto counselorRequest);

        /// <summary>
        /// Show all Counselor Request.
        /// </summary>
        Task<GeneralResult<List<CounselorRequestDetailsDto>>> AllCounselorRequest();

        /// <summary>
        /// Show all pending counselor request.
        /// </summary>
        Task<GeneralResult<List<CounselorRequestDetailsDto>>> PendingCounselorRequest();

        /// <summary>
        /// Show all counselor by specialization.
        /// </summary>
        Task<GeneralResult<List<CounselorRequestDetailsDto>>> CounselorBySpecialization(string specialization);

        /// <summary>
        /// Show counselor CV.
        /// </summary>
        Task<GeneralResult<List<CounselorRequestDetailsDto>>> CounselorCV(int id);

        /// <summary>
        /// Show all accepted counselor request.
        /// </summary>
        Task<GeneralResult<List<CounselorRequestDetailsDto>>> AcceptedCounselorRequest();

        /// <summary>
        /// Show all rejected counselor request.
        /// </summary>
        Task<GeneralResult<List<CounselorRequestDetailsDto>>> RejectedCounselorRequest();

        /// <summary>
        /// Processing counselor request Status (accept or reject).
        /// </summary>
        Task<GeneralResult> ProgressCounselorRequest([FromBody] ProcessCounselorRequestDto processCounselorRequest);

        /// <summary>
        /// Show all consultation Request.
        /// </summary>
        Task<GeneralResult<List<ConsultationDetailsDto>>> AllConsultationRequest();

        /// <summary>
        /// Show one consultation Request by id.
        /// </summary>
        Task<GeneralResult<ConsultationDetailsDto>> GetConsultationRequestById(int id);

        /// <summary>
        /// Show one consultation Request by id.
        /// </summary>
        Task<GeneralResult<List<ConsultationDetailsDto>>> GetConsultationByType(ConsultationType type);

        /// <summary>
        /// Send consultation request.
        /// </summary>
        Task<GeneralResult> BookingConsultation(ConsultationCreateDto consultationCreateDto);

        /// <summary>
        /// Send text consultation.
        /// </summary>
        Task<GeneralResult> SendTextConsultation(ConsultationCreateDto consultationCreateDto);

        /// <summary>
        /// Processing consultation Status (Scheduled, in-progress, completed, cancelled).
        /// </summary>
        Task<GeneralResult> ProgressConsultationStatus([FromBody] ProcessConsultationStatusDto processConsultationStatus);

        /// <summary>
        /// Show consultations for counselor by its id.
        /// </summary>
        Task<GeneralResult<List<ConsultationDetailsDto>>> GetConsultationForCounselor(int id);

        /// <summary>
        /// open new consultation ticket.
        /// </summary>
        Task<GeneralResult> OpenTicket([FromBody] TicketCreateDto ticketCreateDto);

        /// <summary>
        /// Show all consultation tickets.
        /// </summary>
        Task<GeneralResult<List<TicketDetailsDto>>> AllConsultationTickets();

        /// <summary>
        /// Show one consultation ticket by id.
        /// </summary>
        Task<GeneralResult<TicketDetailsDto>> GetTicketById(int id);

        /// <summary>
        /// Show consultation ticket by its id.
        /// </summary>
        Task<GeneralResult<TicketDetailsDto>> GetConsultationTicketById(int id);

        /// <summary>
        /// Send message to ticket.
        /// </summary>
        Task<GeneralResult> SendTicketMessage([FromBody] TicketMessageCreateDto ticketMessageCreate);

        /// <summary>
        /// Edit message to ticket.
        /// </summary>
        Task<GeneralResult> EditTicketMessage(int id, [FromBody] TicketMessageEntreLaunchdateDto ticketMessageEntreLaunchdate);

        /// <summary>
        /// Delete message to ticket.
        /// </summary>
        Task<GeneralResult> DeleteTicketMessage(int id);

        /// <summary>
        /// Get message of a  specific ticket.
        /// </summary>
        Task<GeneralResult<List<TicketMessageDetailsDto>>> ShowTicketMessages(int id);

        // Counselor Time methods.

        /// <summary>
        /// create new consultation time.
        /// </summary>
        Task<GeneralResult> CreateCounselorTime([FromBody] ConsultationTimeCreateDto consultationTimeCreateDto);

        /// <summary>
        /// edit consultation time.
        /// </summary>
        Task<GeneralResult> EditCounselorTimes(int id, ConsultationTimeEntreLaunchdateDto consultationTimeEntreLaunchdateDto);

        /// <summary>
        /// get all consultation times of a counselor.
        /// </summary>
        Task<GeneralResult<List<ConsultationTimeDetailsDto>>> GetAllCounselorTimes(int id);

        /// <summary>
        /// Send attachment to ticket.
        /// </summary>
        Task<GeneralResult> SendTicketAttachment([FromBody] TicketAttachmentCreateDto ticketAttachmentCreateDto);

        /// <summary>
        /// Delete attachment to ticket.
        /// </summary>
        Task<GeneralResult> DeleteTicketAttachment(int id, string userId);

        /// <summary>
        /// Get attachment of a  specific ticket.
        /// </summary>
        Task<GeneralResult<List<TicketAttachmentDetailsDto>>> ShowTicketAttachment(int id);
    }
}

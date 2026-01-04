using EntreLaunch.DTOs.BaseDtos;
using EntreLaunch.DTOs.ConsultationDtos;
using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Interfaces.ConsultationsIntf
{
    public interface ITicketService
    {
        /// <summary>
        /// Create a new ticket for a consultation.
        /// </summary>
        Task<GeneralResult> CreateTicket(TicketCreateDto dto);

        /// <summary>
        /// Update the status of a ticket (e.g., Open, Closed).
        /// </summary>
        Task<GeneralResult> UpdateTicketStatus(ProcessTicketDto dto);

        /// <summary>
        /// Retrieve all tickets in the system.
        /// </summary>
        Task<GeneralResult<PaginatedResult<TicketFullDetailsDto>>> GetAllTickets(PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve a ticket by its unique identifier.
        /// </summary>
        Task<GeneralResult<TicketFullDetailsDto>> GetTicketById(int id);

        /// <summary>
        /// Retrieve a ticket associated with a specific consultation.
        /// </summary>
        Task<GeneralResult<TicketFullDetailsDto>> GetTicketByConsultationId(int consultationId);

        /// <summary>
        /// Create a new message within a ticket conversation.
        /// </summary>
        Task<GeneralResult> CreateTicketMessage(TicketMessageCreateDto dto);

        /// <summary>
        /// Update the content of an existing ticket message.
        /// </summary>
        Task<GeneralResult> UpdateTicketMessage(int id, TicketMessageUpdateDto dto);

        /// <summary>
        /// Delete a specific ticket message by its ID.
        /// </summary>
        Task<GeneralResult> DeleteTicketMessage(int id);

        /// <summary>
        /// Retrieve all messages for a given ticket.
        /// </summary>
        Task<GeneralResult<PaginatedResult<TicketMessageDetailsDto>>> GetTicketMessages(int ticketId, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Upload and attach a file to a ticket conversation.
        /// </summary>
        Task<GeneralResult> CreateTicketAttachment(TicketAttachmentCreateDto dto);

        /// <summary>
        /// Delete an attachment from a ticket by its ID and sender ID.
        /// </summary>
        Task<GeneralResult> DeleteTicketAttachment(int id, string userId);

        /// <summary>
        /// Retrieve all attachments for a given ticket.
        /// </summary>
        Task<GeneralResult<PaginatedResult<TicketAttachmentDetailsDto>>> GetTicketAttachments(int ticketId, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve all tickets associated with a specific counselor.
        /// </summary>
        Task<GeneralResult<PaginatedResult<TicketFullDetailsDto>>> GetTicketsByCounselor(int counselorId, PaginationParams pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve all open tickets associated with a specific consultation.
        /// </summary>
        Task<GeneralResult<List<TicketFullDetailsDto>>> GetOpenTicketsForConsultation(int consultationId);

        /// <summary>
        /// Check if a user can access a specific ticket based on their role.
        /// </summary>
        Task<GeneralResult<bool>> CanUserAccessTicket(int ticketId, string userId);
    }
}

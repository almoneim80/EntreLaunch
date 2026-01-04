namespace EntreLaunch.Entities
{
    [Index(nameof(ConsultationId), IsUnique = true)]
    public class ConsultationTicket : SharedData, IBaseEntity
    {
        // Counselor is the one who creates the ticket
        public int CreatorId { get; set; }
        public virtual Counselor Creator { get; set; } = null!;

        public int ConsultationId { get; set; }
        public virtual Consultation Consultation { get; set; } = null!;

        public ConsultationTicketStatus? Status { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        // ticket message
        public virtual ICollection<ConsultationTicketMessage> TicketMessages { get; set; } = new List<ConsultationTicketMessage>();
        // ticket attachments
        public virtual ICollection<ConsultationTicketAttachment> TicketAttachments { get; set; } = new List<ConsultationTicketAttachment>();
    }
}

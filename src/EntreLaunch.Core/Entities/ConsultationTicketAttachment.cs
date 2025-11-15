namespace EntreLaunch.Entities;

public class ConsultationTicketAttachment : SharedData
{
    public int TicketId { get; set; }
    public virtual ConsultationTicket Ticket { get; set; } = null!;

    public string SenderId { get; set; } = null!; // user(clint) or counselor 
    public virtual User Sender { get; set; } = null!;

    public string? Url { get; set; }
    public DateTimeOffset? SendTime { get; set; }
    public bool IsClientMessage { get; set; } = false;
}

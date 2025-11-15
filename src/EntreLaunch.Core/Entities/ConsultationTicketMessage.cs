namespace EntreLaunch.Entities;

public class ConsultationTicketMessage : SharedData
{
    public int TicketId { get; set; }
    public virtual ConsultationTicket Ticket { get; set; } = null!;

    // The sender is either Counselor (in this case Counselor.UserId) or Customer (User.Id)
    public string SenderId { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;
    public bool IsClientMessage { get; set; } = false;
    public string? Content { get; set; }
    public DateTimeOffset? SendTime { get; set; }
}

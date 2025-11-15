namespace EntreLaunch.Entities;

public class Notification : SharedData
{
    public string SenderId { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;

    public string ReciverId { get; set; } = null!;
    public virtual User Reciver { get; set; } = null!;

    public string? Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTimeOffset? ReadAt { get; set; }
}

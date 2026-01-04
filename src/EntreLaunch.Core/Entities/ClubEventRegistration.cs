namespace EntreLaunch.Entities
{
    [Table("club_event_registrations")]
    public class ClubEventRegistration : SharedData
    {
        public string UserId { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        public int EventId { get; set; }
        public virtual ClubEvent Event { get; set; } = null!;

        public DateTimeOffset RegisteredAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsCancelled { get; set; } = false;
        public DateTimeOffset? CancelledAt { get; set; }

        public string? Notes { get; set; }
    }
}

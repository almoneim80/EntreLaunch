namespace EntreLaunch.Entities
{
    public class ClubSubscriber : SharedData
    {
        public string UserId { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public int? EventId { get; set; }
        public virtual ClubEvent? Event { get; set; }
        public DateTimeOffset? SubscriptionDate { get; set; }
        public DateTimeOffset? SubscriptionEnd { get; set; }
        public SubscribeType SubscribeFor { get; set; }
        public bool IsActive { get; set; }
    }
}

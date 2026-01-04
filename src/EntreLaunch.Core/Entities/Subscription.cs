namespace EntreLaunch.Entities
{
    public class Subscription : SharedData
    {
        public string UserId { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        public SubscriptionType Type { get; set; }
        public int? ReferenceId { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        public bool IsAutoRenewal { get; set; }
        public DateTimeOffset? NextRenewalDate { get; set; }
        public int RenewalCount { get; set; }

        public bool IsGifted { get; set; }
        public int? ParentSubscriptionId { get; set; }

        public int? TrialPeriodDays { get; set; }
        public DateTimeOffset? CanceledAt { get; set; }

        public decimal Price { get; set; }
        public SubscriptionStatus Status { get; set; }

        public int? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }

        public string? MetadataJson { get; set; }
    }
}

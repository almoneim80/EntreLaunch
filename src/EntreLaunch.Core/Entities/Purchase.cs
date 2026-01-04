namespace EntreLaunch.Entities
{
    [Table("purchases")]
    public class Purchase : SharedData
    {
        public string UserId { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        public PurchaseItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets points to the related resource (e.g. CourseId, CertificateId, etc.)
        /// </summary>
        public int ReferenceId { get; set; }

        public int PaymentId { get; set; }
        public virtual Payment Payment { get; set; } = null!;

        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets optional JSON metadata (e.g. address, notes, etc.)
        /// </summary>
        public string? MetadataJson { get; set; }

        public bool IsRefunded { get; set; } = false;
        public DateTimeOffset? RefundedAt { get; set; }
    }
}

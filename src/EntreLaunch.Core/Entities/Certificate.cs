namespace EntreLaunch.Entities
{
    public class Certificate : SharedData
    {
        // if it is for training path
        public int? PathId { get; set; }
        public virtual TrainingPath? Path { get; set; }

        // if it is for course
        public int? CourseId { get; set; }
        public virtual Course? Course { get; set; }

        public string? UserId { get; set; }
        public virtual User? User { get; set; }

        public DateTimeOffset? IssuedAt { get; set; }
        public Enums.StudentCertificateType CertificateType { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
        public string? ShippingAddress { get; set; }
        public DateTimeOffset? ExpirationDate { get; set; }

        public string CertificateId { get; set; } = $"EntreLaunch-{Guid.NewGuid():N}".ToUpper();
    }
}

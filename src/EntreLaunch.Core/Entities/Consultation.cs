namespace EntreLaunch.Entities
{
    [Index(nameof(Status))]
    [Index(nameof(Type))]
    public class Consultation : SharedData
    {
        // FK indicates counselor 
        public int CounselorId { get; set; }
        public virtual Counselor Counselor { get; set; } = null!;

        // FK stands for user (client)
        public string ClientId { get; set; } = null!;
        public virtual User Client { get; set; } = null!;

        // Counseling time
        public int? ConsultationTimeId { get; set; }
        public virtual ConsultationTime? ConsultationTime { get; set; }

        public ConsultationType Type { get; set; }
        public ConsultationStatus Status { get; set; }
        public string? Description { get; set; }

        public int? PurchaseId { get; set; }
        public virtual Purchase? Purchase { get; set; }

        // The reminder for this counseling (Relationship 1:1)
        public virtual ConsultationTicket? Ticket { get; set; }
    }
}

namespace EntreLaunch.Entities
{
    [SupportsElastic]
    [SupportsChangeLog]
    [Table("consultation_times")]
    [Index(nameof(DateTimeSlot))]
    public class ConsultationTime : SharedData, IBaseEntity
    {
        // FK indicates the advisor (used in turn)
        public int CounselorId { get; set; }
        public virtual Counselor Counselor { get; set; } = null!;

        [Searchable]
        [Required]
        public DateTimeOffset DateTimeSlot { get; set; }
        public bool IsBooked { get; set; } = false;
        public bool IsRecurringDaily { get; set; }
        public virtual ICollection<Consultation> HistoricalConsultations { get; set; } = new List<Consultation>();
    }
}

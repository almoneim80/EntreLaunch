using EntreLaunch.Interfaces.BaseIntf;

namespace EntreLaunch.Entities
{
    [SupportsElastic]
    [SupportsChangeLog]
    [Table("consultation_times")]
    public class ConsultationTime : SharedData, IBaseEntity
    {
        // FK indicates the advisor (used in turn)
        public int CounselorId { get; set; }
        public virtual Counselor Counselor { get; set; } = null!;

        [Searchable]
        public DateTimeOffset? DateTimeSlot { get; set; }
        public bool IsBooked { get; set; } = false;

        [Searchable]
        public virtual ICollection<Consultation>? BookedConsultations { get; set; }
    }
}

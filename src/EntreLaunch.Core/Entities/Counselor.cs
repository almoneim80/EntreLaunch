namespace EntreLaunch.Entities
{
    public class Counselor : SharedData
    {
        public string UserId { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        public string? Qualification { get; set; }
        public string? City { get; set; }
        public int SpecializationExperience { get; set; }
        public int ConsultingExperience { get; set; }
        public int DailyHours { get; set; }
        public Dictionary<string, string>? SocialMediaAccounts { get; set; }
        public CounselorRequesttStatus Status { get; set; } = CounselorRequesttStatus.Pending;
        public bool Active { get; set; }
        // A single counselor has multiple counseling times
        public virtual ICollection<ConsultationTime>? ConsultationTimes { get; set; }
        // A single consultant has multiple consultations
        public virtual ICollection<Consultation>? Consultations { get; set; }
        // A single advisor may create multiple tickets
        public virtual ICollection<ConsultationTicket>? Tickets { get; set; }
    }
}

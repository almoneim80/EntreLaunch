namespace EntreLaunch.Entities
{
    public class TrainingPath : SharedData
    {
        public string? Name { get; set; }
        public decimal Price { get; set; } = 0;
        public decimal Discount { get; set; } = 0;
        public string? Description { get; set; }
        public bool CertificateExists { get; set; }
        public int CertificateValidityInDays { get; set; } = 365;
        public int? MaxEnrollment { get; set; } = 0;
        public bool IsFree { get; set; } = false;

        public virtual ICollection<Course>? Courses { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace EntreLaunch.Entities
{
    [Table("email_schedule")]
    [SupportsChangeLog]
    public class EmailSchedule : BaseEntity
    {
        /// <summary>
        /// Gets or sets the JSON based schedule assigned for the email groEntreLaunch. Examples: {"Cron": "0 0 14 ? ? TUE,THU"}, {"Day": "5,14", "Time": "14.00"}, {"Immediately": "true","Delay": "15"}.
        /// </summary>
        [Required]
        public string Schedule { get; set; } = string.Empty;

        [Required]
        public int GroEntreLaunchId { get; set; }

        [JsonIgnore]
        [ForeignKey("GroEntreLaunchId")]
        public virtual EmailGroup? GroEntreLaunch { get; set; }
    }
}

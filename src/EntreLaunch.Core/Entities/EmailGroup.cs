using System.Text.Json.Serialization;

namespace EntreLaunch.Entities
{
    [Table("email_group")]
    [SupportsChangeLog]
    public class EmailGroup : SharedData
    {
        /// <summary>
        /// Gets or sets the name of the email group.
        /// </summary>
        [Required]
        [Searchable]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Searchable]
        public string Language { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<EmailTemplate>? EmailTemplates { get; set; }
    }
}

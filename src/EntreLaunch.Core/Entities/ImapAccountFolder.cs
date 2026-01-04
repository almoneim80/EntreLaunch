using System.Text.Json.Serialization;

namespace EntreLaunch.Entities;

[Table("imap_account_folder")]
[SupportsChangeLog]
public class ImapAccountFolder : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public int LastUid { get; set; }

    [Required]
    public int ImapAccountId { get; set; }

    [JsonIgnore]
    [ForeignKey("ImapAccountId")]
    public virtual ImapAccount? ImapAccount { get; set; }
}

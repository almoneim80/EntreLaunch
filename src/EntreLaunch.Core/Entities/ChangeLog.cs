using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EntreLaunch.Entities
{
    [Table("change_log")]
    public class ChangeLog : BaseEntityWithId, IHasCreatedAt
    {
        public string ObjectType { get; set; } = string.Empty;

        public int ObjectId { get; set; }

        public EntityState EntityState { get; set; }
        public string Data { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}

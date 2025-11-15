using EntreLaunch.Interfaces.BaseIntf;

namespace EntreLaunch.Entities
{
    [SupportsElastic]
    [SupportsChangeLog]
    [Table("club_events")]
    public class ClubEvent : SharedData, IBaseEntity
    {
        [Searchable]
        public string? Name { get; set; }
        [Searchable]
        public string? City { get; set; }
        [Searchable]
        public string? Description { get; set; }
        [Searchable]
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        [Searchable]
        public virtual ICollection<ClubSubscriber>? ClubEventSubscribers { get; set; }
    }
}

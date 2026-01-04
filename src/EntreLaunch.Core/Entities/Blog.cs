#nullable disable
namespace EntreLaunch.Entities
{
    public class Blog : SharedData
    {
        public string UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public string Title { get; set; }
        public string Details { get; set; }
        public string Media { get; set; }

        public BlogStatus Status { get; set; }
    }
}

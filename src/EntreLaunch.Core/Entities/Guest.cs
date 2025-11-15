namespace EntreLaunch.Entities
{
    public class Guest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public int? SharesCount { get; set; }
        public string? Email { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? EntreLaunchdatedAt { get; set; }
    }
}

namespace EntreLaunch.Entities
{
    public class WheelPlayerState : SharedData
    {
        public string PlayerId { get; set; } = null!;
        public DateTimeOffset Date { get; set; }

        public bool HasUsedFreeSpin { get; set; }
        public bool AllowPaidSpin { get; set; }
    }
}

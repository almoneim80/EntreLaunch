namespace EntreLaunch.Entities
{
    public class SimulationAdLike : SharedData
    {
        public int AdId { get; set; }
        public virtual SimulationAdvertisement? Advertisement { get; set; }

        public string? GuestId { get; set; }
        public virtual Guest? Guest { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

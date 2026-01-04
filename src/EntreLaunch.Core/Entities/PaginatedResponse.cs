namespace EntreLaunch.Entities
{
    public class PaginatedResponse<T>
    {
        public int TotalCount { get; set; }

        public List<T> Data { get; set; } = new();
    }
}

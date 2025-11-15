namespace EntreLaunch.Plugin.SmartExam.Helpers
{
    public static class ListExtensions
    {
        private static readonly Random _random = new Random();

        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}

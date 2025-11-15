namespace EntreLaunch.Interfaces
{
    public interface IVariablesService
    {
        public Dictionary<string, string> GetVariables(string language);
    }
}

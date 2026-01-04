namespace EntreLaunch.Interfaces.ImportIntf
{
    public interface IHasNestedImports
    {
        /// <summary>
        /// Returns the nested imports of the object.
        /// </summary>
        IEnumerable<object?> GetNestedImports();
    }
}

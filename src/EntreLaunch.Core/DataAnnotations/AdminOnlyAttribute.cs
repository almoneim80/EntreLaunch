namespace EntreLaunch.DataAnnotations
{
    /// <summary>
    /// This attribute marks a permission as Admin-only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AdminOnlyAttribute : Attribute
    {
        // just an empty Attribute
    }
}

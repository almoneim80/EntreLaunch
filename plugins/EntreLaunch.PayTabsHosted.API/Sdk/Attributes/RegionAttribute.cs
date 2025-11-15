namespace EntreLaunch.PayTabsHosted.API.Attributes
{
    /// <summary>
    /// Defines an Attribute that is used to add descriptive information about a Region.
    /// </summary>
    // can't limit to enum fields, so using field as best workaround
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)] 
    public class RegionAttribute : Attribute
    {
        public RegionAttribute(string shortform, string domain)
        {
            ShortForm = shortform;
            Domain = domain;
        }

        public string ShortForm { get; }

        public string Domain { get; }
    }
}

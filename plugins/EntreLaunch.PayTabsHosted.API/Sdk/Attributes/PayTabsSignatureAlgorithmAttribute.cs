namespace EntreLaunch.PayTabsHosted.API.Attributes
{
    /// <summary>
    /// Defines an Attribute that is used to specify information about the signature algorithms sEntreLaunchported by the system.
    /// </summary>
    // can't limit to enum fields, so using field as best workaround.
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PayTabsSignatureAlgorithmAttribute : Attribute
    {
        public PayTabsSignatureAlgorithmAttribute(string name, int saltLength)
        {
            Name = name;
            SaltLength = saltLength;
        }

        public string Name { get; }

        public int SaltLength { get; }
    }
}

using EntreLaunch.PayTabsHosted.API.Attributes;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API
{
    /// <summary>
    /// Adds extensions to the Region type.
    /// </summary>
    public static class RegionExtensions
    {
        private static RegionAttribute GetRegionAttribute(Region region)
        {
            var enumType = typeof(Region);
            var memberInfos = enumType.GetMember(region.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);

            if (enumValueMemberInfo == null)
            {
                throw new ArgumentException($"Invalid region: {region}", nameof(region));
            }

            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(RegionAttribute), false);
            var attribute = (RegionAttribute)valueAttributes[0];

            return attribute;
        }

        /// <summary>
        /// Returns the PayTabs defined shortform for the region, e.g. 'eu' for 'Europe'.
        /// </summary>
        public static string ToShortform(this Region region)
        {
            var attribute = GetRegionAttribute(region);
            return attribute.ShortForm;
        }

        /// <summary>
        /// Returns the API endpoint domain for the given region.
        /// </summary>
        public static string ToDomain(this Region region)
        {
            var attribute = GetRegionAttribute(region);
            return attribute.Domain;
        }
    }
}

using Nest;
using EntreLaunch.PayTabsHosted.API.Attributes;
using EntreLaunch.PayTabsHosted.API.Types;

namespace EntreLaunch.PayTabsHosted.API
{
    /// <summary>
    /// Adds extensions to the PayTabsSignatureAlgorithm type to enable:
    /// Get the name of the signature algorithm(GetName).
    /// Get the length of the Salt used in the algorithm(GetSaltLength).
    /// </summary>
    public static class AmazonSignatureAlgorithmExtensions
    {
        private static PayTabsSignatureAlgorithmAttribute GetAmazonSignatureAlgorithmAttribute(PayTabsSignatureAlgorithm algorithm)
        {
            var enumType = typeof(PayTabsSignatureAlgorithm);
            var memberInfos = enumType.GetMember(algorithm.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo?.GetCustomAttributes(typeof(PayTabsSignatureAlgorithmAttribute), false);
            if (valueAttributes == null)
            {
                throw new InvalidOperationException("No attributes found");
            }

            var attribute = (PayTabsSignatureAlgorithmAttribute)valueAttributes[0];
            return attribute;
        }

        /// <summary>
        /// Returns the name of the Amazon signature Algorithm, "AMZN-PAY-RSASSA-PSS-V2" for V2 and "AMZN-PAY-RSASSA-PSS" for Default.
        /// </summary>
        public static string GetName(this PayTabsSignatureAlgorithm algorithm)
        {
            var attribute = GetAmazonSignatureAlgorithmAttribute(algorithm);
            return attribute.Name;
        }

        /// <summary>
        /// Returns the salt length of the Amazon signature Algorithm, 32 for V2 and 20 for Default.
        /// </summary>
        public static int GetSaltLength(this PayTabsSignatureAlgorithm algorithm)
        {
            var attribute = GetAmazonSignatureAlgorithmAttribute(algorithm);
            return attribute.SaltLength;
        }
    }
}

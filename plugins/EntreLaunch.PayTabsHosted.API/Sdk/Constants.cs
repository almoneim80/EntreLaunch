using System.Collections.ObjectModel;

namespace EntreLaunch.PayTabsHosted.API
{
    public class Constants
    {
        public const string SdkVersion = "1.0.0.0";
        public const string SdkName = "paytabs-getway-api-sdk-dotnet";
        public const int ApiVersion = 2;

        public static readonly ReadOnlyDictionary<string, int> ServiceErrors = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>()
        {
            { "Internal Server Error", 500 },
            { "HTTP Bad Gateway", 502 },
            { "Service Unavailable", 503 },
            { "HTTP Gateway Timeout", 504 },
            { "Request Timeout", 408 },
            { "Too Many Requests", 429 }
        });

        public class Headers
        {
            public const string AuthorizationKey = "authorization";
            public const string ClientKey = "client-key";
            public const string RequestId = "pay-request-id";
            public const string AuthToken = "authtoken";
            public const string Region = "region";
            public const string Date = "pay-date";
            public const string Host = "pay-host";
        }

        public class ApiServices
        {
            public const string Default = "";
            public const string REQUEST = "request"; // on purpose (default is the 'root' API)
            public const string QUERY = "query";
            public const string INQUIRYVALU = "info/valu";
            public const string InStore = "in-store";
        }

        public class Resources
        {
            public class WebStore
            {
                public const string Default = "";
                public const string Page = "page";
                public const string CheckoutSessions = "checkoutSessions";
                public const string ChargePermissions = "chargePermissions";
                public const string Charges = "charges";
                public const string Refunds = "refunds";
                public const string Buyer = "buyers";

                // CV2 Reporting API Constants
                public const string Reports = "reports";
                public const string ReportDocuments = "report-documents";
                public const string ReportSchedules = "report-schedules";

                // Merchant Account Management API Constants
                public const string AccountManagement = "merchantAccounts";
            }

            public class InStore
            {
                public const string MerchantScan = "merchantScan";
                public const string Refund = "refund";
                public const string Charge = "charge";
            }

            public const string DeliveryTracker = "deliveryTrackers";
            public const string Token = "token";
            public const string INQUIRYVALU = "inquiry";
            public const string Request = "request";
            public const string Invoice = "invoice";
        }

        public class Methods
        {
            internal const string Query = "query";
            internal const string Close = "close";
            internal const string Capture = "capture";
            internal const string Delete = "delete";
            internal const string Cancel = "cancel";
            internal const string Complete = "complete";
            internal const string Finalize = "finalize";
        }
    }
}

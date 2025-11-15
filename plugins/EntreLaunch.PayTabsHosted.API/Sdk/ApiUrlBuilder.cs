namespace EntreLaunch.PayTabsHosted.API
{
    public class ApiUrlBuilder
    {
        private readonly ApiConfiguration _config;

        public ApiUrlBuilder(ApiConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Build the full API path including the base URL and the API endpoint URL parts.
        /// Purpose: Used to create a complete link containing:
        /// Base URL.
        /// Specific Services/Resources.
        /// Identifiers or Methods required.
        /// </summary>
        public Uri BuildFullApiPath(string service = "", string resource = "", string resourceIdentifier = "", string mehod = "")
        {
            string url = GetApiEndPointBaseUrl().ToString(); // example: https://merchant.paytabs.com/

            if (!string.IsNullOrEmpty(service))
            {
                url += $"{service}/"; // example: https://secure-egypt.PayTabs.com/payment/query/
            }

            if (!string.IsNullOrEmpty(resource))
            {
                url += $"{resource}/"; // example: https://secure-egypt.PayTabs.com/payment/query/delete
            }

            if (!string.IsNullOrEmpty(resourceIdentifier))
            {
                url += $"{resourceIdentifier}/"; // example: https://secure-egypt.PayTabs.com/payment/query/checkoutSessions/08eca0c9-214c-4246-a42f-af408861ce20
            }

            if (!string.IsNullOrEmpty(mehod))
            {
                url += $"{mehod}/"; // example: https://secure-egypt.PayTabs.com/payment/query/delete
            }

            return new Uri(url);
        }

        //public Uri BuildFullApiPath(string service = "", string resource = "", string resourceIdentifier = "")
        //{
        //    string url = GetApiEndPointBaseUrl().ToString(); // example: https://merchant.paytabs.com/

        //    if (!string.IsNullOrEmpty(service))
        //    {
        //        url += $"{service}/"; // example: https://secure-egypt.PayTabs.com/payment/query/
        //    }

        //    if (!string.IsNullOrEmpty(resource))
        //    {
        //        url += $"{resource}/"; // example: https://secure-egypt.PayTabs.com/payment/query/delete
        //    }

        //    if (!string.IsNullOrEmpty(resourceIdentifier))
        //    {
        //        url += $"{resourceIdentifier}/"; // example: https://secure-egypt.PayTabs.com/payment/query/checkoutSessions/08eca0c9-214c-4246-a42f-af408861ce20
        //    }

        //    return new Uri(url);
        //}

        /// <summary>
        /// Build the full Merchant URL including the base URL and the Merchant endpoint URL parts.
        /// Purpose: Used to create Merchant-specific links, such as managing invoices or pages.
        /// Example: If you are working on an invoice, the link may produce:
        /// https://merchant.paytabs.com/payment/invoice/.
        /// </summary>
        public Uri BuildFullMerchantUrl(string service = "", string resource = "", string resourceIdentifier = "")
        {
            string url = GetMerchantBaseUrl().ToString(); // example: https://merchant.paytabs.com/

            if (!string.IsNullOrEmpty(service) && service == "invoice")
            {
                url += $"invoice/"; // example: https://merchant.PayTabs.com/payment/invoice/
            }
            else if (!string.IsNullOrEmpty(service) && service == "page")
            {
                url += $"page/"; // example: https://merchant.PayTabs.com/payment/page/
            }
            else
            {
                url += $"request/"; // example: https://secure-egypt.PayTabs.com/payment/request/
            }
            if (!string.IsNullOrEmpty(resource))
            {
                url += $"{resource}/"; // example: https://merchant.PayTabs.com/payment/request/checkoutSessions/
            }

            if (!string.IsNullOrEmpty(resourceIdentifier))
            {
                url += $"{resourceIdentifier}/"; // example: https://merchant.PayTabs.com/payment/request/checkoutSessions/08eca0c9-214c-4246-a42f-af408861ce20
            }

            return new Uri(url);
        }

        /// <summary>
        /// Get the base URL for the API, e.g. the part within this application's execution that is static.
        /// Purpose: Returns the Base URL of PayTabs according to the Region specified in the settings.
        /// Example: If the region is EGY, the URL is:
        /// https://secure-egypt.paytabs.com/payment/.
        /// </summary>
        public Uri GetApiEndPointBaseUrl()
        {
            string regionDomain = _config.Region.ToDomain();
            //string environment = _config.Environment.ToString().ToLower();
            string serviceURL;

            serviceURL = regionDomain switch
            {
                "SAU" => $"https://{regionDomain}/payment/", //"https://secure.paytabs.sa",
                "ARE" => $"https://{regionDomain}/payment/", //"https://secure.paytabs.com",
                "EGY" => $"https://{regionDomain}/payment/", //"https://secure-egypt.paytabs.com",
                "OMN" => $"https://{regionDomain}/payment/", //"https://secure-oman.paytabs.com",
                "JOR" => $"https://{regionDomain}/payment/", //"https://secure-jordan.paytabs.com",
                "IRQ" => $"https://{regionDomain}/payment/", //"https://secure-iraq.paytabs.com",
                "GLOBAL" => $"https://{regionDomain}/payment/", //"https://secure-iraq.paytabs.com",
                _ => "https://secure-global.paytabs.com/",
            };
            return new Uri(serviceURL);
        }

        /// <summary>
        /// Get the base URL for the API, e.g. the part within this application's execution that is static.
        /// Purpose: Returns the Merchant Base URL according to the region.
        /// Example: If the region is ARE, the URL is:
        /// https://merchant.paytabs.com/.
        /// </summary>
        public Uri GetMerchantBaseUrl()
        {
            string regionDomain = _config.Region.ToDomain();
            string serviceURL;

            serviceURL = regionDomain switch
            {
                "SAU" => "https://merchant.paytabs.sa/",
                "ARE" => "https://merchant.paytabs.com/",
                "EGY" => "https://merchant-egypt.paytabs.com/",
                "OMN" => "https://merchant-oman.paytabs.com/",
                "JOR" => "https://merchant-jordan.paytabs.com/",
                "IRQ" => "https://merchant-iraq.paytabs.com/",
                "GLOBAL" => "https://merchant-iraq.paytabs.com/",
                _ => "https://merchant-global.paytabs.com/",
            };
            return new Uri(serviceURL);
        }
    }
}

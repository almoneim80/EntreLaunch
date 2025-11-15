using System;
using System.Collections.Generic;

namespace EntreLaunch.PayTabsHosted.API.Types
{
    /// <summary>
    /// Represents an API request, containing the URL, method (GET/POST), headers, and data sent.
    /// </summary>
#nullable disable
    // TODO: add unit tests
    public class ApiRequest
    {
        private ApiRequestBody body;

        public ApiRequest(Uri path, PayHttpMethod method)
        {
            Headers = new Dictionary<string, string>();
            QueryParameters = new Dictionary<string, List<string>>();
            Parameters = new Dictionary<string, string>();

            Path = path;
            HttpMethod = method;
        }

        public ApiRequest(Uri path, PayHttpMethod method, ApiRequestBody body, Dictionary<string, string> headers)
            : this(path, method)
        {
            Body = body;
            Headers = headers;
        }

        public Uri Path { get; set; }

        public PayHttpMethod HttpMethod { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public Dictionary<string, List<string>> QueryParameters { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public ApiRequestBody Body
        {
            get => body;
            set
            {
                body = value;
                #pragma warning disable CS0618
                BodyAsJsonString = body.ToJson();
                #pragma warning restore CS0618
            }
        }
        public string BodyAsJsonString { get; private set; }
    }
}

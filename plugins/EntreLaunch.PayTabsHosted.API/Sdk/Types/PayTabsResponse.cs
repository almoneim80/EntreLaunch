using System;
using System.Collections.Generic;

namespace EntreLaunch.PayTabsHosted.API.Types
{
#nullable disable
    // TODO: rename file to PayTabsResponse
    // TODO: introduce strong-typed fields for 'reasonCode' and 'message' (response of erroneous API calls)

    /// <summary>
    /// Response object for API calls.
    /// Represents the request response with information such as status, code, and headers.
    /// </summary>
    public class PayTabsResponse
    {
        public Uri Url { get; internal set; }

        public PayHttpMethod Method { get; internal set; }

        public string RawRequest { get; internal set; }

        public string RawResponse { get; internal set; }

        public string RequestId { get; internal set; }

        public string Status { get; internal set; }

        public int Code { get; internal set; }

        public int Retries { get; internal set; }

        public long Duration { get; internal set; }

        public bool Success
        {
            get
            {
                if (Code >= 200 && Code <= 299 ) return true;
                else return false;
            }
        }

        public Dictionary<string, string> Headers { get; internal set; }
    }
}

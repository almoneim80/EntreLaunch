using Newtonsoft.Json;
using System.Runtime.Serialization;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Invoice
{
    /// <summary>
    /// To create a new invoice that includes payment methods, customer details, and shipping.
    /// </summary>
#nullable disable
    public class CreateInvoiceRequest : CreateInvoiceBase
    {
        [OnSerializing]
        internal void OnSerializing(StreamingContext content)
        {
            if (string.IsNullOrEmpty(CustomerRef))
            {
                // skip 'CustomerDetails' if there wasn't provided anything
                if (CustomerDetails?.Name == null && CustomerDetails?.Email == null && CustomerDetails?.PhoneNumber == null && CustomerDetails?.Country == null && CustomerDetails?.State == null && CustomerDetails?.City == null)
                {
                    CustomerDetails = null;
                }
            }
            else
            {
                CustomerDetails = null;
            }
        }

        [OnSerialized]
        internal void OnSerialized(StreamingContext content)
        {
            if (string.IsNullOrEmpty(CustomerRef))
            {
                if (CustomerDetails == null)
                {
                    CustomerDetails = new CustomerDetails();
                }
            }
        }

        [JsonProperty("payment_methods")]
        public string[] PaymentMethods { get; set; }

        [JsonProperty("customer_ref")]
        public string CustomerRef { get; set; }

        [JsonProperty("customer_details")]
        public CustomerDetails CustomerDetails { get; set; }

        [JsonProperty("hide_shipping")]
        public bool HideShipping { get; set; }

        [JsonProperty("invoice")]
        public InvoiceData Invoice { get; set; }

        [JsonProperty("callback")]
        public string Callback { get; set; }

        [JsonProperty("return")]
        public string Return { get; set; }
    }
}

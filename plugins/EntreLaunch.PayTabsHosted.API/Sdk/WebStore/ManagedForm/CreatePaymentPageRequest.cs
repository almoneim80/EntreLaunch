using Newtonsoft.Json;
using System.Runtime.Serialization;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.ManagedForm
{
    /// <summary>
    /// Represents a request to create a managed payment page.
    /// </summary>
#nullable disable
    public class CreatePaymentPageRequest : CreatePaymentBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePaymentPageRequest"/> class.
        /// Create Payment Page - General.
        /// </summary>
        public CreatePaymentPageRequest(CreatePaymentBase createPaymentBase, string payPageLang, CustomerDetails customerDetails, ShippingDetails shippingDetails, string callbackUrl, string returnUrl)
        {
            ProfileId = createPaymentBase.ProfileId;
            TransactionType = createPaymentBase.TransactionType;
            TransactionClass = createPaymentBase.TransactionClass;
            CartId = createPaymentBase.CartId;
            CartCurrency = createPaymentBase.CartCurrency;
            CartAmount = createPaymentBase.CartAmount;
            CartDescription = createPaymentBase.CartDescription;

            PayPageLang = payPageLang;
            CustomerDetails = customerDetails;
            CallbackUrl = callbackUrl;
            ReturnUrl = returnUrl;
            ShippingDetails = shippingDetails;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePaymentPageRequest"/> class.
        /// Create Payment Page - General with payment methods.
        /// </summary>
        public CreatePaymentPageRequest(CreatePaymentBase createPaymentBase, string[] paymentMethods, string payPageLang, CustomerDetails customerDetails, ShippingDetails shippingDetails, string callbackUrl, string returnUrl)
        {
            ProfileId = createPaymentBase.ProfileId;
            TransactionType = createPaymentBase.TransactionType;
            TransactionClass = createPaymentBase.TransactionClass;
            CartId = createPaymentBase.CartId;
            CartCurrency = createPaymentBase.CartCurrency;
            CartAmount = createPaymentBase.CartAmount;
            CartDescription = createPaymentBase.CartDescription;

            PaymentMethods = paymentMethods;
            PayPageLang = payPageLang;
            CustomerDetails = customerDetails;
            CallbackUrl = callbackUrl;
            ReturnUrl = returnUrl;
            ShippingDetails = shippingDetails;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePaymentPageRequest"/> class.
        /// Create Payment Page - with InvoiceData.
        /// </summary>
        public CreatePaymentPageRequest(CreatePaymentBase createPaymentBase, string[] paymentMethods, string payPageLang, CustomerDetails customerDetails, string callbackUrl, string returnUrl, bool hideShipping, string customerRef, InvoiceData invoice)
        {
            ProfileId = createPaymentBase.ProfileId;
            TransactionType = createPaymentBase.TransactionType;
            TransactionClass = createPaymentBase.TransactionClass;
            CartId = createPaymentBase.CartId;
            CartCurrency = createPaymentBase.CartCurrency;
            CartAmount = createPaymentBase.CartAmount;
            CartDescription = createPaymentBase.CartDescription;

            PaymentMethods = paymentMethods;
            PayPageLang = payPageLang;
            CustomerDetails = customerDetails;
            CallbackUrl = callbackUrl;
            ReturnUrl = returnUrl;
            HideShipping = hideShipping;
            CustomerRef = customerRef;
            Invoice = invoice;
        }


        [OnSerializing]
        internal void OnSerializing(StreamingContext content)
        {
            // skip 'CustomerDetails' if there wasn't provided anything
            if (CustomerDetails?.Name == null && CustomerDetails?.Email == null && CustomerDetails?.PhoneNumber == null && CustomerDetails?.Country == null && CustomerDetails?.State == null && CustomerDetails?.City == null )
            {
                CustomerDetails = null;
            }

            if (HideShipping)
            {
                ShippingDetails = null;
            }

            if (!WithInvoice)
            {
                Invoice = null;
            }
            if (TokenInfo?.Tokenise == null)
            {
                TokenInfo = null;
            }
        }

        [OnSerialized]
        internal void OnSerialized(StreamingContext content)
        {
            if (CustomerDetails == null)
            {
                CustomerDetails = new CustomerDetails();
            }
            if (HideShipping)
            {
                ShippingDetails = null;
            }
            else if(!HideShipping && ShippingDetails == null)
            {
                ShippingDetails = new ShippingDetails();
            }

            if (!WithInvoice)
            {
                Invoice = null;
            }
            else if (WithInvoice && Invoice == null)
            {
                Invoice = new InvoiceData();
            }
        }

        [JsonProperty("payment_methods")]
        public string[] PaymentMethods { get; set; }

        [JsonProperty("paypage_lang")]
        public string PayPageLang { get; set; }

        [JsonProperty("customer_details")]
        public CustomerDetails CustomerDetails { get; set; }

        [JsonProperty("callback")]
        public string CallbackUrl { get; set; }

        [JsonProperty("Country")]
        public string ReturnUrl { get; set; }

        [JsonProperty("hide_shipping")]
        public bool HideShipping { get; set; }

        [JsonProperty("shipping_details")]
        public ShippingDetails ShippingDetails { get; set; }

        [JsonIgnore]
        public bool WithInvoice { get; set; } = false;

        [JsonProperty("customer_ref")]
        public string CustomerRef { get; set; }

        [JsonProperty("invoice")]
        public InvoiceData Invoice { get; set; }

        [JsonProperty("card_discounts")]
        public string CardDiscounts { get; set; }

        [JsonProperty("card_filter")]
        public string CardFilter { get; set; }

        [JsonProperty("card_filter_title")]
        public string CardFilterTitle { get; set; }

        [JsonProperty("tokenise")]
        public int Tokenise { get; set; }

        [JsonProperty("show_save_card")]
        public string ShowSaveCard { get; set; }

        [JsonProperty("donation_mode")]
        public bool DonationMode { get; set; }

        [JsonProperty("cart_min")]
        public decimal CartMin { get; set; }

        [JsonProperty("cart_min")]
        public decimal CartMax { get; set; }

        [JsonProperty("framed")]
        public bool Framed { get; set; }

        [JsonProperty("framed_return_top")]
        public bool FramedReturnTop { get; set; }

        [JsonProperty("framed_return_parent")]
        public string FramedReturnParent { get; set; }

        [JsonProperty("force_full_ui")]
        public string ForceFullUi { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("config_id")]
        public int ConfigId { get; set; }

        /// <summary>
        /// Gets or sets create Payment Page - with Alternate currency.
        /// </summary>
        [JsonProperty("alt_currency")]
        public string AltCurrency { get; set; }

        [JsonProperty("token_info")]
        public TokenInfo TokenInfo { get; set; }
    }
}

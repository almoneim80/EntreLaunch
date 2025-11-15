using Newtonsoft.Json;
using System.Runtime.Serialization;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.HostedPage
{
#nullable disable
    /// <summary>
    /// this use to 
    /// CreatePaymentPageTabby,CreatePaymentPageTamara,CreatePaymentPagePayPal,CreatePaymentPageAman,CreatePaymentPageHalan.
    /// Create a new hosted payment page, with sEntreLaunchport for payment methods, invoice and shipping details.
    /// </summary>
    public class CreateHostedPaymentRequest : CreatePaymentBase
    {
        public CreateHostedPaymentRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateHostedPaymentRequest"/> class.
        /// Create Payment Page - General.
        /// </summary>
        public CreateHostedPaymentRequest(CreatePaymentBase createPaymentBase, string payPageLang, CustomerDetails customerDetails, ShippingDetails shippingDetails, string callbackUrl, string returnUrl)
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
        /// Initializes a new instance of the <see cref="CreateHostedPaymentRequest"/> class.
        /// Create Payment Page - General with payment methods.
        /// </summary>
        public CreateHostedPaymentRequest(CreatePaymentBase createPaymentBase, string[] paymentMethods, string payPageLang, CustomerDetails customerDetails, ShippingDetails shippingDetails, string callbackUrl, string returnUrl)
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
        /// Initializes a new instance of the <see cref="CreateHostedPaymentRequest"/> class.
        /// Create Payment Page - with InvoiceData.
        /// </summary>
        public CreateHostedPaymentRequest(CreatePaymentBase createPaymentBase, string[] paymentMethods, string payPageLang, CustomerDetails customerDetails, string callbackUrl, string returnUrl, bool hideShipping, string customerRef, InvoiceData invoice)
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

        [JsonProperty("payment_methods", NullValueHandling = NullValueHandling.Ignore)]
        public string[] PaymentMethods { get; set; }

        [JsonProperty("paypage_lang", NullValueHandling = NullValueHandling.Ignore)]
        public string PayPageLang { get; set; }

        [JsonProperty("return", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ReturnUrl { get; set; }

        [JsonProperty("callback", NullValueHandling = NullValueHandling.Ignore)]
        public string CallbackUrl { get; set; }

        [JsonProperty("customer_ref", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomerRef { get; set; }

        [JsonProperty("customer_details", NullValueHandling = NullValueHandling.Ignore)]
        public CustomerDetails CustomerDetails { get; set; }

        [JsonProperty("hide_shipping", NullValueHandling = NullValueHandling.Ignore)]
        public bool HideShipping { get; set; }

        [JsonProperty("shipping_details", NullValueHandling = NullValueHandling.Ignore)]
        public ShippingDetails ShippingDetails { get; set; }

        [JsonIgnore]
        public bool WithInvoice { get; set; } = false;

        [JsonProperty("invoice", NullValueHandling = NullValueHandling.Ignore)]
        public InvoiceData Invoice { get; set; }

        [JsonProperty("user_defined", NullValueHandling = NullValueHandling.Ignore)]
        public UserDefined UserDefined { get; set; }

        [JsonProperty("card_discounts", NullValueHandling = NullValueHandling.Ignore)]
        public List<CardDiscounts> CardDiscounts { get; set; }

        [JsonProperty("config_id", NullValueHandling = NullValueHandling.Ignore)]
        public int ConfigId { get; set; }

        [JsonProperty("card_filter", NullValueHandling = NullValueHandling.Ignore)]
        public string CardFilter { get; set; }

        [JsonProperty("card_filter_title", NullValueHandling = NullValueHandling.Ignore)]
        public string CardFilterTitle { get; set; }

        [JsonProperty("tokenise", NullValueHandling = NullValueHandling.Ignore)]
        public int Tokenise { get; set; }

        [JsonProperty("show_save_card", NullValueHandling = NullValueHandling.Ignore)]
        public bool ShowSaveCard { get; set; }

        [JsonProperty("donation_mode", NullValueHandling = NullValueHandling.Ignore)]
        public bool DonationMode { get; set; }

        [JsonProperty("cart_min", NullValueHandling = NullValueHandling.Ignore)]
        public int CartMin { get; set; }

        [JsonProperty("cart_max", NullValueHandling = NullValueHandling.Ignore)]
        public int CartMax { get; set; }

        [JsonProperty("framed", NullValueHandling = NullValueHandling.Ignore)]
        public bool Framed { get; set; }

        [JsonProperty("framed_return_top", NullValueHandling = NullValueHandling.Ignore)]
        public bool FramedReturnTop { get; set; }

        [JsonProperty("framed_return_parent", NullValueHandling = NullValueHandling.Ignore)]
        public bool FramedReturnParent { get; set; }

        [JsonProperty("framed_message_target", NullValueHandling = NullValueHandling.Ignore)]
        public Uri FramedMessageTarget { get; set; }

        [JsonProperty("force_full_ui", NullValueHandling = NullValueHandling.Ignore)]
        public string ForceFullUi { get; set; }

        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }

        [JsonProperty("alt_currency", NullValueHandling = NullValueHandling.Ignore)]
        public string AltCurrency { get; set; }

        [JsonProperty("token_info", NullValueHandling = NullValueHandling.Ignore)]
        public TokenInfo TokenInfo { get; set; }
    }
}

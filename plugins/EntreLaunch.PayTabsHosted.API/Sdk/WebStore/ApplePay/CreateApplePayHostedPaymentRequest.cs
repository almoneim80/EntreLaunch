using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.ApplePay
{
#nullable disable
    public class CreateApplePayHostedPaymentRequest : ApiRequestBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateApplePayHostedPaymentRequest"/> class.
        /// Creates a new ApplePay hosted payment request.
        /// </summary>
        public CreateApplePayHostedPaymentRequest(int profileId, string transactionType, string region, string transactionClass, string cartId, string cartCurrency, decimal cartAmount, string cartDescription)
        {
            ProfileId = profileId;
            TransactionType = transactionType;
            TransactionClass = transactionClass;
            CartId = cartId;
            CartCurrency = cartCurrency;
            CartAmount = cartAmount;
            CartDescription = cartDescription;
        }

        /// <summary>
        /// Gets or sets the merchant Profile ID you can get from your PayTabs dashboard.
        /// </summary>
        [JsonRequired]
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonRequired]
        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonRequired]
        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [JsonRequired]
        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonRequired]
        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonRequired]
        [JsonProperty("cart_amount")]
        public decimal CartAmount { get; set; }

        [JsonRequired]
        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }

        [JsonProperty("payment_methods")]
        public string[] PaymentMethods { get; set; } = new string[] { "applepay" };
        [JsonRequired]

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

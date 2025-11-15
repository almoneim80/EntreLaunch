using Newtonsoft.Json;
using EntreLaunch.PayTabsHosted.API.Types;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.WebStore.HostedPage
{
    /// <summary>
    /// Edit an existing hosted payment page with EntreLaunchdated basket and customer details.
    /// </summary>
#nullable disable
    public class EntreLaunchdateHostedPaymentPageRequest : ApiRequestBody
    {
        /// <summary>
        /// Gets or sets the merchant Profile ID you can get from your PayTabs dashboard.
        /// </summary>
        [JsonProperty("profile_id")]
        public int ProfileId { get; set; }

        [JsonProperty("tran_ref")]
        public string TransactionReference { get; set; }

        [JsonProperty("tran_type")]
        public string TransactionType { get; set; }

        [JsonProperty("tran_class")]
        public string TransactionClass { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("cart_currency")]
        public string CartCurrency { get; set; }

        [JsonProperty("cart_amount")]
        public decimal CartAmount { get; set; }

        [JsonProperty("cart_description")]
        public string CartDescription { get; set; }


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

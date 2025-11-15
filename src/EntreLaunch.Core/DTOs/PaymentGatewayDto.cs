using Newtonsoft.Json;

namespace EntreLaunch.DTOs;

public class PayTabsPayLinkRequest
{
    [JsonProperty("profile_id")]
    public string ProfileId { get; set; } = null!;

    [JsonProperty("tran_type")]
    public string TranType { get; set; } = "EntreLaunch payments";

    [JsonProperty("tran_class")]
    public string TranClass { get; set; } = "EntreLaunch payments";

    [JsonProperty("cart_id")]
    public string CartId { get; set; } = null!;

    [JsonProperty("cart_currency")]
    public string CartCurrency { get; set; } = "SAR";

    [JsonProperty("cart_amount")]
    public decimal CartAmount { get; set; }

    [JsonProperty("cart_description")]
    public string CartDescription { get; set; } = "Payment";

    [JsonProperty("paypage_lang")]
    public string PaypageLang { get; set; } = "en";

    [JsonProperty("customer_details")]
    public PayTabsCustomerDetails? CustomerDetails { get; set; }

    [JsonProperty("callback")]
    public string CallbackUrl { get; set; } = null!;

    [JsonProperty("return")]
    public string ReturnUrl { get; set; } = null!;
}

public class PayTabsCustomerDetails
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("phone")]
    public string? Phone { get; set; }

    [JsonProperty("Street")]
    public string? Street1 { get; set; }

    [JsonProperty("city")]
    public string? City { get; set; }

    [JsonProperty("country")]
    public string? Country { get; set; }
}

public class PayTabsInitiateResponse
{
    [JsonProperty("tran_ref")]
    public string? TranRef { get; set; }

    [JsonProperty("redirect_url")]
    public string? RedirectUrl { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("success")]
    public bool Success { get; set; }
}

public class PaymentInitiateRequest
{
    public int PaymentId { get; set; }
#nullable disable
    public string PaymentToken { get; set; }
}

public class PayTabsCallbackRequest
{
    public string TranRef { get; set; }
    public string RespStatus { get; set; }
    public string RespMessage { get; set; }
    public string Signature { get; set; }
    // Add other properties here...

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "tranRef", TranRef },
            { "respStatus", RespStatus },
            { "respMessage", RespMessage },
            { "signature", Signature }
            // Add other properties here...
        };
    }
}

public class PayTabsIpnRequest
{
    public string TranRef { get; set; } // رقم المرجع
    public string CartId { get; set; } // معرّف الطلب
    public string CartCurrency { get; set; } // العملة المستخدمة
    public decimal CartAmount { get; set; } // المبلغ المطلوب
    public string TranCurrency { get; set; } // العملة الخاصة بالمعاملة
    public decimal TranTotal { get; set; } // المبلغ الإجمالي الذي تم خصمه
    public PaymentResultDto PaymentResult { get; set; } // تفاصيل الدفع
    public CustomerDetailsDto CustomerDetails { get; set; } // تفاصيل العميل
    public string MerchantId { get; set; } // معرّف التاجر
    public string ProfileId { get; set; } // معرّف الملف الشخصي
    public string IpnTrace { get; set; } // معرّف التتبع
    public string Signature { get; set; } // التوقيع للتحقق

    public Dictionary<string, string> ToDictionary()
    {
        var dictionary = new Dictionary<string, string>
        {
            { nameof(TranRef), TranRef },
            { nameof(CartId), CartId },
            { nameof(CartCurrency), CartCurrency },
            { nameof(CartAmount), CartAmount.ToString() },
            { nameof(TranCurrency), TranCurrency },
            { nameof(TranTotal), TranTotal.ToString() },
            { nameof(MerchantId), MerchantId },
            { nameof(ProfileId), ProfileId },
            { nameof(IpnTrace), IpnTrace },
            { nameof(Signature), Signature }
        };

        if (PaymentResult != null)
        {
            dictionary.Add("PaymentResult.ResponseStatus", PaymentResult.ResponseStatus);
            dictionary.Add("PaymentResult.ResponseCode", PaymentResult.ResponseCode);
            dictionary.Add("PaymentResult.ResponseMessage", PaymentResult.ResponseMessage);
            dictionary.Add("PaymentResult.TransactionTime", PaymentResult.TransactionTime?.ToString("o")); // ISO 8601 format
        }

        if (CustomerDetails != null)
        {
            dictionary.Add("CustomerDetails.Name", CustomerDetails.Name);
            dictionary.Add("CustomerDetails.Email", CustomerDetails.Email);
            dictionary.Add("CustomerDetails.Phone", CustomerDetails.Phone);
            dictionary.Add("CustomerDetails.Street1", CustomerDetails.Street1);
            dictionary.Add("CustomerDetails.City", CustomerDetails.City);
            dictionary.Add("CustomerDetails.State", CustomerDetails.State);
            dictionary.Add("CustomerDetails.Country", CustomerDetails.Country);
            dictionary.Add("CustomerDetails.Zip", CustomerDetails.Zip);
            dictionary.Add("CustomerDetails.Ip", CustomerDetails.Ip);
        }

        return dictionary;
    }
}

public class PaymentResultDto
{
    public string ResponseStatus { get; set; } // حالة الاستجابة
    public string ResponseCode { get; set; } // كود الاستجابة
    public string ResponseMessage { get; set; } // رسالة الاستجابة
    public DateTime? TransactionTime { get; set; } // وقت تنفيذ المعاملة
}

public class CustomerDetailsDto
{
    public string Name { get; set; } // اسم العميل
    public string Email { get; set; } // البريد الإلكتروني للعميل
    public string Phone { get; set; } // رقم الهاتف
    public string Street1 { get; set; } // عنوان الشارع
    public string City { get; set; } // المدينة
    public string State { get; set; } // الولاية/المنطقة
    public string Country { get; set; } // الدولة
    public string Zip { get; set; } // الرمز البريدي
    public string Ip { get; set; } // عنوان IP الخاص بالعميل
}

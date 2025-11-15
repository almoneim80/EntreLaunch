using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.WebStore.Types;

#nullable disable
public class InvoiceData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvoiceData"/> class.
    /// Represents the details of the invoice (costs, discounts, and total).
    /// </summary>
    public InvoiceData()
    {
    }
    public InvoiceData(decimal shippingCharges, decimal extraCharges, decimal extraDiscount, decimal total, LineItems lineItems)
    {
        ShippingCharges = shippingCharges;
        ExtraCharges = extraCharges;
        ExtraDiscount = extraDiscount;
        Total = total;
        LineItems = lineItems;
    } 

    [JsonProperty("shipping_charges")]
    public decimal ShippingCharges { get; init; }

    [JsonProperty("extra_charges")]
    public decimal ExtraCharges { get; init; }

    [JsonProperty("extra_discount")]
    public decimal ExtraDiscount { get; init; }

    [JsonProperty("total")]
    public decimal Total { get; init; }

    [JsonProperty("expiry_date")]
    public decimal ExpiryDate { get; init; }

    [JsonProperty("due_date")]
    public decimal DueDate { get; init; }

    [JsonProperty("notifications")]
    public InvoiceNotifications Notifications { get; init; }

    [JsonProperty("line_items")]
    public LineItems LineItems { get; init; }
}

/// <summary>
/// Represents the details of the invoice items.
/// </summary>
public class LineItems
{
    [JsonProperty("sku")]
    public string Sku { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("unit_cost")]
    public decimal UnitCost { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("net_total")]
    public decimal NetTotal { get; set; }

    [JsonProperty("discount_rate")]
    public int Discount_rate { get; set; }

    [JsonProperty("discount_amount")]
    public int Discount_amount { get; set; }

    [JsonProperty("tax_rate")]
    public int TaxRate { get; set; }

    [JsonProperty("tax_total")]
    public int TaxTotal { get; set; }

    [JsonProperty("total")]
    public decimal Total { get; set; }
}

public class InvoiceNotifications
{
    [JsonProperty("emails")]
    public string[] Emails { get; set; }

    [JsonProperty("phone_numbers")]
    public string[] PhoneNumbers { get; set; }
}

public class InvoiceBulkEmail
{
    [JsonProperty("address")]
    public string Address { get; set; }
}

public class InvoiceBulkSm
{
    [JsonProperty("numberRequested")]
    public string NumberRequested { get; set; }

    [JsonProperty("sentTo")]
    public string SentTo { get; set; }
}

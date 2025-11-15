using Microsoft.AspNetCore.Mvc;
using EntreLaunch.PayTabsHosted.API.Services;
using EntreLaunch.PayTabsHosted.API.WebStore;
using EntreLaunch.PayTabsHosted.API.WebStore.HostedPage;
using EntreLaunch.PayTabsHosted.API.WebStore.Types;

namespace EntreLaunch.PayTabsHosted.API.Controllers;

[Route("api/PayTabsPayment")]
public class PayTabsPaymentController : ControllerBase
{
    private readonly PayTabsService _payTabs;

    public PayTabsPaymentController(PayTabsService payTabs)
    {
        _payTabs = payTabs;
    }

    [HttpPost("create")]
    public IActionResult CreatePayment([FromBody] PaymentRequest request)
    {
        var response = _payTabs.CreatePayment(request.Amount, request.OrderId);
        return Ok(new
        {
            PaymentUrl = response.RedirectUrl,
            Reference = response.TransactionReference
        });
    }
}

public class PaymentRequest
{
#nullable disable
    public decimal Amount { get; set; }
    public string OrderId { get; set; }
}

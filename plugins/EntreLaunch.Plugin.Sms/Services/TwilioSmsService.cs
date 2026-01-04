using Serilog;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace EntreLaunch.Plugin.Sms.Services;
public class TwilioSmsService : ISmsService
{
    private readonly TwilioConfig twilioConfig;
    public TwilioSmsService(TwilioConfig twilioCfg)
    {
        twilioConfig = twilioCfg;
        try
        {
            TwilioClient.Init(twilioCfg.AccountSid, twilioCfg.AuthToken);
        }
        catch (ApiException e)
        {
            Log.Error("Failed to init twillio client {0}", e.Message);
        }
    }

    public string GetSender(string recipient)
    {
        return twilioConfig.FromNumber;
    }

    public async Task SendAsync(string recipient, string message)
    {
        var options = new CreateMessageOptions(new PhoneNumber(recipient))
        {
            From = new PhoneNumber(twilioConfig.FromNumber),
            Body = message,
        };

        await MessageResource.CreateAsync(options);
        Log.Information("Sms message sent to {0} via Twilio gateway: {1}", recipient, message);
    }

    Task<AddSmsResult> ISmsService.AddSmsToDb(CreateSmsDto create)
    {
        throw new NotImplementedException();
    }

    Task<List<SmsLog>> ISmsService.GetAllSms()
    {
        throw new NotImplementedException();
    }

    Task<SmsLog> ISmsService.GetSmsById(int id)
    {
        throw new NotImplementedException();
    }

    string ISmsService.ReplacePlaceholders(string templateContent, Dictionary<string, string> placeholders)
    {
        throw new NotImplementedException();
    }

    Task ISmsService.SuccessSent(int? smsLogId)
    {
        throw new NotImplementedException();
    }

    string ISmsService.ValidateAndFormatPhoneNumber(string phoneNumber)
    {
        throw new NotImplementedException();
    }
}

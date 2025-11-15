namespace EntreLaunch.Plugin.Sms.Interfaces;
public interface ISmsService
{
    /// <summary>
    /// Send an SMS message to the selected recipient.
    /// </summary>
    Task SendAsync(string recipient, string message);

    /// <summary>
    /// Gets the sender ID used for the specified recipient.
    /// </summary>
    string GetSender(string recipient);

    /// <summary>
    /// add sms to db.
    /// </summary>
    Task<AddSmsResult> AddSmsToDb(CreateSmsDto create);

    /// <summary>
    /// get all sms.
    /// </summary>
    Task<List<SmsLog>> GetAllSms();

    /// <summary>
    /// get one sms by id.
    /// </summary>
    Task<SmsLog> GetSmsById(int id);

    /// <summary>
    /// Validate and format a phone number using PhoneNumbers library.
    /// </summary>
    string ValidateAndFormatPhoneNumber(string phoneNumber);

    /// <summary>
    /// Mark the SMS as sent successfully.
    /// </summary>
    Task SuccessSent(int? smsLogId);

    /// <summary>
    /// Replaces placeholders in the provided template content with the corresponding values from the placeholders dictionary.
    /// </summary>
    string ReplacePlaceholders(string templateContent, Dictionary<string, string> placeholders);
}

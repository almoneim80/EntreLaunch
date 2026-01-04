namespace EntreLaunch.Plugin.Sms.DTOs;
public class SendOtpDto
{
    public string? Recipient { get; set; }
    public string? Language { get; set; }
}

public class VerifyOtpDto
{
    public string? Recipient { get; set; }
    public string? OtpCode { get; set; }
    public int OtpId { get; set; }
}

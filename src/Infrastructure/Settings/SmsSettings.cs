namespace RuanFa.Shop.Infrastructure.Settings;

public class SmsSettings
{
    public const string Section = "SmsSettings";
    public bool EnableSmsNotifications { get; init; }
    public SmsConfig SmsConfig { get; init; } = null!;
}

public sealed class SmsConfig
{
    public string ApiKey { get; init; } = null!;
    public string ApiSecret { get; init; } = null!;
    public string SenderPhoneNumber { get; init; } = null!;
    public string SmsServiceUrl { get; init; } = null!;
}

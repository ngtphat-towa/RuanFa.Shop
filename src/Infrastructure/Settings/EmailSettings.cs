namespace RuanFa.Shop.Infrastructure.Settings;

public sealed class EmailSettings
{
    public const string Section = "EmailSettings";
    public bool EnableEmailNotifications { get; init; }
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public SmtpConfig SmtpSettings { get; init; } = null!;
}
public sealed class SmtpConfig
{
    public string Server { get; init; } = null!;
    public int Port { get; init; }
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
}

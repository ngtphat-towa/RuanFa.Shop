namespace RuanFa.Shop.Infrastructure.Settings;

public class StripePaymentSettings
{
    public const string Section = "Stripe";

    public string PublishableKey { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string WebhookSecret { get; set; } = null!;
}

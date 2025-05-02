namespace RuanFa.Shop.Infrastructure.Settings;

public class SocialLoginSettings
{
    public const string Section = "SocialLogin";
    public GoogleSettings Google { get; set; } = null!;
    public FacebookSettings Facebook { get; set; } = null!;
}

namespace RuanFa.Shop.Infrastructure.Settings;

public sealed class JwtSettings
{
    public const string Section = "Jwt";

    public string Audience { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Secret { get; init; } = null!;
    public int TokenExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationInDays { get; init; }
}

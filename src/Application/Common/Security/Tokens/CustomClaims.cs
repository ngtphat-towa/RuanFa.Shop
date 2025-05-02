namespace RuanFa.Shop.Application.Common.Security.Tokens;

public static class CustomClaims
{
    public static string Permission => nameof(Permission).ToLowerInvariant();
    public static string Role => nameof(Role).ToLowerInvariant();
}

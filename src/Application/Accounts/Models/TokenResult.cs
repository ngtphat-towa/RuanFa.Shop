namespace RuanFa.Shop.Application.Accounts.Models;

public record TokenResult
{
    public string AccessToken { get; init; } = string.Empty;
    public long ExpiresIn { get; init; }
    public string? RefreshToken { get; init; }
    public string TokenType { get; } = "Bearer";
}

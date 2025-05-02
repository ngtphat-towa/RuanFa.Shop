namespace RuanFa.Shop.Application.Accounts.Models;

public record FashionPreferencesResult
{
    public string ClothingSize { get; init; } = string.Empty;
    public List<string> FavoriteCategories { get; init; } = new List<string>();
}

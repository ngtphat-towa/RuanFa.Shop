using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Accounts.ValueObjects;

public class FashionPreferences : ValueObject
{
    public string ClothingSize { get; init; } = string.Empty;
    public List<string> FavoriteCategories { get; init; } = new List<string>();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ClothingSize;
        foreach (var category in FavoriteCategories.OrderBy(c => c))
        {
            yield return category;
        }
    }
}

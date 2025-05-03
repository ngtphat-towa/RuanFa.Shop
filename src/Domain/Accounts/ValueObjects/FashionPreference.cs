using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Accounts.ValueObjects;

public class FashionPreference : ValueObject
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

    public static FashionPreference Create(string clothingSize, List<string>? favoriteCategories = null)
    {
        if (string.IsNullOrWhiteSpace(clothingSize))
            throw new ArgumentException("Clothing size is required.", nameof(clothingSize));

        // Clean and remove duplicates from favorite categories if provided
        var cleanedCategories = favoriteCategories?
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? new List<string>();

        return new FashionPreference
        {
            ClothingSize = clothingSize.Trim(),
            FavoriteCategories = cleanedCategories
        };
    }
}

using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Catalogs.ValueObjects;

public class CategoryImage : ValueObject
{
    public string Alt { get; private set; } = null!;
    public string Url { get; private set; } = null!;

    private CategoryImage() { } // For EF Core

    private CategoryImage(string alt, string url)
    {
        Alt = alt;
        Url = url;
    }

    public static ErrorOr<CategoryImage> Create(string? alt, string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return DomainErrors.CategoryImage.EmptyImageUrl;

        if (string.IsNullOrWhiteSpace(alt))
            return DomainErrors.CategoryImage.EmptyAltText;

        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return DomainErrors.CategoryImage.InvalidImageUrl;

        return new CategoryImage(alt, url);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Alt;
        yield return Url;
    }
}

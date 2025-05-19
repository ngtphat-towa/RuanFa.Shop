using ErrorOr;
using RuanFa.Shop.Domain.Commons.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Commons.ValueObjects;

public class SeoMeta : ValueObject
{
    public string? Slug { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string? Keywords { get; private set; }

    private SeoMeta() { }

    private SeoMeta(string? slug, string? title, string? description, string? keywords)
    {
        Slug = slug;
        Title = title;
        Description = description;
        Keywords = keywords;
    }

    public static ErrorOr<SeoMeta> Create(string? slug, string? title, string? description, string? keywords)
    {
        if (!string.IsNullOrWhiteSpace(slug) && slug.Length > 100)
            return DomainErrors.SeoMeta.SlugTooLong;

        if (!string.IsNullOrWhiteSpace(title) && title.Length > 70)
            return DomainErrors.SeoMeta.TitleTooLong;

        if (!string.IsNullOrWhiteSpace(description) && description.Length > 160)
            return DomainErrors.SeoMeta.DescriptionTooLong;

        if (!string.IsNullOrWhiteSpace(keywords) && keywords.Length > 255)
            return DomainErrors.SeoMeta.KeywordsTooLong;

        return new SeoMeta(slug, title, description, keywords);
    }

    public ErrorOr<Updated> Update(
     string? slug = null,
     string? title = null,
     string? description = null,
     string? keywords = null)
    {
        if (!string.IsNullOrWhiteSpace(slug) && slug.Length > 100)
            return DomainErrors.SeoMeta.SlugTooLong;

        if (!string.IsNullOrWhiteSpace(title) && title.Length > 70)
            return DomainErrors.SeoMeta.TitleTooLong;

        if (!string.IsNullOrWhiteSpace(description) && description.Length > 160)
            return DomainErrors.SeoMeta.DescriptionTooLong;

        if (!string.IsNullOrWhiteSpace(keywords) && keywords.Length > 255)
            return DomainErrors.SeoMeta.KeywordsTooLong;

        Slug = slug ?? Slug;
        Title = title ?? Title;
        Description = description ?? Description;
        Keywords = keywords ?? Keywords;

        return Result.Updated;
    }


    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Slug;
        yield return Title;
        yield return Description;
        yield return Keywords;
    }
}

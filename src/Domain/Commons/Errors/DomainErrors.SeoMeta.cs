using ErrorOr;

namespace RuanFa.Shop.Domain.Commons.Errors;

public static partial class DomainErrors
{
    public static class SeoMeta
    {
        public static Error SlugTooLong => Error.Validation(
            code: "SeoMeta.SlugTooLong",
            description: "The SEO slug cannot exceed 100 characters.");

        public static Error TitleTooLong => Error.Validation(
            code: "SeoMeta.TitleTooLong",
            description: "The SEO title cannot exceed 70 characters.");

        public static Error DescriptionTooLong => Error.Validation(
            code: "SeoMeta.DescriptionTooLong",
            description: "The SEO description cannot exceed 160 characters.");

        public static Error KeywordsTooLong => Error.Validation(
            code: "SeoMeta.KeywordsTooLong",
            description: "The SEO keywords cannot exceed 255 characters.");
    }
}

using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class CategoryImage
    {
        // Error when image URL is empty
        public static Error EmptyImageUrl => Error.Validation(
            code: "CategoryImage.EmptyImageUrl",
            description: "The image URL is required and cannot be empty."
        );

        // Error when image Alt text is empty
        public static Error EmptyAltText => Error.Validation(
            code: "CategoryImage.EmptyAltText",
            description: "The image alt text is required and cannot be empty."
        );

        // Error when image URL is invalid
        public static Error InvalidImageUrl => Error.Validation(
            code: "CategoryImage.InvalidImageUrl",
            description: "The image URL provided is invalid. Please ensure it is a valid URL."
        );
    }
}

using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class ProductImage
    {
        public static Error EmptyImageUrl => Error.Validation(
            code: "ProductImage.EmptyImageUrl",
            description: "The image URL is required and cannot be empty."
        );

        public static Error InvalidImageType => Error.Validation(
            code: "ProductImage.InvalidImageType",
            description: "The image type is invalid."
        );

        public static Error EmptyAltText => Error.Validation(
            code: "ProductImage.EmptyAltText",
            description: "The image alt text is required and cannot be empty."
        );

        public static Error InvalidImageUrl(string reason) => Error.Validation(
            code: "ProductImage.InvalidImageUrl",
            description: $"The image URL provided is invalid: {reason}"
        );

        public static Error AltTextTooLong => Error.Validation(
            code: "ProductImage.AltTextTooLong",
            description: "The image alt text cannot exceed 125 characters."
        );

        public static Error InvalidProductId => Error.Validation(
            code: "ProductImage.InvalidProductId",
            description: "The product ID cannot be empty."
        );

        public static Error InvalidVariantId => Error.Validation(
            code: "ProductImage.InvalidVariantId",
            description: "The variant ID is invalid or does not exist."
        );

        public static Error InvalidImageData => Error.Validation(
            code: "ProductImage.InvalidImageData",
            description: "The image data cannot be null."
        );

        public static Error InvalidImageDimensions => Error.Validation(
            code: "ProductImage.InvalidImageDimensions",
            description: "The image dimensions must be greater than zero."
        );

        public static Error InvalidFileSize => Error.Validation(
            code: "ProductImage.InvalidFileSize",
            description: "The image file size must be greater than zero."
        );

        public static Error ImageNotFound => Error.NotFound(
            code: "ProductImage.ImageNotFound",
            description: "The specified image was not found."
        );

        public static Error CannotSetDefaultForNonExistentImage => Error.Conflict(
            code: "ProductImage.CannotSetDefaultForNonExistentImage",
            description: "Cannot set an image as default if it does not exist."
        );

        public static Error InvalidMimeType => Error.Validation(
            code: "ProductImage.InvalidMimeType",
            description: "The MIME type is invalid or does not match the image extension."
        );
    }
}

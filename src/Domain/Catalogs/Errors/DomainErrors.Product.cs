using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Enums;

namespace RuanFa.Shop.Domain.Catalogs.Errors;
public static partial class DomainErrors
{
    public static class Product
    {
        public static Error InvalidName => Error.Validation(
            code: "Product.InvalidName",
            description: "The product name is required and must be between 3 and 100 characters."
        );

        public static Error InvalidSku => Error.Validation(
            code: "Product.InvalidSku",
            description: "The SKU is required, must be 3-50 characters, and contain only alphanumeric characters, dashes, or underscores."
        );

        public static Error InvalidBasePrice => Error.Validation(
            code: "Product.InvalidBasePrice",
            description: "The base price must be non-negative."
        );

        public static Error InvalidWeight => Error.Validation(
            code: "Product.InvalidWeight",
            description: "The weight must be non-negative."
        );

        public static Error InvalidGroupId => Error.Validation(
            code: "Product.InvalidGroupId",
            description: "The attribute group ID must be a valid, non-empty GUID."
        );

        public static Error InvalidTaxClass => Error.Validation(
            code: "Product.InvalidTaxClass",
            description: "The tax class must be a valid value."
        );

        public static Error InvalidStatus => Error.Validation(
            code: "Product.InvalidStatus",
            description: "The product status must be a valid value."
        );

        public static Error InvalidStatusTransition(ProductStatus currentStatus, ProductStatus targetStatus, string description) => Error.Conflict(
            code: "Product.InvalidStatusTransition",
            description: $"Cannot transition from {currentStatus} to {targetStatus}: {description}"
        );

        public static Error VariantNotFound => Error.NotFound(
            code: "Product.VariantNotFound",
            description: "The specified variant was not found for this product."
        );

        public static Error ImageNotFound => Error.NotFound(
            code: "Product.ImageNotFound",
            description: "The specified image was not found for this product."
        );

        public static Error CannotActivateWithoutVariantsOrImages => Error.Conflict(
            code: "Product.CannotActivateWithoutVariantsOrImages",
            description: "The product cannot be activated without at least one variant and one image."
        );

        public static Error DuplicateSku => Error.Conflict(
            code: "Product.DuplicateSku",
            description: "The SKU is already in use by another product or variant."
        );

        public static Error NoActiveVariants => Error.Conflict(
            code: "Product.NoActiveVariants",
            description: "The product cannot be activated without at least one active variant."
        );

        public static Error InvalidAttributeGroup => Error.Validation(
            code: "Product.InvalidAttributeGroup",
            description: "The specified attribute group does not exist."
        );

        public static Error CannotDeleteActiveProduct => Error.Conflict(
            code: "Product.CannotDeleteActiveProduct",
            description: "An active product cannot be deleted."
        );

        public static Error InsufficientStockForActivation => Error.Conflict(
            code: "Product.InsufficientStockForActivation",
            description: "The product cannot be activated because one or more variants have insufficient stock."
        );

        public static Error MissingDefaultVariant => Error.Conflict(
            code: "Product.MissingDefaultVariant",
            description: "The product cannot be activated without a default variant."
        );

        public static Error MissingDefaultImage => Error.Conflict(
            code: "Product.MissingDefaultImage",
            description: "The product cannot be activated without a default image."
        );

        public static Error MissingDefaultImageType => Error.Conflict(
            code: "Product.MissingDefaultImageType",
            description: "The product cannot be activated without a default image of type 'Default'."
        );

        public static Error DuplicateImageUrl => Error.Conflict(
            code: "Product.DuplicateImageUrl",
            description: "An image with the same URL is already associated with this product."
        );

        public static Error CannotRemoveDefaultVariant => Error.Conflict(
            code: "Product.CannotRemoveDefaultVariant",
            description: "The default variant cannot be removed unless another variant is set as default."
        );

        public static Error CannotRemoveDefaultImage => Error.Conflict(
            code: "Product.CannotRemoveDefaultImage",
            description: "The default image cannot be removed unless another image is set as default."
        );
    }

}

using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class ProductVariant
    {
        public static Error InvalidSku => Error.Validation(
            code: "ProductVariant.InvalidSku",
            description: "The SKU is required, must be 3-50 characters, and contain only alphanumeric characters, dashes, or underscores."
        );

        public static Error InvalidPriceOffset => Error.Validation(
            code: "ProductVariant.InvalidPriceOffset",
            description: "The price offset must be between -10000 and 10000."
        );

        public static Error InvalidStockQuantity => Error.Validation(
            code: "ProductVariant.InvalidStockQuantity",
            description: "The stock quantity must be non-negative."
        );

        public static Error InvalidLowStockThreshold => Error.Validation(
            code: "ProductVariant.InvalidLowStockThreshold",
            description: "The low stock threshold must be non-negative."
        );

        public static Error InvalidProductId => Error.Validation(
            code: "ProductVariant.InvalidProductId",
            description: "The product ID cannot be empty."
        );

        public static Error DuplicateAttributeOption => Error.Conflict(
            code: "ProductVariant.DuplicateAttributeOption",
            description: "The attribute option is already associated with this variant."
        );

        public static Error AttributeOptionNotFound => Error.NotFound(
            code: "ProductVariant.AttributeOptionNotFound",
            description: "The specified attribute option was not found."
        );

        public static Error InsufficientStock => Error.Conflict(
            code: "ProductVariant.InsufficientStock",
            description: "Insufficient stock available for the requested operation."
        );
    }
}

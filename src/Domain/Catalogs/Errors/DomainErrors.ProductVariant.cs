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
            description: "The product ID must be a valid, non-empty GUID."
        );

        public static Error InsufficientStock => Error.Conflict(
            code: "ProductVariant.InsufficientStock",
            description: "Insufficient stock available for the requested operation."
        );

        public static Error InvalidStockCheckQuantity => Error.Validation(
            code: "ProductVariant.InvalidStockCheckQuantity",
            description: "The quantity to check must be positive."
        );

        public static Error NegativeTotalPrice => Error.Validation(
            code: "ProductVariant.NegativeTotalPrice",
            description: "The total price (base price + price offset) cannot be negative."
        );
    }
}

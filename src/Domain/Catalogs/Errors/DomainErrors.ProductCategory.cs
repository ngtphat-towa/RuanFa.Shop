using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class ProductCategory
    {
        public static Error InvalidProductId => Error.Validation(
            code: "ProductCategory.InvalidProductId",
            description: "The product ID cannot be empty."
        );

        public static Error InvalidCategoryId => Error.Validation(
            code: "ProductCategory.InvalidCategoryId",
            description: "The category ID cannot be empty."
        );

        public static Error DuplicateCategory => Error.Conflict(
            code: "ProductCategory.DuplicateCategory",
            description: "The product is already associated with this category."
        );

        public static Error CategoryNotFound => Error.NotFound(
            code: "ProductCategory.CategoryNotFound",
            description: "The specified category was not found for this product."
        );

        public static Error InvalidCategory => Error.Validation(
            code: "ProductCategory.InvalidCategory",
            description: "The specified category is invalid or does not exist."
        );
    }
}

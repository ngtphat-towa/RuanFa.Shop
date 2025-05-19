using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class ProductCategory
    {

        public static Error InvalidCategoryId => Error.Validation(
            code: "ProductCategory.InvalidCategoryId",
            description: "The category ID must be a valid, non-empty GUID."
        );

        public static Error AlreadyAssigned => Error.Conflict(
            code: "ProductCategory.DuplicateCategory",
            description: "The product is already associated with this category."
        );

        public static Error CategoryNotFound => Error.NotFound(
            code: "ProductCategory.CategoryNotFound",
            description: "The product is not associated with the specified category."
        );

        public static Error InactiveCategory => Error.Conflict(
            code: "ProductCategory.InactiveCategory",
            description: "Cannot add a product to an inactive category."
        );
    }
}

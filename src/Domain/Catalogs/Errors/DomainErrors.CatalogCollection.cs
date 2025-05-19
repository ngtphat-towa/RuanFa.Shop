using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class CatalogCollection
    {
        public static Error InvalidId => Error.Validation(
            code: "CatalogCollection.InvalidId",
            description: "The collection ID must be a valid, non-empty GUID."
        );

        public static Error InvalidName => Error.Validation(
            code: "CatalogCollection.InvalidName",
            description: "The collection name is required and must be between 3 and 100 characters."
        );

        public static Error InvalidSlug => Error.Validation(
            code: "CatalogCollection.InvalidSlug",
            description: "The slug is required, must be 3-100 characters, and contain only lowercase letters, numbers, and hyphens."
        );

        public static Error DuplicateSlug => Error.Conflict(
            code: "CatalogCollection.DuplicateSlug",
            description: "The slug is already in use by another collection."
        );

        public static Error InvalidDescription => Error.Validation(
            code: "CatalogCollection.InvalidDescription",
            description: "The description cannot exceed 500 characters."
        );

        public static Error InvalidImageUrl => Error.Validation(
            code: "CatalogCollection.InvalidImageUrl",
            description: "The image URL must be a valid absolute URI."
        );

        public static Error InvalidType => Error.Validation(
            code: "CatalogCollection.InvalidType",
            description: "The collection type must be a valid value."
        );

        public static Error InvalidDisplayOrder => Error.Validation(
            code: "CatalogCollection.InvalidDisplayOrder",
            description: "The display order must be non-negative."
        );

        public static Error NotFound => Error.NotFound(
            code: "CatalogCollection.NotFound",
            description: "The specified collection was not found."
        );
    }
}

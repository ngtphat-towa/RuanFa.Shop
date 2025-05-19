using ErrorOr;

namespace RuanFa.Shop.Domain.Catalogs.Errors;

public static partial class DomainErrors
{
    public static class ProductCollection
    {
        public static Error InvalidCollectionId => Error.Validation(
            code: "ProductCollection.InvalidCollectionId",
            description: "The  ID must be a valid, non-empty GUID."
        );

        public static Error InvalidProductId => Error.Validation(
            code: "ProductCollection.InvalidProductId",
            description: "The product ID must be a valid, non-empty GUID."
        );

        public static Error ProductAlreadyInCollection => Error.Conflict(
            code: "CatalogCollection.ProductAlreadyInCollection",
            description: "The product is already in the collection."
        );

        public static Error ProductNotInCollection => Error.NotFound(
            code: "CatalogCollection.ProductNotInCollection",
            description: "The product is not in the collection."
        );

        public static Error CollectionNotFound => Error.NotFound(
            code: "ProductCollection.CollectionNotFound",
            description: "The product is not associated with the specified collection."
        );

    }
}

using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Errors;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class ProductCollection
{
    #region Properties
    public Guid CollectionId { get; private set; }
    public Guid ProductId { get; private set; }
    #endregion

    #region Relationships
    public CatalogCollection Collection { get; private set; } = null!;
    public Product Product { get; private set; } = null!;
    #endregion

    #region Constructor
    private ProductCollection() { } // For EF Core

    private ProductCollection(Guid collectionId, Guid productId)
    {
        CollectionId = collectionId;
        ProductId = productId;
    }
    #endregion

    #region Factory
    public static ErrorOr<ProductCollection> Create(
        Guid collectionId,
        Guid productId)
    {
        if (collectionId == Guid.Empty)
            return DomainErrors.ProductCollection.InvalidCollectionId;

        if (productId == Guid.Empty)
            return DomainErrors.ProductCollection.InvalidProductId;

        var productCategory = new ProductCollection(collectionId, productId);
        return productCategory;
    }
    #endregion
}

using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class ProductCategory : Entity<Guid>
{
    #region Properties
    public Guid CategoryId { get; private set; }
    public Guid ProductId { get; private set; }
    #endregion

    #region Relationships
    public Category Category { get; private set; } = null!;
    public Product Product { get; private set; } = null!;
    #endregion

    #region Constructor
    private ProductCategory() { } // For EF Core

    private ProductCategory(Guid categoryId, Guid productId)
    {
        Id = Guid.NewGuid();
        CategoryId = categoryId;
        ProductId = productId;
    }
    #endregion

    #region Factory
    public static ErrorOr<ProductCategory> Create(
        Guid categoryId,
        Guid productId)
    {
        if (categoryId == Guid.Empty)
            return DomainErrors.ProductCategory.InvalidCategoryId;

        if (productId == Guid.Empty)
            return DomainErrors.ProductCategory.InvalidProductId;

        var productCategory = new ProductCategory(categoryId, productId);
        productCategory.AddDomainEvent(new ProductCategoryCreatedEvent(
            productCategory.Id,
            categoryId,
            productId));
        return productCategory;
    }
    #endregion

    #region Domain Events
    public record ProductCategoryCreatedEvent(Guid Id, Guid CategoryId, Guid ProductId) : IDomainEvent;

    public record ProductCategoryRemovedEvent(Guid Id, Guid CategoryId, Guid ProductId) : IDomainEvent;
    #endregion
}

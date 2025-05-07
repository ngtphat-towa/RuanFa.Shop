using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Categorizations;

namespace RuanFa.Shop.Domain.Catalogs.Entities;
public class ProductCategory 
{
    #region Relationship
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    #endregion

    #region Methods
    public static ErrorOr<ProductCategory> Create(
        Guid productId,
        Guid categoryId)
    {
        // Validate input
        if (productId == Guid.Empty)
        {
            return DomainErrors.ProductCategory.InvalidProductId;
        }

        if (categoryId == Guid.Empty)
        {
            return DomainErrors.ProductCategory.InvalidCategoryId;
        }

        // Create and return the new AttributeGroupCollection
        return new ProductCategory
        {
           ProductId = productId,
           CategoryId = categoryId
        };
    }
    #endregion
}

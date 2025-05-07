using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;
using RuanFa.Shop.Domain.Catalogs.ValueObjects;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class ProductImage : Entity<Guid>
{
    #region Properties
    public ImageData Image { get; private set; } = null!;
    public bool IsDefault { get; private set; }
    #endregion

    #region Relationships
    public Guid ProductId { get; private set; }
    public Product Product { get; set; } = null!;
    public Guid? VariantId { get; private set; }
    public ProductVariant? Variant { get; set; }
    #endregion

    #region Constructor
    private ProductImage() { } // For EF Core

    private ProductImage(ImageData image, Guid productId, Guid? variantId, bool isDefault)
    {
        Id = Guid.NewGuid();
        Image = image;
        ProductId = productId;
        VariantId = variantId;
        IsDefault = isDefault;
    }
    #endregion

    #region Factory
    public static ErrorOr<ProductImage> Create(
        ImageData image,
        Guid productId,
        Guid? variantId = null,
        bool isDefault = false)
    {
        if (image == null)
            return DomainErrors.ProductImage.InvalidImageData;

        if (productId == Guid.Empty)
            return DomainErrors.ProductImage.InvalidProductId;

        if (variantId.HasValue && variantId == Guid.Empty)
            return DomainErrors.ProductImage.InvalidVariantId;

        var productImage = new ProductImage(image, productId, variantId, isDefault);
        productImage.AddDomainEvent(new ProductImageCreatedEvent(productImage.Id, productId, variantId));
        return productImage;
    }
    #endregion

    #region Methods
    public Updated SetDefault(bool isDefault = false)
    {
        if (IsDefault == isDefault) return Result.Updated;

        IsDefault = isDefault;
        AddDomainEvent(new ProductImageDefaultStatusChangedEvent(Id, isDefault));
        return Result.Updated;
    }

    public void UpdateImage(ImageData newImage)
    {
        if (newImage == null)
            throw new ArgumentNullException(nameof(newImage));

        Image = newImage;
        AddDomainEvent(new ProductImageUpdatedEvent(Id, newImage.Url));
    }

    public ErrorOr<Updated> AssociateWithVariant(Guid? variantId)
    {
        if (variantId.HasValue && variantId == Guid.Empty)
            return DomainErrors.ProductImage.InvalidVariantId;

        VariantId = variantId;
        AddDomainEvent(new ProductImageVariantAssociationChangedEvent(Id, variantId));
        return Result.Updated;
    }
    #endregion

    #region Domain Events
    public record ProductImageCreatedEvent(Guid ImageId, Guid ProductId, Guid? VariantId) : IDomainEvent;

    public record ProductImageDefaultStatusChangedEvent(Guid ImageId, bool IsDefault) : IDomainEvent;

    public record ProductImageUpdatedEvent(Guid ImageId, string NewUrl) : IDomainEvent;

    public record ProductImageVariantAssociationChangedEvent(Guid ImageId, Guid? VariantId) : IDomainEvent;
    #endregion
}

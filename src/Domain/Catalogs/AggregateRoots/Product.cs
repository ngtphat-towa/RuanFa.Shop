using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.ValueObjects;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using RuanFa.Shop.SharedKernel.Models.Domains;
using System.Text.RegularExpressions;

namespace RuanFa.Shop.Domain.Catalogs.AggregateRoots;

public class Product : AggregateRoot<Guid>
{
    #region Properties
    public string Name { get; private set; } = null!;
    public string Sku { get; private set; } = null!;
    public decimal BasePrice { get; private set; }
    public decimal Weight { get; private set; }
    public TaxClass TaxClass { get; private set; } = TaxClass.None;
    public ProductStatus Status { get; private set; } = ProductStatus.Draft;
    #endregion

    #region Relationships
    public Guid GroupId { get; private set; }
    public AttributeGroup Group { get; private set; } = null!;
    private readonly List<ProductCategory> _productCategories = new();
    private readonly List<ProductVariant> _variants = new();
    private readonly List<ProductImage> _images = new();
    public IReadOnlyCollection<ProductCategory> ProductCategories => _productCategories.AsReadOnly();
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
    #endregion

    #region Constructor
    private Product() { } // For EF Core

    private Product(
        string name,
        string sku,
        decimal basePrice,
        decimal weight,
        Guid groupId,
        TaxClass taxClass,
        ProductStatus status)
    {
        Id = Guid.NewGuid();
        Name = name;
        Sku = sku;
        BasePrice = basePrice;
        Weight = weight;
        GroupId = groupId;
        TaxClass = taxClass;
        Status = status;
    }
    #endregion

    #region Factory
    public static ErrorOr<Product> Create(
        string name,
        string sku,
        decimal basePrice,
        decimal weight,
        Guid groupId,
        TaxClass taxClass = TaxClass.None,
        ProductStatus status = ProductStatus.Draft)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 100)
            return DomainErrors.Product.InvalidName;

        if (string.IsNullOrWhiteSpace(sku) || sku.Length < 3 || sku.Length > 50 || !Regex.IsMatch(sku, @"^[a-zA-Z0-9\-_]+$"))
            return DomainErrors.Product.InvalidSku;

        if (basePrice < 0)
            return DomainErrors.Product.InvalidBasePrice;

        if (weight < 0)
            return DomainErrors.Product.InvalidWeight;

        if (groupId == Guid.Empty)
            return DomainErrors.Product.InvalidGroupId;

        if (!Enum.IsDefined(typeof(TaxClass), taxClass))
            return DomainErrors.Product.InvalidTaxClass;

        if (!Enum.IsDefined(typeof(ProductStatus), status))
            return DomainErrors.Product.InvalidStatus;

        var product = new Product(name, sku, basePrice, weight, groupId, taxClass, status);
        product.AddDomainEvent(new ProductCreatedEvent(product.Id, name, sku, status));
        return product;
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> Update(
        string? name = null,
        string? sku = null,
        decimal? basePrice = null,
        decimal? weight = null,
        TaxClass? taxClass = null,
        Guid? groupId = null)
    {
        if (name != null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 100)
                return DomainErrors.Product.InvalidName;
            Name = name;
        }

        if (sku != null)
        {
            if (string.IsNullOrWhiteSpace(sku) || sku.Length < 3 || sku.Length > 50 || !Regex.IsMatch(sku, @"^[a-zA-Z0-9\-_]+$"))
                return DomainErrors.Product.InvalidSku;
            Sku = sku;
        }

        if (basePrice.HasValue)
        {
            if (basePrice.Value < 0)
                return DomainErrors.Product.InvalidBasePrice;
            BasePrice = basePrice.Value;
            // Validate existing variants
            foreach (var variant in _variants)
            {
                if (basePrice.Value + variant.PriceOffset < 0)
                    return DomainErrors.ProductVariant.NegativeTotalPrice;
            }
        }

        if (weight.HasValue)
        {
            if (weight.Value < 0)
                return DomainErrors.Product.InvalidWeight;
            Weight = weight.Value;
        }

        if (taxClass.HasValue)
        {
            if (!Enum.IsDefined(typeof(TaxClass), taxClass.Value))
                return DomainErrors.Product.InvalidTaxClass;
            TaxClass = taxClass.Value;
        }

        if (groupId.HasValue)
        {
            if (groupId.Value == Guid.Empty)
                return DomainErrors.Product.InvalidGroupId;
            GroupId = groupId.Value;
        }

        AddDomainEvent(new ProductUpdatedEvent(Id, Name, Sku, GroupId));
        return Result.Updated;
    }

    public ErrorOr<Updated> AddCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            return DomainErrors.ProductCategory.InvalidCategoryId;

        if (_productCategories.Any(pc => pc.CategoryId == categoryId))
            return DomainErrors.ProductCategory.DuplicateCategory;

        var productCategoryResult = ProductCategory.Create(categoryId, Id);
        if (productCategoryResult.IsError)
            return productCategoryResult.Errors;

        _productCategories.Add(productCategoryResult.Value);
        AddDomainEvent(new ProductCategoryAddedEvent(Id, categoryId));
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveCategory(Guid categoryId)
    {
        var productCategory = _productCategories.FirstOrDefault(pc => pc.CategoryId == categoryId);
        if (productCategory == null)
            return DomainErrors.Category.NotFound;

        _productCategories.Remove(productCategory);
        AddDomainEvent(new ProductCategoryRemovedEvent(Id, categoryId));
        return Result.Updated;
    }

    public ErrorOr<ProductVariant> AddVariant(
        string sku,
        decimal priceOffset,
        int stockQuantity,
        int lowStockThreshold,
        bool isDefault = false)
    {
        if (BasePrice + priceOffset < 0)
            return DomainErrors.ProductVariant.NegativeTotalPrice;

        var variantResult = ProductVariant.Create(sku, priceOffset, stockQuantity, lowStockThreshold, Id, isDefault);
        if (variantResult.IsError)
            return variantResult.Errors;

        if (_variants.Any(v => v.Sku == sku) || Sku == sku)
            return DomainErrors.Product.DuplicateSku;

        _variants.Add(variantResult.Value);
        if (isDefault)
        {
            foreach (var v in _variants.Where(v => v.Id != variantResult.Value.Id))
            {
                var unsetResult = v.UnsetDefault();
                if (unsetResult.IsError)
                    return unsetResult.Errors;
            }
        }
        AddDomainEvent(new ProductVariantAddedEvent(Id, variantResult.Value.Id, sku));
        return variantResult.Value;
    }

    public ErrorOr<Updated> RemoveVariant(Guid variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            return DomainErrors.Product.VariantNotFound;

        if (variant.IsDefault && _variants.Count > 1)
            return DomainErrors.Product.CannotRemoveDefaultVariant;

        _variants.Remove(variant);
        AddDomainEvent(new ProductVariantRemovedEvent(Id, variantId));
        return Result.Updated;
    }

    public ErrorOr<Updated> SetDefaultVariant(Guid variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            return DomainErrors.Product.VariantNotFound;

        foreach (var v in _variants)
        {
            if (v.Id != variantId)
            {
                var unsetResult = v.UnsetDefault();
                if (unsetResult.IsError)
                    return unsetResult.Errors;
            }
        }

        var setResult = variant.SetAsDefault();
        if (setResult.IsError)
            return setResult.Errors;

        AddDomainEvent(new ProductDefaultVariantSetEvent(Id, variantId));
        return Result.Updated;
    }

    public ErrorOr<ProductImage> AddImage(
        ImageType imageType,
        string alt,
        string url,
        string? mimeType = null,
        int? width = null,
        int? height = null,
        long? fileSizeBytes = null,
        bool isDefault = false,
        Guid? variantId = null)
    {
        if (alt.Length > 125)
            return DomainErrors.ProductImage.AltTextTooLong;

        var imageDataResult = ImageData.Create(imageType, alt, url, mimeType, width, height, fileSizeBytes);
        if (imageDataResult.IsError)
            return imageDataResult.Errors;

        if (variantId.HasValue && variantId != Guid.Empty && !_variants.Any(v => v.Id == variantId))
            return DomainErrors.ProductImage.InvalidVariantId;

        if (_images.Any(img => img.Image.Url == url))
            return DomainErrors.Product.DuplicateImageUrl;

        var imageResult = ProductImage.Create(imageDataResult.Value, Id, variantId, isDefault);
        if (imageResult.IsError)
            return imageResult.Errors;

        _images.Add(imageResult.Value);
        if (isDefault)
        {
            foreach (var img in _images.Where(img => img.Id != imageResult.Value.Id))
            {
                img.SetDefault(false);
            }
        }
        AddDomainEvent(new ProductImageAddedEvent(Id, imageResult.Value.Id, url));
        return imageResult.Value;
    }

    public ErrorOr<Updated> RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(img => img.Id == imageId);
        if (image == null)
            return DomainErrors.Product.ImageNotFound;

        if (image.IsDefault && _images.Count > 1)
            return DomainErrors.Product.CannotRemoveDefaultImage;

        _images.Remove(image);
        AddDomainEvent(new ProductImageRemovedEvent(Id, imageId));
        return Result.Updated;
    }

    public ErrorOr<Updated> SetDefaultImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(img => img.Id == imageId);
        if (image == null)
            return DomainErrors.Product.ImageNotFound;

        foreach (var img in _images)
        {
            img.SetDefault(img.Id == imageId);
        }

        AddDomainEvent(new ProductDefaultImageSetEvent(Id, imageId));
        return Result.Updated;
    }

    public ErrorOr<Updated> ChangeStatus(ProductStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(ProductStatus), newStatus))
            return DomainErrors.Product.InvalidStatus;

        if (Status == newStatus)
            return DomainErrors.Product.InvalidStatusTransition(
                currentStatus: Status,
                targetStatus: newStatus,
                description: $"The product is already in {newStatus} status.");

        if (newStatus == ProductStatus.Active)
        {
            if (!_variants.Any() || !_images.Any())
                return DomainErrors.Product.CannotActivateWithoutVariantsOrImages;

            if (!_variants.Any(v => v.IsActive))
                return DomainErrors.Product.NoActiveVariants;

            if (!_variants.Any(v => v.IsDefault))
                return DomainErrors.Product.MissingDefaultVariant;

            if (!_images.Any(img => img.IsDefault))
                return DomainErrors.Product.MissingDefaultImage;

            if (!_images.Any(img => img.IsDefault && img.Image.ImageType == ImageType.Default))
                return DomainErrors.Product.MissingDefaultImageType;

            if (_variants.Any(v => v.StockQuantity <= 0))
                return DomainErrors.Product.InsufficientStockForActivation;
        }

        if (newStatus == ProductStatus.Discontinued && Status != ProductStatus.Active && Status != ProductStatus.Inactive)
        {
            return DomainErrors.Product.InvalidStatusTransition(
                currentStatus: Status,
                targetStatus: newStatus,
                description: "A product can only be discontinued from Active or Inactive status.");
        }

        Status = newStatus;
        AddDomainEvent(new ProductStatusChangedEvent(Id, Status, newStatus));
        return Result.Updated;
    }
    #endregion

    #region Domain Events
    public record ProductCreatedEvent(Guid ProductId, string Name, string Sku, ProductStatus Status) : IDomainEvent;
    public record ProductUpdatedEvent(Guid ProductId, string Name, string Sku, Guid GroupId) : IDomainEvent;
    public record ProductCategoryAddedEvent(Guid ProductId, Guid CategoryId) : IDomainEvent;
    public record ProductCategoryRemovedEvent(Guid ProductId, Guid CategoryId) : IDomainEvent;
    public record ProductVariantAddedEvent(Guid ProductId, Guid VariantId, string Sku) : IDomainEvent;
    public record ProductVariantRemovedEvent(Guid ProductId, Guid VariantId) : IDomainEvent;
    public record ProductDefaultVariantSetEvent(Guid ProductId, Guid VariantId) : IDomainEvent;
    public record ProductImageAddedEvent(Guid ProductId, Guid ImageId, string Url) : IDomainEvent;
    public record ProductImageRemovedEvent(Guid ProductId, Guid ImageId) : IDomainEvent;
    public record ProductDefaultImageSetEvent(Guid ProductId, Guid ImageId) : IDomainEvent;
    public record ProductStatusChangedEvent(Guid ProductId, ProductStatus OldStatus, ProductStatus NewStatus) : IDomainEvent;
    #endregion
}

using ErrorOr;
using RuanFa.Shop.Domain.Attributes;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.ValueObjects;
using RuanFa.Shop.SharedKernel.Models.Domains;
using System.Text.RegularExpressions;

namespace RuanFa.Shop.Domain.Catalogs;

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
    public ICollection<ProductCategory> ProductCategories { get; private set; } = new List<ProductCategory>();
    public ICollection<ProductVariant> Variants { get; private set; } = new List<ProductVariant>();
    public ICollection<ProductImage> Images { get; private set; } = new List<ProductImage>();
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

        return new Product(name, sku, basePrice, weight, groupId, taxClass, status);
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> UpdateDetails(
        string? name = null,
        string? sku = null,
        decimal? basePrice = null,
        decimal? weight = null,
        TaxClass? taxClass = null)
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

        return Result.Updated;
    }

    public ErrorOr<Updated> AddCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            return DomainErrors.ProductCategory.InvalidCategoryId;

        if (ProductCategories.Any(pc => pc.CategoryId == categoryId))
            return DomainErrors.ProductCategory.DuplicateCategory;

        var productCategoryResult = ProductCategory.Create(Id, categoryId);
        if (productCategoryResult.IsError)
            return productCategoryResult.Errors;

        ProductCategories.Add(productCategoryResult.Value);
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveCategory(Guid categoryId)
    {
        var productCategory = ProductCategories.FirstOrDefault(pc => pc.CategoryId == categoryId);
        if (productCategory == null)
            return DomainErrors.ProductCategory.CategoryNotFound;

        ProductCategories.Remove(productCategory);
        return Result.Updated;
    }

    public ErrorOr<ProductVariant> AddVariant(
        string sku,
        decimal priceOffset,
        int stockQuantity,
        int lowStockThreshold,
        bool isDefault = false)
    {
        var variantResult = ProductVariant.Create(sku, priceOffset, stockQuantity, lowStockThreshold, Id, isDefault);
        if (variantResult.IsError)
            return variantResult.Errors;

        if (Variants.Any(v => v.Sku == sku) || Sku == sku)
            return DomainErrors.Product.DuplicateSku;

        Variants.Add(variantResult.Value);
        if (isDefault)
        {
            foreach (var v in Variants.Where(v => v.Id != variantResult.Value.Id))
            {
                var unsetResult = v.UnsetDefault();
                if (unsetResult.IsError)
                    return unsetResult.Errors;
            }
        }
        return variantResult.Value;
    }

    public ErrorOr<Updated> SetDefaultVariant(Guid variantId)
    {
        var variant = Variants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
            return DomainErrors.Product.VariantNotFound;

        foreach (var v in Variants)
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

        return Result.Updated;
    }

    public ErrorOr<ProductImage> AddImage(ImageType imageType, string alt, string url, int? width = null, int? height = null, long? fileSizeBytes = null, bool isDefault = false, Guid? variantId = null)
    {
        var imageDataResult = ImageData.Create(imageType, alt, url, width, height, fileSizeBytes);
        if (imageDataResult.IsError)
            return imageDataResult.Errors;

        if (variantId.HasValue && variantId != Guid.Empty && !Variants.Any(v => v.Id == variantId))
            return DomainErrors.ProductImage.InvalidVariantId;

        if (Images.Any(img => img.Image.Url == url))
            return DomainErrors.Product.DuplicateImageUrl;

        var imageResult = ProductImage.Create(imageDataResult.Value, Id, variantId, isDefault);
        if (imageResult.IsError)
            return imageResult.Errors;

        Images.Add(imageResult.Value);
        if (isDefault)
        {
            foreach (var img in Images.Where(img => img.Id != imageResult.Value.Id))
            {
                img.SetDefault(false);
            }
        }
        return imageResult.Value;
    }

    public ErrorOr<Updated> SetDefaultImage(Guid imageId)
    {
        var image = Images.FirstOrDefault(img => img.Id == imageId);
        if (image == null)
            return DomainErrors.Product.ImageNotFound;

        foreach (var img in Images)
        {
            img.SetDefault(img.Id == imageId);
        }

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

        // Business rules for Active status
        if (newStatus == ProductStatus.Active)
        {
            if (!Variants.Any() || !Images.Any())
                return DomainErrors.Product.CannotActivateWithoutVariantsOrImages;

            if (!Variants.Any(v => v.IsActive))
                return DomainErrors.Product.NoActiveVariants;

            if (!Variants.Any(v => v.IsDefault))
                return DomainErrors.Product.MissingDefaultVariant;

            if (!Images.Any(img => img.IsDefault))
                return DomainErrors.Product.MissingDefaultImage;

            if (!Images.Any(img => img.IsDefault && img.Image.ImageType == ImageType.Default))
                return DomainErrors.Product.MissingDefaultImageType;

            if (Variants.Any(v => v.StockQuantity <= 0))
                return DomainErrors.Product.InsufficientStockForActivation;
        }

        // Restrict Discontinued transition
        if (newStatus == ProductStatus.Discontinued && Status != ProductStatus.Active && Status != ProductStatus.Inactive)
        {
            return DomainErrors.Product.InvalidStatusTransition(
                currentStatus: Status,
                targetStatus: newStatus,
                description: "A product can only be discontinued from Active or Inactive status.");
        }

        Status = newStatus;
        return Result.Updated;
    }
    #endregion
}

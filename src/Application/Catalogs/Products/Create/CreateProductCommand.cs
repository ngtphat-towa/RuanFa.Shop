using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Products.Models.Data;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Models.Requests;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.ValueObjects;
using RuanFa.Shop.Domain.Commons.ValueObjects;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Products.Create;

public sealed record CreateProductCommand : ICommand<Guid>
{
    #region General
    public required string Name { get; init; }
    public required string Sku { get; init; }
    public string? VariantSku { get; init; }
    public required decimal BasePrice { get; init; }
    public decimal? SalePrice { get; init; }
    public required decimal Weight { get; init; }

    public TaxClass TaxClass { get; init; } = TaxClass.None;
    public ProductStatus Status { get; init; } = ProductStatus.Draft;
    public List<DescriptionDataInput>? Descriptions { get; init; }
    #endregion

    #region Categorizations
    public required Guid GroupId { get; init; }
    public Guid? CategoryId { get; init; }
    public List<Guid>? CollectionIds { get; init; }
    #endregion

    #region Default Variant
    public decimal PriceOffset { get; init; }
    public int StockQuantity { get; init; }
    public int LowStockThreshold { get; init; }
    public bool IsActive { get; init; } = true;
    public bool IsVisible { get; init; } = true;
    public List<AttributeValueInput>? AttributeValues { get; init; }
    #endregion

    #region Product Image
    public List<ProductImageInput>? Images { get; init; }
    #endregion
}

internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Validate: SalePrice must not exceed BasePrice
        if (request.SalePrice.HasValue && request.SalePrice.Value > request.BasePrice)
        {
            return DomainErrors.Product.InvalidSalePrice;
        }

        // Validate: SKU uniqueness across Products and Variants
        var skuCheck = await ValidateSkuAsync(request.Sku, request.VariantSku, cancellationToken);
        if (skuCheck.IsError)
            return skuCheck.Errors;

        // Load: AttributeGroup with its Attributes and Options
        var attributeGroupResult = await LoadAttributeGroupAsync(request.GroupId, cancellationToken);
        if (attributeGroupResult.IsError)
            return attributeGroupResult.Errors;
        var attributeGroup = attributeGroupResult.Value;

        // Build: DescriptionData list
        var descriptions = BuildDescriptions(request.Descriptions);
        if (descriptions.IsError)
            return descriptions.Errors;

        // Create: Product entity
        var productResult = Product.Create(
            name: request.Name,
            sku: request.Sku,
            basePrice: request.BasePrice,
            salePrice: request.SalePrice,
            weight: request.Weight,
            groupId: request.GroupId,
            taxClass: request.TaxClass,
            status: request.Status,
            descriptions: descriptions.Value);

        if (productResult.IsError)
            return productResult.Errors;
        var product = productResult.Value;

        // Attach: Category if provided
        if (request.CategoryId.HasValue)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId.Value, cancellationToken);

            if (category == null)
                return DomainErrors.Category.NotFound;

            var categoryAddResult = product.AddCategory(request.CategoryId.Value);
            if (categoryAddResult.IsError)
                return categoryAddResult.Errors;
        }

        _context.Products.Add(product);

        // Process: Collections if provided
        if (request.CollectionIds != null && request.CollectionIds.Any())
        {
            var collectionsInDb = await _context.Collections
                .Where(c => request.CollectionIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var missingCollections = request.CollectionIds.Except(collectionsInDb).ToList();
            if (missingCollections.Any())
                return DomainErrors.CatalogCollection.NotFound;

            var productCollections = collectionsInDb
                .Select(collId => product.AddCollection(collId).Value)
                .ToList();

            _context.ProductCollections.AddRange(productCollections);
        }

        // Create: Default Variant (SKU = "{Sku}-default")
        string variantSku = request.VariantSku ?? $"{request.Sku}-default";
        var variantResult = product.AddVariant(
            sku: variantSku,
            priceOffset: request.PriceOffset,
            stockQuantity: request.StockQuantity,
            lowStockThreshold: request.LowStockThreshold,
            isDefault: true,
            isVisible: request.IsVisible,
            isActive: request.IsActive);

        if (variantResult.IsError)
            return variantResult.Errors;
        var variant = variantResult.Value;
        _context.Variants.Add(variant);

        // Add: Initial stock movement
        var stockMovementResult = variant.AdjustStock(request.StockQuantity, MovementType.Initial);
        if (stockMovementResult.IsError)
            return stockMovementResult.Errors;
        _context.StockMovements.Add(stockMovementResult.Value);

        // Assign: AttributeValues to the Variant
        var attributeAssignment = ValidateAndAddAttributes(
            request.AttributeValues,
            attributeGroup,
            variant);

        if (attributeAssignment.IsError)
            return attributeAssignment.Errors;

        // Process: Images if provided
        var imagesResult = ProcessImages(request.Images, product, variant);
        if (imagesResult.IsError)
            return imagesResult.Errors;

        // Save: Persist all changes
        await _context.SaveChangesAsync(cancellationToken);
        return product.Id;
    }

    private async Task<ErrorOr<Success>> ValidateSkuAsync(string sku, string? variantSku,  CancellationToken cancellationToken)
    {
        var existsInProducts = await _context.Products.AnyAsync(p => p.Sku == sku, cancellationToken);
        var existsInVariants = await _context.Variants.AnyAsync(v => v.Sku == sku || (!string.IsNullOrEmpty(variantSku) && v.Sku == variantSku), cancellationToken);

        if (existsInProducts || existsInVariants)
            return DomainErrors.Product.DuplicateSku;

        return Result.Success;
    }

    private async Task<ErrorOr<AttributeGroup>> LoadAttributeGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var attributeGroup = await _context.AttributeGroups
            .Include(ag => ag.AttributeGroupAttributes)
                .ThenInclude(aga => aga.Attribute)
                    .ThenInclude(a => a.AttributeOptions)
            .FirstOrDefaultAsync(ag => ag.Id == groupId, cancellationToken);

        if (attributeGroup == null)
            return DomainErrors.AttributeGroup.NotFound();

        return attributeGroup;
    }

    private ErrorOr<List<DescriptionData>> BuildDescriptions(List<DescriptionDataInput>? inputs)
    {
        if (inputs == null || inputs.Count == 0)
            return new List<DescriptionData>();

        var results = inputs
            .Select(i => DescriptionData.Create(i.Type, i.Value))
            .ToList();

        if (results.Any(r => r.IsError))
            return results
                .Where(r => r.IsError)
                .SelectMany(r => r.Errors)
                .ToList();

        return results
            .Where(r => !r.IsError)
            .Select(r => r.Value!)
            .ToList();
    }

    private ErrorOr<Success> ValidateAndAddAttributes(
        List<AttributeValueInput>? attributeValues,
        AttributeGroup attributeGroup,
        ProductVariant variant)
    {
        // Determine: Required attribute IDs in this group
        var requiredAttributeIds = attributeGroup.AttributeGroupAttributes
            .Where(aga => aga.Attribute.IsRequired)
            .Select(aga => aga.AttributeId)
            .ToHashSet();

        if ((attributeValues == null || !attributeValues.Any()) && requiredAttributeIds.Any())
        {
            return requiredAttributeIds
                .Select(id => DomainErrors.VariantAttributeValue.MissingRequiredAttribute(id))
                .ToList();
        }

        if (attributeValues == null || attributeValues.Count == 0)
            return Result.Success;

        // Check: Missing required IDs
        var providedIds = attributeValues.Select(av => av.AttributeId).ToHashSet();
        var missingRequired = requiredAttributeIds.Except(providedIds).ToList();
        if (missingRequired.Any())
        {
            return missingRequired
                .Select(id => DomainErrors.VariantAttributeValue.MissingRequiredAttribute(id))
                .ToList();
        }

        // Verify: Provided IDs exist in the group
        var existingIds = attributeGroup.AttributeGroupAttributes
            .Select(aga => aga.AttributeId)
            .ToHashSet();
        var invalidIds = providedIds.Except(existingIds).ToList();
        if (invalidIds.Any())
            return DomainErrors.VariantAttributeValue.InvalidAttributeForGroup;

        // Process: Each provided value
        foreach (var attrInput in attributeValues)
        {
            if (attrInput.AttributeOptionId == null &&
                string.IsNullOrWhiteSpace(attrInput.Value))
            {
                continue;
            }

            var attributeDef = attributeGroup.AttributeGroupAttributes
                .First(aga => aga.AttributeId == attrInput.AttributeId)
                .Attribute;

            if (attributeDef.Type != attrInput.AttributeType)
                return DomainErrors.VariantAttributeValue.InvalidAttibuteType;

            if ((attributeDef.Type == AttributeType.Dropdown || attributeDef.Type == AttributeType.Swatch)
                && (!attrInput.AttributeOptionId.HasValue
                    || !attributeDef.AttributeOptions.Any(o => o.Id == attrInput.AttributeOptionId.Value)))
            {
                return DomainErrors.VariantAttributeValue.InvalidAttributeOptionForAttribute;
            }
            else if (attributeDef.Type == AttributeType.Text && string.IsNullOrWhiteSpace(attrInput.Value))
            {
                return DomainErrors.VariantAttributeValue.InvalidValue;
            }

            var addVariantAttrResult = variant.AddVariantAttributeValue(
                attributeId: attrInput.AttributeId,
                attributeType: attrInput.AttributeType,
                attributeOptionId: attrInput.AttributeOptionId,
                value: attrInput.Value);

            if (addVariantAttrResult.IsError)
                return addVariantAttrResult.Errors;

            _context.VariantAttributeValues.Add(addVariantAttrResult.Value);
        }

        return Result.Success;
    }

    private ErrorOr<Success> ProcessImages(
        List<ProductImageInput>? images,
        Product product,
        ProductVariant variant)
    {
        if (images == null || images.Count == 0)
            return Result.Success;

        // Validate: Only one default image allowed
        if (images.Count(i => i.IsDefault) > 1)
            return DomainErrors.ProductImage.MultipleDefaultImages;

        var imageList = images
            .Select(i => new ProductImageInput
            {
                ImageType = i.ImageType,
                Alt = i.Alt,
                Url = i.Url,
                IsDefault = i.IsDefault
            })
            .ToList();

        // Ensure: At least one default image exists
        if (!imageList.Any(i => i.IsDefault) && imageList.Count > 0)
        {
            imageList[0].IsDefault = true;
        }

        foreach (var input in imageList)
        {
            var imageDataResult = ImageData.Create(
                imageType: input.ImageType,
                alt: input.Alt,
                url: input.Url);

            if (imageDataResult.IsError)
                return imageDataResult.Errors;
            var imageData = imageDataResult.Value;

            var productImageResult = ProductImage.Create(
                image: imageData,
                productId: product.Id,
                variantId: variant.Id,
                isDefault: input.IsDefault);

            if (productImageResult.IsError)
                return productImageResult.Errors;
            _context.ProductImages.Add(productImageResult.Value);
        }

        return Result.Success;
    }
}

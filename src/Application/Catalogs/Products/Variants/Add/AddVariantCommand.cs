using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Products.Models.Data;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.ValueObjects;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Products.Variants.Add;
public record AddVariantCommand : ICommand<Guid>
{
    public Guid ProductId { get; init; }
    public required string Sku { get; init; }

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

internal sealed class AddVariantCommandHandler : ICommandHandler<AddVariantCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddVariantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Guid>> Handle(AddVariantCommand request, CancellationToken cancellationToken)
    {
        // Validate SKU uniqueness
        var skuExists = await _context.Variants.AnyAsync(v => v.Sku == request.Sku, cancellationToken);
        if (skuExists)
            return DomainErrors.Product.DuplicateSku;

        // Load product with attribute group
        var product = await _context.Products
            .Include(p => p.Group)
                .ThenInclude(ag => ag.AttributeGroupAttributes)
                    .ThenInclude(aga => aga.Attribute)
                        .ThenInclude(a => a.AttributeOptions)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
            return DomainErrors.Product.NotFound;

        var attributeGroup = product.Group;
        if (attributeGroup == null)
            return DomainErrors.AttributeGroup.NotFound();

        // Add variant
        var variantResult = product.AddVariant(
            sku: request.Sku,
            priceOffset: request.PriceOffset,
            stockQuantity: request.StockQuantity,
            lowStockThreshold: request.LowStockThreshold,
            isDefault: false,
            isVisible: request.IsVisible,
            isActive: request.IsActive);

        if (variantResult.IsError)
            return variantResult.Errors;
        var variant = variantResult.Value;
        _context.Variants.Add(variant);

        // Adjust stock
        var stockMovementResult = variant.AdjustStock(request.StockQuantity, MovementType.Initial);
        if (stockMovementResult.IsError)
            return stockMovementResult.Errors;
        _context.StockMovements.Add(stockMovementResult.Value);

        // Validate and assign attributes
        var attributeAssignment = ValidateAndAddAttributes(request.AttributeValues, attributeGroup, variant);
        if (attributeAssignment.IsError)
            return attributeAssignment.Errors;

        // Add images
        var imageResult = ProcessImages(request.Images, product, variant);
        if (imageResult.IsError)
            return imageResult.Errors;

        // Save
        await _context.SaveChangesAsync(cancellationToken);
        return variant.Id;
    }

    private ErrorOr<Success> ValidateAndAddAttributes(
        List<AttributeValueInput>? attributeValues,
        AttributeGroup attributeGroup,
        ProductVariant variant)
    {
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

        var providedIds = attributeValues.Select(av => av.AttributeId).ToHashSet();
        var missingRequired = requiredAttributeIds.Except(providedIds).ToList();
        if (missingRequired.Any())
        {
            return missingRequired
                .Select(id => DomainErrors.VariantAttributeValue.MissingRequiredAttribute(id))
                .ToList();
        }

        var existingIds = attributeGroup.AttributeGroupAttributes
            .Select(aga => aga.AttributeId)
            .ToHashSet();
        var invalidIds = providedIds.Except(existingIds).ToList();
        if (invalidIds.Any())
            return DomainErrors.VariantAttributeValue.InvalidAttributeForGroup;

        foreach (var attrInput in attributeValues)
        {
            if (attrInput.AttributeOptionId == null && string.IsNullOrWhiteSpace(attrInput.Value))
                continue;

            var attributeDef = attributeGroup.AttributeGroupAttributes
                .First(aga => aga.AttributeId == attrInput.AttributeId)
                .Attribute;

            if (attributeDef.Type != attrInput.AttributeType)
                return DomainErrors.VariantAttributeValue.InvalidAttibuteType;

            if ((attributeDef.Type == AttributeType.Dropdown || attributeDef.Type == AttributeType.Swatch)
                && (!attrInput.AttributeOptionId.HasValue ||
                    !attributeDef.AttributeOptions.Any(o => o.Id == attrInput.AttributeOptionId.Value)))
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

        if (images.Count(i => i.IsDefault) > 1)
            return DomainErrors.ProductImage.MultipleDefaultImages;

        var imageList = images
            .Select(i => new ProductImageInput
            {
                ImageType = i.ImageType,
                Alt = i.Alt,
                Url = i.Url,
                IsDefault = i.IsDefault,
            })
            .ToList();

        if (!imageList.Any(i => i.IsDefault))
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

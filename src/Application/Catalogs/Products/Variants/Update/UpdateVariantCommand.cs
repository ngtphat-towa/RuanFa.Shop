using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Products.Models.Data;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.ValueObjects;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Products.Variants.Update;

public sealed record UpdateVariantCommand : ICommand<Guid>
{
    public required Guid VariantId { get; init; }

    public string? Sku { get; init; }
    public decimal? PriceOffset { get; init; }
    public int? LowStockThreshold { get; init; }
    public int? StockQuantity { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsVisible { get; init; }

    public List<AttributeValueInput>? AttributeValues { get; init; }
    public List<ProductImageInput>? Images { get; init; }
}

internal sealed class UpdateVariantCommandHandler : ICommandHandler<UpdateVariantCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public UpdateVariantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Guid>> Handle(UpdateVariantCommand request, CancellationToken cancellationToken)
    {
        // 1. Load existing variant along with its Product and related attributes
        var variant = await _context.Variants
            .Include(v => v.VariantAttributeValues)
            .Include(v => v.Product)                                // to get ProductId & GroupId
            .FirstOrDefaultAsync(v => v.Id == request.VariantId, cancellationToken);

        if (variant == null)
            return DomainErrors.ProductVariant.NotFound;

        var product = variant.Product!;
        // 2. Validate: new SKU (if provided) must not collide with any other SKU
        if (!string.IsNullOrWhiteSpace(request.Sku) && request.Sku != variant.Sku)
        {
            var skuCheck = await ValidateVariantSkuAsync(request.Sku!, variant.Id, cancellationToken);
            if (skuCheck.IsError)
                return skuCheck.Errors;
        }

        // 3. Load the attribute group belonging to this product
        var attributeGroupResult = await LoadAttributeGroupAsync(product.GroupId, cancellationToken);
        if (attributeGroupResult.IsError)
            return attributeGroupResult.Errors;
        var attributeGroup = attributeGroupResult.Value;

        // 4. Update core variant properties: SKU, PriceOffset, LowStockThreshold
        var updateResult = variant.Update(
            sku: string.IsNullOrWhiteSpace(request.Sku) ? null : request.Sku,
            priceOffset: request.PriceOffset ?? variant.PriceOffset,
            lowStockThreshold: request.LowStockThreshold ?? variant.LowStockThreshold);

        if (updateResult.IsError)
            return updateResult.Errors;

        // 5. Update availability if provided
        if (request.IsActive.HasValue)
        {
            var availabilityResult = variant.SetAvailability(request.IsActive.Value);
            if (availabilityResult.IsError)
                return availabilityResult.Errors;
        }

        // 6. Update visibility if provided
        if (request.IsVisible.HasValue)
        {
            var visibilityResult = variant.SetVisibility(request.IsVisible.Value);
            if (visibilityResult.IsError)
                return visibilityResult.Errors;
        }

        // 7. Adjust stock quantity if changed
        if (request.StockQuantity.HasValue && request.StockQuantity.Value != variant.StockQuantity)
        {
            var adjustment = request.StockQuantity.Value - variant.StockQuantity;
            if (adjustment != 0)
            {
                var stockMovementResult = variant.AdjustStock(adjustment, MovementType.Adjustment);
                if (stockMovementResult.IsError)
                    return stockMovementResult.Errors;

                _context.StockMovements.Add(stockMovementResult.Value);
            }
        }

        // 8. Update attribute values if provided
        if (request.AttributeValues != null)
        {
            var attributeResult = UpdateAttributes(request.AttributeValues, attributeGroup, variant);
            if (attributeResult.IsError)
                return attributeResult.Errors;
        }

        // 9. Update images if provided
        if (request.Images != null)
        {
            var imageResult = await UpdateImagesAsync(request.Images, product, variant, cancellationToken);
            if (imageResult.IsError)
                return imageResult.Errors;
        }

        // 10. Persist changes
        await _context.SaveChangesAsync(cancellationToken);
        return variant.Id;
    }

    private async Task<ErrorOr<Success>> ValidateVariantSkuAsync(
        string newSku,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        // A variant SKU must not collide with any other variant or any product SKU
        var existsInVariants = await _context.Variants
            .AnyAsync(v => v.Sku == newSku && v.Id != variantId, cancellationToken);

        var existsInProducts = await _context.Products
            .AnyAsync(p => p.Sku == newSku, cancellationToken);

        if (existsInVariants || existsInProducts)
            return DomainErrors.Product.DuplicateSku;

        return Result.Success;
    }

    private async Task<ErrorOr<AttributeGroup>> LoadAttributeGroupAsync(
        Guid groupId,
        CancellationToken cancellationToken)
    {
        var attributeGroup = await _context.AttributeGroups
            .Include(g => g.AttributeGroupAttributes)
                .ThenInclude(aga => aga.Attribute)
                    .ThenInclude(a => a.AttributeOptions)
            .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);

        if (attributeGroup == null)
            return DomainErrors.AttributeGroup.NotFound();

        return attributeGroup;
    }

    private ErrorOr<Success> UpdateAttributes(
        List<AttributeValueInput> inputs,
        AttributeGroup attributeGroup,
        ProductVariant variant)
    {
        // 1. Determine required attribute IDs in this group
        var requiredIds = attributeGroup.AttributeGroupAttributes
            .Where(aga => aga.Attribute.IsRequired)
            .Select(aga => aga.AttributeId)
            .ToHashSet();

        // 2. Collect provided IDs
        var providedIds = inputs.Select(i => i.AttributeId).ToHashSet();

        // 3. Check missing required
        var missingRequired = requiredIds.Except(providedIds).ToList();
        if (missingRequired.Any())
        {
            return missingRequired
                .Select(id => DomainErrors.VariantAttributeValue.MissingRequiredAttribute(id))
                .ToList();
        }

        // 4. Validate that all provided IDs exist in group
        var validIds = attributeGroup.AttributeGroupAttributes
            .Select(aga => aga.AttributeId)
            .ToHashSet();

        var invalidIds = providedIds.Except(validIds).ToList();
        if (invalidIds.Any())
            return DomainErrors.VariantAttributeValue.InvalidAttributeForGroup;

        // 5. Process each provided attribute value
        foreach (var attrInput in inputs)
        {
            // Skip if no option and no textual value
            if (attrInput.AttributeOptionId == null && string.IsNullOrWhiteSpace(attrInput.Value))
                continue;

            var attrDef = attributeGroup.AttributeGroupAttributes
                .First(aga => aga.AttributeId == attrInput.AttributeId)
                .Attribute;

            if (attrDef.Type != attrInput.AttributeType)
                return DomainErrors.VariantAttributeValue.InvalidAttibuteType;

            if ((attrDef.Type == AttributeType.Dropdown || attrDef.Type == AttributeType.Swatch)
                && (!attrInput.AttributeOptionId.HasValue
                    || !attrDef.AttributeOptions.Any(o => o.Id == attrInput.AttributeOptionId.Value)))
            {
                return DomainErrors.VariantAttributeValue.InvalidAttributeOptionForAttribute;
            }
            else if (attrDef.Type == AttributeType.Text && string.IsNullOrWhiteSpace(attrInput.Value))
            {
                return DomainErrors.VariantAttributeValue.InvalidValue;
            }

            // Check if the variant already has this attribute
            var existing = variant.VariantAttributeValues
                .FirstOrDefault(vav => vav.AttributeId == attrInput.AttributeId);

            if (existing != null)
            {
                // Update existing
                var updateRes = existing.Update(
                    attributeOptionId: attrInput.AttributeOptionId,
                    value: attrInput.Value,
                    attributeType: attrDef.Type);

                if (updateRes.IsError)
                    return updateRes.Errors;
            }
            else
            {
                // Add new
                var addRes = variant.AddVariantAttributeValue(
                    attributeId: attrInput.AttributeId,
                    attributeType: attrInput.AttributeType,
                    attributeOptionId: attrInput.AttributeOptionId,
                    value: attrInput.Value);

                if (addRes.IsError)
                    return addRes.Errors;

                _context.VariantAttributeValues.Add(addRes.Value);
            }
        }

        return Result.Success;
    }

    private async Task<ErrorOr<Success>> UpdateImagesAsync(
        List<ProductImageInput> inputs,
        Product product,
        ProductVariant variant,
        CancellationToken cancellationToken)
    {
        // 1. Ensure at most one default
        if (inputs.Count(i => i.IsDefault) > 1)
            return DomainErrors.ProductImage.MultipleDefaultImages;

        // 2. If none marked default, mark the first
        if (!inputs.Any(i => i.IsDefault) && inputs.Count > 0)
            inputs[0].IsDefault = true;

        // 3. Remove existing images for this variant
        var existingImages = await _context.ProductImages
            .Where(pi => pi.ProductId == product.Id && pi.VariantId == variant.Id)
            .ToListAsync(cancellationToken);

        foreach (var img in existingImages)
        {
            var removeRes = product.RemoveImage(img.Id);
            if (removeRes.IsError)
                return removeRes.Errors;

            _context.ProductImages.Remove(img);
        }

        // 4. Add new images
        foreach (var input in inputs)
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

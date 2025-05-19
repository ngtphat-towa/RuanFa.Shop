using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Products.Models.Data;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Models.Requests;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Commons.ValueObjects;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Products.Update;

public sealed record UpdateProductCommand : ICommand<Guid>
{
    public Guid Id { get; set; }

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
    public bool? IsActive { get; init; }
    public bool? IsVisible { get; init; }
    public List<AttributeValueInput>? AttributeValues { get; init; }
    #endregion

    #region Product Image
    public List<ProductImageInput>? Images { get; init; }
    #endregion
}

internal sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Guid>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // 1. Load existing product with all needed relations (tracked)
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Group)
            .Include(p => p.Images)
            .Include(p => p.ProductCollections)
                .ThenInclude(pc => pc.Collection)
            .Include(p => p.Variants)
                .ThenInclude(v => v.VariantAttributeValues)
                    .ThenInclude(vav => vav.AttributeOption)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return DomainErrors.Product.NotFound;

        // 2. Validate: SalePrice ≤ BasePrice
        if (request.SalePrice.HasValue && request.SalePrice.Value > request.BasePrice)
            return DomainErrors.Product.InvalidSalePrice;

        // 3. Validate: Product.SKU uniqueness (excluding self)
        if (request.Sku != product.Sku)
        {
            var skuCheck = await ValidateProductSkuAsync(request.Sku, request.Id, cancellationToken);
            if (skuCheck.IsError)
                return skuCheck.Errors;
        }

        // 4. Load AttributeGroup (cannot change it on update)
        var attributeGroupResult = await LoadAttributeGroupAsync(product.GroupId, cancellationToken);
        if (attributeGroupResult.IsError)
            return attributeGroupResult.Errors;
        var attributeGroup = attributeGroupResult.Value;

        // 5. If user provided a new VariantSku, validate uniqueness (excluding the existing default variant)
        var defaultVariant = product.Variants.FirstOrDefault(v => v.IsDefault);
        if (defaultVariant == null)
            return DomainErrors.ProductVariant.NotFound;

        if (!string.IsNullOrWhiteSpace(request.VariantSku) &&
            request.VariantSku != defaultVariant.Sku)
        {
            var variantSkuCheck = await ValidateVariantSkuAsync(request.VariantSku, defaultVariant.Id, cancellationToken);
            if (variantSkuCheck.IsError)
                return variantSkuCheck.Errors;
        }

        // 6. Update main Product properties (name, sku, prices, weight, taxClass, descriptions, status)
        var updateProductResult = UpdateProductProperties(product, request);
        if (updateProductResult.IsError)
            return updateProductResult.Errors;

        // 7. Update Category association if changed
        var currentCategoryId = product.Category?.Id;
        if (request.CategoryId != currentCategoryId)
        {
            // 7a. Remove old category if present
            if (currentCategoryId.HasValue)
            {
                var removeCatRes = product.RemoveCategory(currentCategoryId.Value);
                if (removeCatRes.IsError)
                    return removeCatRes.Errors;
            }

            // 7b. Add new category if provided
            if (request.CategoryId.HasValue)
            {
                var categoryEntity = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.CategoryId.Value, cancellationToken);
                if (categoryEntity == null)
                    return DomainErrors.Category.NotFound;

                var addCatRes = product.AddCategory(request.CategoryId.Value);
                if (addCatRes.IsError)
                    return addCatRes.Errors;
            }
        }

        // 8. Update Collections: remove absent ones, add new ones
        await UpdateProductCollectionsAsync(product, request.CollectionIds, cancellationToken);

        // 9. Update the default Variant, including SKU logic
        var variantResult = UpdateDefaultVariant(product, request, defaultVariant);
        if (variantResult.IsError)
            return variantResult.Errors;

        var updatedVariant = variantResult.Value;

        // 10. Update AttributeValues on default variant if provided
        if (request.AttributeValues != null)
        {
            var attributeResult = UpdateAttributes(request, attributeGroup, updatedVariant);
            if (attributeResult.IsError)
                return attributeResult.Errors;
        }

        // 11. Update Product + Variant images if provided
        if (request.Images != null)
        {
            var imageResult = await ProcessImagesAsync(request, product, updatedVariant, cancellationToken);
            if (imageResult.IsError)
                return imageResult.Errors;
        }

        // 12. Persist all changes
        await _context.SaveChangesAsync(cancellationToken);
        return product.Id;
    }

    private async Task<ErrorOr<Success>> ValidateProductSkuAsync(string newSku, Guid productId, CancellationToken cancellationToken)
    {
        // Must not collide with other products’ SKUs (excluding this product), nor any variant’s SKU
        var existsInProducts = await _context.Products
            .AnyAsync(p => p.Sku == newSku && p.Id != productId, cancellationToken);

        var existsInVariants = await _context.Variants
            .AnyAsync(v => v.Sku == newSku /* variants might belong to any product */, cancellationToken);

        if (existsInProducts || existsInVariants)
            return DomainErrors.Product.DuplicateSku;

        return Result.Success;
    }

    private async Task<ErrorOr<Success>> ValidateVariantSkuAsync(string newVariantSku, Guid defaultVariantId, CancellationToken cancellationToken)
    {
        // Must not collide with any other variant’s SKU (excluding the existing default variant)
        var exists = await _context.Variants
            .AnyAsync(v => v.Sku == newVariantSku && v.Id != defaultVariantId, cancellationToken);

        if (exists)
            return DomainErrors.Product.DuplicateSku;
        // (Assumes “DuplicateSku” is used for any SKU collision.)
        return Result.Success;
    }

    private async Task<ErrorOr<AttributeGroup>> LoadAttributeGroupAsync(Guid groupId, CancellationToken cancellationToken)
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

    private static ErrorOr<Success> UpdateProductProperties(Product product, UpdateProductCommand request)
    {
        // Build descriptions if any
        List<DescriptionData>? descriptions = null;
        if (request.Descriptions is { Count: > 0 })
        {
            var descriptionResults = request.Descriptions
                .Select(d => DescriptionData.Create(d.Type, d.Value))
                .ToList();

            if (descriptionResults.Any(r => r.IsError))
                return descriptionResults
                    .Where(r => r.IsError)
                    .SelectMany(r => r.Errors)
                    .ToList();

            descriptions = descriptionResults
                .Where(r => !r.IsError)
                .Select(r => r.Value)
                .ToList();
        }

        // Update core product
        var updateResult = product.Update(
            name: request.Name,
            sku: request.Sku,
            basePrice: request.BasePrice,
            salePrice: request.SalePrice,
            weight: request.Weight,
            taxClass: request.TaxClass,
            groupId: product.GroupId, // cannot change group on update
            descriptions: descriptions);

        if (updateResult.IsError)
            return updateResult.Errors;

        // Update status
        var statusResult = product.ChangeStatus(request.Status);
        if (statusResult.IsError)
            return statusResult.Errors;

        return Result.Success;
    }

    private async Task UpdateProductCollectionsAsync(Product product, List<Guid>? requestedCollectionIds, CancellationToken cancellationToken)
    {
        var existingCollectionIds = product.ProductCollections
            .Select(pc => pc.CollectionId)
            .ToHashSet();

        var requestedIds = requestedCollectionIds?.ToHashSet() ?? new HashSet<Guid>();

        // Remove any collections no longer requested
        var toRemove = existingCollectionIds.Except(requestedIds).ToList();
        foreach (var collId in toRemove)
        {
            var pcEntry = product.ProductCollections.First(pc => pc.CollectionId == collId);
            product.RemoveCollection(collId);
            _context.ProductCollections.Remove(pcEntry);
        }

        // Add any new collections
        var toAdd = requestedIds.Except(existingCollectionIds).ToList();
        if (toAdd.Any())
        {
            // Verify they all exist
            var collectionsInDb = await _context.Collections
                .Where(c => toAdd.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var missing = toAdd.Except(collectionsInDb).ToList();
            if (missing.Any())
                throw new InvalidOperationException("Collection IDs not found.");
            // In practice, you might return an ErrorOr instead of throwing.

            foreach (var collId in toAdd)
            {
                var addRes = product.AddCollection(collId);
                if (addRes.IsError)
                    throw new InvalidOperationException("Failed to add collection.");
                // Again, you could return an ErrorOr instead of throwing.
                _context.ProductCollections.Add(addRes.Value);
            }
        }
    }

    private ErrorOr<ProductVariant> UpdateDefaultVariant(
        Product product,
        UpdateProductCommand request,
        ProductVariant defaultVariant)
    {
        // 1. Determine the new SKU for the variant
        string? newVariantSku = null;

        if (!string.IsNullOrWhiteSpace(request.VariantSku) &&
            request.VariantSku != defaultVariant.Sku)
        {
            // Use exactly what the caller specified
            newVariantSku = request.VariantSku;
        }
        else if (request.Sku != product.Sku)
        {
            // If product SKU changed and this variant formerly followed "{oldSku}-default", keep pattern
            if (defaultVariant.Sku.EndsWith("-default") &&
                defaultVariant.Sku.StartsWith(product.Sku))
            {
                newVariantSku = $"{request.Sku}-default";
            }
            else
            {
                // Otherwise, leave it alone
                newVariantSku = null;
            }
        }

        // 2. Update core variant properties (SKU, priceOffset, lowStockThreshold)
        var updateResult = defaultVariant.Update(
            sku: newVariantSku,
            priceOffset: request.PriceOffset,
            lowStockThreshold: request.LowStockThreshold);

        if (updateResult.IsError)
            return updateResult.Errors;

        // 3. Update availability if provided
        if (request.IsActive.HasValue)
        {
            var availabilityResult = defaultVariant.SetAvailability(request.IsActive.Value);
            if (availabilityResult.IsError)
                return availabilityResult.Errors;
        }

        // 4. Update visibility if provided
        if (request.IsVisible.HasValue)
        {
            var visibilityResult = defaultVariant.SetVisibility(request.IsVisible.Value);
            if (visibilityResult.IsError)
                return visibilityResult.Errors;
        }

        // 5. Adjust stock quantity if it changed
        if (request.StockQuantity != defaultVariant.StockQuantity)
        {
            var adjustment = request.StockQuantity - defaultVariant.StockQuantity;
            if (adjustment != 0)
            {
                var stockMovementResult = defaultVariant.AdjustStock(adjustment, MovementType.Adjustment);
                if (stockMovementResult.IsError)
                    return stockMovementResult.Errors;
                _context.StockMovements.Add(stockMovementResult.Value);
            }
        }

        return defaultVariant;
    }

    private ErrorOr<Success> UpdateAttributes(
        UpdateProductCommand request,
        AttributeGroup attributeGroup,
        ProductVariant variant)
    {
        if (request.AttributeValues == null)
            return Result.Success;

        // Collect required attribute IDs
        var requiredIds = attributeGroup.AttributeGroupAttributes
            .Where(aga => aga.Attribute.IsRequired)
            .Select(aga => aga.AttributeId)
            .ToHashSet();

        // Existing IDs on the variant
        var existingIds = variant.VariantAttributeValues
            .Select(av => av.AttributeId)
            .ToHashSet();

        // Incoming IDs
        var providedIds = request.AttributeValues.Select(av => av.AttributeId).ToHashSet();

        // Combined set to check missing required
        var allIds = new HashSet<Guid>(existingIds);
        allIds.UnionWith(providedIds);

        var missingRequired = requiredIds.Except(allIds).ToList();
        if (missingRequired.Any())
        {
            return missingRequired
                .Select(id => DomainErrors.VariantAttributeValue.MissingRequiredAttribute(id))
                .ToList();
        }

        // Make sure all provided IDs exist in the group
        var validIds = attributeGroup.AttributeGroupAttributes
            .Select(aga => aga.AttributeId)
            .ToHashSet();

        var invalidIds = providedIds.Except(validIds).ToList();
        if (invalidIds.Any())
            return DomainErrors.VariantAttributeValue.InvalidAttributeForGroup;

        // Process each provided attribute value
        foreach (var avInput in request.AttributeValues)
        {
            var attributeDef = attributeGroup.AttributeGroupAttributes
                .First(aga => aga.AttributeId == avInput.AttributeId)
                .Attribute;

            if (attributeDef.Type != avInput.AttributeType)
                return DomainErrors.VariantAttributeValue.InvalidAttibuteType;

            if ((attributeDef.Type == AttributeType.Dropdown || attributeDef.Type == AttributeType.Swatch)
                && (!avInput.AttributeOptionId.HasValue
                    || !attributeDef.AttributeOptions.Any(o => o.Id == avInput.AttributeOptionId.Value)))
            {
                return DomainErrors.VariantAttributeValue.InvalidAttributeOptionForAttribute;
            }
            else if (attributeDef.Type == AttributeType.Text && string.IsNullOrWhiteSpace(avInput.Value))
            {
                return DomainErrors.VariantAttributeValue.InvalidValue;
            }

            // Check if it already exists on the variant
            var existingValue = variant.VariantAttributeValues
                .FirstOrDefault(vav => vav.AttributeId == avInput.AttributeId);

            if (existingValue != null)
            {
                // Update existing
                var updateRes = existingValue.Update(
                    attributeOptionId: avInput.AttributeOptionId,
                    value: avInput.Value,
                    attributeType: attributeDef.Type);

                if (updateRes.IsError)
                    return updateRes.Errors;
            }
            else
            {
                // Add new
                var addRes = variant.AddVariantAttributeValue(
                    attributeId: avInput.AttributeId,
                    attributeType: avInput.AttributeType,
                    attributeOptionId: avInput.AttributeOptionId,
                    value: avInput.Value);

                if (addRes.IsError)
                    return addRes.Errors;

                _context.VariantAttributeValues.Add(addRes.Value);
            }
        }

        return Result.Success;
    }

    private async Task<ErrorOr<Success>> ProcessImagesAsync(
        UpdateProductCommand request,
        Product product,
        ProductVariant variant,
        CancellationToken cancellationToken)
    {
        if (request.Images == null)
            return Result.Success;

        // Only one default allowed
        if (request.Images.Count(i => i.IsDefault) > 1)
            return DomainErrors.ProductImage.MultipleDefaultImages;

        // If none marked default, mark the first
        if (!request.Images.Any(i => i.IsDefault) && request.Images.Count > 0)
        {
            request.Images[0].IsDefault = true;
        }

        // Delete existing images for this variant
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

        // Add new images
        foreach (var imgInput in request.Images)
        {
            var addImageRes = product.AddImage(
                imageType: imgInput.ImageType,
                alt: imgInput.Alt,
                url: imgInput.Url,
                isDefault: imgInput.IsDefault,
                variantId: variant.Id);

            if (addImageRes.IsError)
                return addImageRes.Errors;

            _context.ProductImages.Add(addImageRes.Value);
        }

        return Result.Success;
    }
}

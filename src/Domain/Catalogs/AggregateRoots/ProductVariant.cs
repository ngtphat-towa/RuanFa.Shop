using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.SharedKernel.Models.Domains;
using System.Text.RegularExpressions;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using RuanFa.Shop.Domain.Catalogs.Entities;

namespace RuanFa.Shop.Domain.Catalogs.AggregateRoots;

public class ProductVariant : AggregateRoot<Guid>
{
    #region Properties
    public string Sku { get; private set; } = null!;
    public decimal PriceOffset { get; private set; }
    public int StockQuantity { get; private set; }
    public int LowStockThreshold { get; private set; }
    public StockStatus StockStatus { get; private set; } = StockStatus.InStock;
    public bool IsActive { get; private set; } = true;
    public bool IsVisible { get; private set; } = true;
    public bool IsDefault { get; private set; }
    public Guid ProductId { get; private set; }
    #endregion

    #region Relationships
    public Product Product { get; private set; } = null!;
    private readonly List<VariantAttributeValue> _variantAttributeValues = new();
    private readonly List<StockMovement> _stockMovements = new();
    private readonly List<ProductImage> _variantImages = new();
    public IReadOnlyCollection<VariantAttributeValue> VariantAttributeValues => _variantAttributeValues.AsReadOnly();
    public IReadOnlyCollection<StockMovement> StockMovements => _stockMovements.AsReadOnly();
    public IReadOnlyCollection<ProductImage> VariantImages => _variantImages.AsReadOnly();
    #endregion

    #region Constructor
    private ProductVariant() { } // For EF Core

    private ProductVariant(
        string sku,
        decimal priceOffset,
        int stockQuantity,
        int lowStockThreshold,
        Guid productId,
        bool isDefault,
        bool? isActive = null,
        bool? isVisible = null)
    {
        Id = Guid.NewGuid();
        Sku = sku;
        PriceOffset = priceOffset;
        StockQuantity = stockQuantity;
        LowStockThreshold = lowStockThreshold;
        ProductId = productId;
        IsDefault = isDefault;
        IsActive = isActive ?? false;
        IsVisible = isVisible ?? false;
        UpdateStockStatus();
    }
    #endregion

    #region Factory
    public static ErrorOr<ProductVariant> Create(
        string sku,
        decimal priceOffset,
        int stockQuantity,
        int lowStockThreshold,
        Guid productId,
        bool isDefault = false,
        bool? isActive = false,
        bool? isVisible = false)
    {
        if (string.IsNullOrWhiteSpace(sku) || sku.Length < 3 || sku.Length > 50 || !Regex.IsMatch(sku, @"^[a-zA-Z0-9\-_]+$"))
            return DomainErrors.ProductVariant.InvalidSku;
        if (priceOffset < -10000m || priceOffset > 10000m)
            return DomainErrors.ProductVariant.InvalidPriceOffset;
        if (stockQuantity < 0)
            return DomainErrors.ProductVariant.InvalidStockQuantity;
        if (lowStockThreshold < 0)
            return DomainErrors.ProductVariant.InvalidLowStockThreshold;
        if (productId == Guid.Empty)
            return DomainErrors.ProductVariant.InvalidProductId;

        var variant = new ProductVariant(
            sku: sku,
            priceOffset: priceOffset,
            stockQuantity: stockQuantity,
            lowStockThreshold: lowStockThreshold,
            productId: productId,
            isDefault: isDefault,
            isActive: isActive,
            isVisible: isVisible);

        variant.AddDomainEvent(new ProductVariantCreatedEvent(variant.Id, productId, sku));
        return variant;
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> Update(
        string? sku = null,
        string? name = null,
        decimal? priceOffset = null,
        int? lowStockThreshold = null)
    {
        if (sku != null)
        {
            if (string.IsNullOrWhiteSpace(sku) || sku.Length < 3 || sku.Length > 50 || !Regex.IsMatch(sku, @"^[a-zA-Z0-9\-_]+$"))
                return DomainErrors.ProductVariant.InvalidSku;
            Sku = sku;
        }
        if (name!=null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 100)
                return DomainErrors.Product.InvalidName;

        }

        if (priceOffset.HasValue)
        {
            if (priceOffset.Value < -10000m || priceOffset.Value > 10000m)
                return DomainErrors.ProductVariant.InvalidPriceOffset;
            if (Product.BasePrice + priceOffset.Value < 0)
                return DomainErrors.ProductVariant.NegativeTotalPrice;
            PriceOffset = priceOffset.Value;
        }

        if (lowStockThreshold.HasValue)
        {
            if (lowStockThreshold.Value < 0)
                return DomainErrors.ProductVariant.InvalidLowStockThreshold;
            LowStockThreshold = lowStockThreshold.Value;
            UpdateStockStatus();
        }
        
        AddDomainEvent(new ProductVariantUpdatedEvent(Id, Sku, PriceOffset, LowStockThreshold));
        return Result.Updated;
    }

    public ErrorOr<StockMovement> AdjustStock(int quantity, MovementType movementType, Guid? referenceId = null, string? notes = null)
    {
        if (movementType == MovementType.Adjustment)
        {
            if (quantity == 0)
                return DomainErrors.StockMovement.InvalidQuantity;
        }
        else
        {
            if (quantity < 0)
                return DomainErrors.StockMovement.InvalidQuantity;
        }

        if (StockMovement.IsOutboundMovement(movementType) && StockQuantity + quantity < 0)
            return DomainErrors.ProductVariant.InsufficientStock;

        var stockMovementResult = StockMovement.Create(
            variantId: Id,
            quantity: Math.Abs(quantity),
            movementType: movementType,
            referenceId: referenceId,
            notes: notes);

        if (stockMovementResult.IsError)
            return stockMovementResult.Errors;

        var oldStockStatus = StockStatus;
        _stockMovements.Add(stockMovementResult.Value);
        StockQuantity += stockMovementResult.Value.GetSignedQuantity();
        UpdateStockStatus();

        AddDomainEvent(new StockAdjustedEvent(Id, quantity, movementType));
        if (oldStockStatus != StockStatus)
            AddDomainEvent(new StockStatusChangedEvent(Id, oldStockStatus, StockStatus));

        return stockMovementResult.Value;
    }

    public ErrorOr<bool> CheckStockAvailability(int quantity)
    {
        if (quantity <= 0)
            return DomainErrors.ProductVariant.InvalidStockCheckQuantity;

        return StockQuantity >= quantity;
    }

    public ErrorOr<Updated> SetVariantAttributeValues(ICollection<(Guid AttributeId, Guid? AttributeOptionId, string? Value, AttributeType Type)> attributeValues)
    {
        if (attributeValues == null || !attributeValues.Any())
            return DomainErrors.VariantAttributeValue.NoAttributeValuesProvided;

        var duplicates = attributeValues.GroupBy(a => a.AttributeId).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicates.Any())
            return DomainErrors.VariantAttributeValue.DuplicateAttributeValue;

        var errors = new List<Error>();
        var validAttributeIds = Product.Group.AttributeGroupAttributes.Select(aga => aga.AttributeId).ToList();
        var requiredAttributes = Product.Group.AttributeGroupAttributes
            .Where(aga => aga.Attribute.IsRequired)
            .Select(aga => aga.AttributeId)
            .ToList();

        foreach (var attributeId in requiredAttributes)
        {
            if (!attributeValues.Any(a => a.AttributeId == attributeId))
                errors.Add(DomainErrors.VariantAttributeValue.MissingRequiredAttribute(attributeId));
        }

        foreach (var (attributeId, attributeOptionId, value, type) in attributeValues)
        {
            if (!validAttributeIds.Contains(attributeId))
                errors.Add(DomainErrors.VariantAttributeValue.InvalidAttributeForGroup);

            if (attributeOptionId.HasValue && attributeOptionId != Guid.Empty)
            {
                var attribute = Product.Group.AttributeGroupAttributes.FirstOrDefault(aga => aga.AttributeId == attributeId)?.Attribute;
                if (attribute == null || !attribute.AttributeOptions.Any(o => o.Id == attributeOptionId))
                    errors.Add(DomainErrors.VariantAttributeValue.InvalidAttributeOptionForAttribute);
            }
        }

        if (errors.Any())
            return errors;

        _variantAttributeValues.Clear();
        foreach (var (attributeId, attributeOptionId, value, type) in attributeValues)
        {
            var valueResult = VariantAttributeValue.Create(
                variantId: Id,
                attributeId: attributeId,
                attributeOptionId: attributeOptionId,
                value: value,
                attributeType: type);

            if (valueResult.IsError)
            {
                errors.AddRange(valueResult.Errors);
                continue;
            }

            _variantAttributeValues.Add(valueResult.Value);
        }

        if (errors.Any())
            return errors;

        AddDomainEvent(new VariantAttributesUpdatedEvent(Id, attributeValues.Select(a => a.AttributeId).ToList()));
        return Result.Updated;
    }

    public ErrorOr<VariantAttributeValue> AddVariantAttributeValue(Guid attributeId, Guid? attributeOptionId, string? value, AttributeType attributeType)
    {
        if (_variantAttributeValues.Any(v => v.AttributeId == attributeId))
            return DomainErrors.VariantAttributeValue.DuplicateAttributeValue;

        var validAttributeIds = Product.Group.AttributeGroupAttributes.Select(aga => aga.AttributeId).ToList();
        if (!validAttributeIds.Contains(attributeId))
            return DomainErrors.VariantAttributeValue.InvalidAttributeForGroup;

        if (attributeOptionId.HasValue && attributeOptionId != Guid.Empty)
        {
            var attribute = Product.Group.AttributeGroupAttributes.FirstOrDefault(aga => aga.AttributeId == attributeId)?.Attribute;
            if (attribute == null || !attribute.AttributeOptions.Any(o => o.Id == attributeOptionId))
                return DomainErrors.VariantAttributeValue.InvalidAttributeOptionForAttribute;
        }

        var valueResult = VariantAttributeValue.Create(
            variantId: Id,
            attributeId: attributeId,
            attributeOptionId: attributeOptionId,
            value: value,
            attributeType: attributeType);

        if (valueResult.IsError)
            return valueResult.Errors;

        _variantAttributeValues.Add(valueResult.Value);
        AddDomainEvent(new VariantAttributeAddedEvent(Id, attributeId));
        return valueResult.Value;
    }

    public ErrorOr<Updated> RemoveVariantAttributeValue(Guid attributeId)
    {
        var value = _variantAttributeValues.FirstOrDefault(v => v.AttributeId == attributeId);
        if (value == null)
            return DomainErrors.VariantAttributeValue.NotFound;

        if (Product.Group.AttributeGroupAttributes.Any(aga => aga.AttributeId == attributeId && aga.Attribute.IsRequired))
            return DomainErrors.VariantAttributeValue.CannotRemoveRequiredAttribute;

        _variantAttributeValues.Remove(value);
        AddDomainEvent(new VariantAttributeRemovedEvent(Id, attributeId));
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveAllVariantAttributeValues()
    {
        var requiredAttributes = Product.Group.AttributeGroupAttributes
            .Where(aga => aga.Attribute.IsRequired)
            .Select(aga => aga.AttributeId)
            .ToList();

        if (requiredAttributes.Any())
            return DomainErrors.VariantAttributeValue.CannotRemoveRequiredAttribute;

        var removedAttributeIds = _variantAttributeValues.Select(v => v.AttributeId).ToList();
        _variantAttributeValues.Clear();

        foreach (var attributeId in removedAttributeIds)
        {
            AddDomainEvent(new VariantAttributeRemovedEvent(Id, attributeId));
        }

        return Result.Updated;
    }

    public ErrorOr<Updated> SetAsDefault()
    {
        IsDefault = true;
        AddDomainEvent(new VariantSetAsDefaultEvent(Id));
        return Result.Updated;
    }

    public ErrorOr<Updated> UnsetDefault()
    {
        IsDefault = false;
        return Result.Updated;
    }

    public ErrorOr<Updated> SetAvailability(bool isActive)
    {
        if (IsActive != isActive)
        {
            IsActive = false;
            AddDomainEvent(new VariantAvailabilityChangedEvent(Id, isActive));
        }

        return Result.Updated;
    }
    public ErrorOr<Updated> SetVisibility(bool isVisible)
    {
        if (IsVisible != isVisible)
        {
            IsVisible = isVisible;
            AddDomainEvent(new VariantVisibilityChangedEvent(Id, IsVisible));
        }
        return Result.Updated;
    }

    private void UpdateStockStatus()
    {
        StockStatus = StockQuantity <= 0
            ? StockStatus.OutOfStock
            : StockQuantity <= LowStockThreshold
                ? StockStatus.LowStock
                : StockStatus.InStock;
        if (IsActive && StockStatus == StockStatus.LowStock)
        {
            AddDomainEvent(new VariantLowStockThresholdEvent(Id));
        }
    }


    #endregion

    #region Domain Events
    public record ProductVariantCreatedEvent(Guid VariantId, Guid ProductId, string Sku) : IDomainEvent;
    public record ProductVariantUpdatedEvent(Guid VariantId, string Sku, decimal PriceOffset, int LowStockThreshold) : IDomainEvent;
    public record StockAdjustedEvent(Guid VariantId, int Quantity, MovementType MovementType) : IDomainEvent;
    public record StockStatusChangedEvent(Guid VariantId, StockStatus OldStatus, StockStatus NewStatus) : IDomainEvent;
    public record VariantAttributesUpdatedEvent(Guid VariantId, ICollection<Guid> AttributeIds) : IDomainEvent;
    public record VariantAttributeAddedEvent(Guid VariantId, Guid AttributeId) : IDomainEvent;
    public record VariantAttributeRemovedEvent(Guid VariantId, Guid AttributeId) : IDomainEvent;
    public record VariantSetAsDefaultEvent(Guid VariantId) : IDomainEvent;
    public record VariantAvailabilityChangedEvent(Guid VariantId, bool IsActive) : IDomainEvent;
    public record VariantVisibilityChangedEvent(Guid VariantId, bool IsVisible) : IDomainEvent;
    public record VariantLowStockThresholdEvent(Guid VariantId) : IDomainEvent;
    #endregion
}

using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.SharedKernel.Models.Domains;
using System.Text.RegularExpressions;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class ProductVariant : Entity<Guid>
{
    #region Properties
    public string Sku { get; private set; } = null!;
    public decimal PriceOffset { get; private set; }
    public int StockQuantity { get; private set; }
    public int LowStockThreshold { get; private set; }
    public StockStatus StockStatus { get; private set; } = StockStatus.InStock;
    public bool IsActive { get; private set; } = true;
    public bool IsDefault { get; private set; }
    #endregion

    #region Relationships
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    private readonly List<VariantAttributeOption> _variantAttributeOptions = new();
    private readonly List<StockMovement> _stockMovements = new();
    private readonly List<ProductImage> _variantImages = new();

    public IReadOnlyCollection<VariantAttributeOption> VariantAttributeOptions => _variantAttributeOptions.AsReadOnly();
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
        bool isDefault = false)
    {
        Id = Guid.NewGuid();
        Sku = sku;
        PriceOffset = priceOffset;
        StockQuantity = stockQuantity;
        LowStockThreshold = lowStockThreshold;
        ProductId = productId;
        IsDefault = isDefault;
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
        bool isDefault = false)
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

        var variant = new ProductVariant(sku, priceOffset, stockQuantity, lowStockThreshold, productId, isDefault);

        if (stockQuantity > 0)
        {
            var initialMovementResult = StockMovement.Create(
                variantId: variant.Id,
                quantity: stockQuantity,
                movementType: MovementType.Initial,
                notes: "Initial stock setup"
            );

            if (initialMovementResult.IsError)
                return initialMovementResult.Errors;

            variant._stockMovements.Add(initialMovementResult.Value);
        }

        variant.AddDomainEvent(new ProductVariantCreatedEvent(variant.Id, productId, sku));
        return variant;
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> UpdateDetails(
        string? sku = null,
        decimal? priceOffset = null,
        int? lowStockThreshold = null)
    {
        if (sku != null)
        {
            if (string.IsNullOrWhiteSpace(sku) || sku.Length < 3 || sku.Length > 50 || !Regex.IsMatch(sku, @"^[a-zA-Z0-9\-_]+$"))
                return DomainErrors.ProductVariant.InvalidSku;
            Sku = sku;
        }

        if (priceOffset.HasValue)
        {
            if (priceOffset.Value < -10000m || priceOffset.Value > 10000m)
                return DomainErrors.ProductVariant.InvalidPriceOffset;
            PriceOffset = priceOffset.Value;
        }

        if (lowStockThreshold.HasValue)
        {
            if (lowStockThreshold.Value < 0)
                return DomainErrors.ProductVariant.InvalidLowStockThreshold;
            LowStockThreshold = lowStockThreshold.Value;
            UpdateStockStatus();
        }

        return Result.Updated;
    }

    public ErrorOr<Updated> AdjustStock(int quantity, MovementType movementType, Guid? referenceId = null, string? notes = null)
    {
        if (movementType == MovementType.Adjustment)
        {
            if (quantity == 0)
                return DomainErrors.StockMovement.InvalidQuantity;
        }
        else
        {
            if (quantity <= 0)
                return DomainErrors.StockMovement.InvalidQuantity;
        }

        if (StockMovement.IsOutboundMovement(movementType) && StockQuantity + quantity < 0)
            return DomainErrors.ProductVariant.InsufficientStock;

        var stockMovementResult = StockMovement.Create(
            variantId: Id,
            quantity: Math.Abs(quantity),
            movementType: movementType,
            referenceId: referenceId,
            notes: notes
        );

        if (stockMovementResult.IsError)
            return stockMovementResult.Errors;

        _stockMovements.Add(stockMovementResult.Value);
        StockQuantity += stockMovementResult.Value.GetSignedQuantity();
        UpdateStockStatus();
        AddDomainEvent(new StockAdjustedEvent(Id, quantity, movementType));

        return Result.Updated;
    }

    public ErrorOr<bool> CheckStockAvailability(int quantity)
    {
        if (quantity <= 0)
            return DomainErrors.ProductVariant.InvalidStockCheckQuantity;

        return StockQuantity >= quantity;
    }

    public ErrorOr<Updated> SetAttributeOptions(ICollection<Guid> attributeOptionIds)
    {
        if (attributeOptionIds == null || !attributeOptionIds.Any())
            return DomainErrors.VariantAttributeOption.NoAttributeOptionsProvided;

        // Validate attribute options belong to the product's AttributeGroup
        var invalidOptions = attributeOptionIds.Where(id => id == Guid.Empty).ToList();
        if (invalidOptions.Any())
            return DomainErrors.VariantAttributeOption.InvalidAttributeOptionId;

        // Clear existing options and add new ones
        _variantAttributeOptions.Clear();
        var errors = new List<Error>();

        foreach (var optionId in attributeOptionIds)
        {
            if (_variantAttributeOptions.Any(opt => opt.AttributeOptionId == optionId))
                continue; // Skip duplicates

            var optionResult = VariantAttributeOption.Create(variantId: Id, optionId);
            if (optionResult.IsError)
            {
                errors.AddRange(optionResult.Errors);
                continue;
            }

            _variantAttributeOptions.Add(optionResult.Value);
        }

        if (errors.Any())
            return errors;

        AddDomainEvent(new VariantAttributesUpdatedEvent(Id, attributeOptionIds));
        return Result.Updated;
    }

    public ErrorOr<Updated> AddAttributeOption(Guid attributeOptionId)
    {
        if (attributeOptionId == Guid.Empty)
            return DomainErrors.VariantAttributeOption.InvalidAttributeOptionId;

        if (_variantAttributeOptions.Any(opt => opt.AttributeOptionId == attributeOptionId))
            return DomainErrors.VariantAttributeOption.DuplicateAttributeOption;

        var optionResult = VariantAttributeOption.Create(variantId: Id, attributeOptionId);
        if (optionResult.IsError)
            return optionResult.Errors;

        _variantAttributeOptions.Add(optionResult.Value);
        AddDomainEvent(new VariantAttributeAddedEvent(Id, attributeOptionId));
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveAttributeOption(Guid attributeOptionId)
    {
        var option = _variantAttributeOptions.FirstOrDefault(opt => opt.AttributeOptionId == attributeOptionId);
        if (option == null)
            return DomainErrors.VariantAttributeOption.AttributeOptionNotFound;

        _variantAttributeOptions.Remove(option);
        AddDomainEvent(new VariantAttributeRemovedEvent(Id, attributeOptionId));
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

    public ErrorOr<Updated> Activate()
    {
        IsActive = true;
        return Result.Updated;
    }

    public ErrorOr<Updated> Deactivate()
    {
        IsActive = false;
        return Result.Updated;
    }

    private void UpdateStockStatus()
    {
        StockStatus = StockQuantity <= 0
            ? StockStatus.OutOfStock
            : StockQuantity <= LowStockThreshold
                ? StockStatus.LowStock
                : StockStatus.InStock;
    }
    #endregion

    #region Domain Events
    public record ProductVariantCreatedEvent(Guid VariantId, Guid ProductId, string Sku) : IDomainEvent;

    public record StockAdjustedEvent(Guid VariantId, int Quantity, MovementType MovementType) : IDomainEvent;

    public record VariantAttributesUpdatedEvent(Guid VariantId, ICollection<Guid> AttributeOptionIds) : IDomainEvent;

    public record VariantAttributeAddedEvent(Guid VariantId, Guid AttributeOptionId) : IDomainEvent;

    public record VariantAttributeRemovedEvent(Guid VariantId, Guid AttributeOptionId) : IDomainEvent;

    public record VariantSetAsDefaultEvent(Guid VariantId) : IDomainEvent;
    #endregion
}

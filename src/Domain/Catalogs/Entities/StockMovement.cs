using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class StockMovement : Entity<Guid>
{
    #region Properties
    public Guid VariantId { get; private set; }
    public ProductVariant Variant { get; private set; } = default!;
    public int Quantity { get; private set; }
    public MovementType MovementType { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public string? Notes { get; private set; }
    #endregion

    #region Constructor
    private StockMovement() { }

    private StockMovement(
        Guid variantId,
        int quantity,
        MovementType movementType,
        Guid? referenceId,
        string? notes)
    {
        Id = Guid.NewGuid();
        VariantId = variantId;
        Quantity = quantity;
        MovementType = movementType;
        ReferenceId = referenceId;
        Notes = notes;
    }
    #endregion

    #region Factory
    public static ErrorOr<StockMovement> Create(
        Guid variantId,
        int quantity,
        MovementType movementType,
        Guid? referenceId = null,
        string? notes = null)
    {
        if (variantId == Guid.Empty)
            return DomainErrors.StockMovement.InvalidVariantId;

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

        var stockMovement = new StockMovement(
            variantId,
            quantity,
            movementType,
            referenceId,
            notes
        );

        return stockMovement;
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> Update(
        int quantity,
        MovementType movementType,
        Guid? referenceId = null,
        string? notes = null)
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

        Quantity = quantity;
        MovementType = movementType;
        ReferenceId = referenceId;
        Notes = notes;

        return Result.Updated;
    }

    public ErrorOr<Updated> AddNotes(string notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
            return DomainErrors.StockMovement.InvalidNotes;

        Notes = string.IsNullOrWhiteSpace(Notes)
            ? notes
            : $"{Notes}\n{notes}";

        return Result.Updated;
    }

    public ErrorOr<Updated> AssociateWithReference(Guid referenceId)
    {
        if (referenceId == Guid.Empty)
            return DomainErrors.StockMovement.InvalidReferenceId;

        ReferenceId = referenceId;
        return Result.Updated;
    }

    public static bool IsInboundMovement(MovementType movementType) =>
        movementType is MovementType.Purchase
                     or MovementType.Return
                     or MovementType.Initial;

    public static bool IsOutboundMovement(MovementType movementType) =>
        movementType is MovementType.Sale
                     or MovementType.Waste
                     or MovementType.Lost;

    public int GetSignedQuantity() =>
        MovementType == MovementType.Adjustment
            ? Quantity // Use quantity as is (can be positive or negative)
            : IsInboundMovement(MovementType)
                ? Quantity  // Positive for inbound
                : -Quantity; // Negative for outbound

    public string GetFormattedNotes() =>
        string.IsNullOrWhiteSpace(Notes) ? "No notes provided." : Notes;
    #endregion
}

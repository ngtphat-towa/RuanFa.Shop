using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class VariantAttributeOption : Entity<Guid>
{
    #region Properties
    public Guid VariantId { get; private set; }
    public Guid AttributeOptionId { get; private set; }
    #endregion

    #region Relationships
    public ProductVariant Variant { get; private set; } = null!;
    public AttributeOption AttributeOption { get; private set; } = null!;
    #endregion

    #region Constructor
    private VariantAttributeOption() { } // For EF Core

    private VariantAttributeOption(Guid variantId, Guid attributeOptionId)
    {
        Id = Guid.NewGuid();
        VariantId = variantId;
        AttributeOptionId = attributeOptionId;
    }
    #endregion

    #region Factory
    public static ErrorOr<VariantAttributeOption> Create(
        Guid variantId,
        Guid attributeOptionId)
    {
        if (variantId == Guid.Empty)
            return DomainErrors.VariantAttributeOption.InvalidVariantId;

        if (attributeOptionId == Guid.Empty)
            return DomainErrors.VariantAttributeOption.InvalidAttributeOptionId;

        var variantAttributeOption = new VariantAttributeOption(variantId, attributeOptionId);
        variantAttributeOption.AddDomainEvent(new VariantAttributeOptionCreatedEvent(
            variantAttributeOption.Id,
            variantId,
            attributeOptionId));
        return variantAttributeOption;
    }
    #endregion

    #region Domain Events
    public record VariantAttributeOptionCreatedEvent(Guid Id, Guid VariantId, Guid AttributeOptionId) : IDomainEvent;

    public record VariantAttributeOptionRemovedEvent(Guid Id, Guid VariantId, Guid AttributeOptionId) : IDomainEvent;
    #endregion
}

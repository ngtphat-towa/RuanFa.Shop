using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class AttributeGroupAttribute : Entity<Guid>
{
    #region Properties
    public Guid AttributeGroupId { get; private set; }
    public Guid AttributeId { get; private set; }
    #endregion

    #region Relationships
    public AttributeGroup AttributeGroup { get; private set; } = null!;
    public CatalogAttribute Attribute { get; private set; } = null!;
    #endregion

    #region Constructor
    private AttributeGroupAttribute() { } // For EF Core

    private AttributeGroupAttribute(Guid attributeGroupId, Guid attributeId)
    {
        Id = Guid.NewGuid();
        AttributeGroupId = attributeGroupId;
        AttributeId = attributeId;
    }
    #endregion

    #region Factory
    public static ErrorOr<AttributeGroupAttribute> Create(
        Guid attributeGroupId,
        Guid attributeId)
    {
        if (attributeGroupId == Guid.Empty)
            return DomainErrors.AttributeGroupAttribute.InvalidGroupId;

        if (attributeId == Guid.Empty)
            return DomainErrors.AttributeGroupAttribute.InvalidAttributeId;

        var attributeGroupAttribute = new AttributeGroupAttribute(attributeGroupId, attributeId);
        attributeGroupAttribute.AddDomainEvent(new AttributeGroupAttributeCreatedEvent(attributeGroupAttribute.Id, attributeGroupId, attributeId));
        return attributeGroupAttribute;
    }
    #endregion

    #region Domain Events
    public record AttributeGroupAttributeCreatedEvent(Guid Id, Guid AttributeGroupId, Guid AttributeId) : IDomainEvent;

    public record AttributeGroupAttributeRemovedEvent(Guid Id, Guid AttributeGroupId, Guid AttributeId) : IDomainEvent;
    #endregion
}

using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Errors;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class AttributeGroupAttribute
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
        return attributeGroupAttribute;
    }
    #endregion
}

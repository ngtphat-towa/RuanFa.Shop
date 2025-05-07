using ErrorOr;
using RuanFa.Shop.Domain.Attributes.Errors;

namespace RuanFa.Shop.Domain.Attributes.Entities;
public class AttributeGroupAttribute
{
    #region Relationships
    public virtual Guid AttributeGroupId { get; private set; }
    public virtual AttributeGroup AttributeGroup { get; private set; } = null!;
    public virtual Guid AttributeId { get; private set; }
    public virtual CatalogAttribute Attribute { get; private set; } = null!;
    #endregion

    #region Constructor
    private AttributeGroupAttribute() { }
    #endregion

    #region Methods
    public static ErrorOr<AttributeGroupAttribute> Create(
        Guid attributeGroupId,
        Guid attributeId)
    {
        // Validate input
        if (attributeGroupId == Guid.Empty)
        {
            return DomainErrors.AttributeGroupAttribute.InvalidGroupId;
        }

        if (attributeId == Guid.Empty)
        {
            return DomainErrors.AttributeGroupAttribute.InvalidAttributeId;
        }

        // Create and return the new AttributeGroupCollection
        return new AttributeGroupAttribute
        {
            AttributeGroupId = attributeGroupId,
            AttributeId = attributeId
        };
    }
    #endregion
}

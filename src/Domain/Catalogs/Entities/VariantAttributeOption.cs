using ErrorOr;
using RuanFa.Shop.Domain.Attributes.Entities;
using RuanFa.Shop.Domain.Catalogs.Errors;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class VariantAttributeOption
{
    #region Relationships
    public Guid VariantId { get; private set; }
    public ProductVariant Variant { get; private set; } = null!;
    public Guid AttributeOptionId { get; private set; }
    public AttributeOption AttributeOption { get; private set; } = null!;
    #endregion

    #region Constructor
    private VariantAttributeOption() { }
    #endregion

    #region Methods
    public static ErrorOr<VariantAttributeOption> Create(
        Guid variantId,
        Guid attributeOptionId)
    {
        // Validate input
        if (variantId == Guid.Empty)
        {
            return DomainErrors.VariantAttributeOption.InvalidVariantId;
        }

        if (attributeOptionId == Guid.Empty)
        {
            return DomainErrors.VariantAttributeOption.InvalidAttributeOptionId;
        }

        return new VariantAttributeOption
        {
            AttributeOptionId = variantId,
            VariantId = attributeOptionId
        };
    }
    #endregion
}

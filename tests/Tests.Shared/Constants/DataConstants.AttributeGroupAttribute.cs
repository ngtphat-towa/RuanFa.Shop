using RuanFa.Shop.Domain.Attributes.Entities;

namespace RuanFa.Shop.Tests.Shared.Constants;

public static partial class DataConstants
{
    public static class AttributeGroupAttributes
    {
        public static readonly AttributeGroupAttribute GeneralColor = AttributeGroupAttribute.Create(
            attributeGroupId: AttributeGroups.GeneralGroup.Id,
            attributeId: DataConstants.CatalogAttributes.ColorAttribute.Id
        ).Value;

        public static readonly AttributeGroupAttribute GeneralSize = AttributeGroupAttribute.Create(
            attributeGroupId: DataConstants.AttributeGroups.GeneralGroup.Id,
            attributeId: DataConstants.CatalogAttributes.SizeAttribute.Id
        ).Value;

        public static readonly AttributeGroupAttribute GeneralMaterial = AttributeGroupAttribute.Create(
            attributeGroupId: DataConstants.AttributeGroups.GeneralGroup.Id,
            attributeId: DataConstants.CatalogAttributes.MaterialAttribute.Id
        ).Value;

        public static readonly AttributeGroupAttribute GeneralBrand = AttributeGroupAttribute.Create(
            attributeGroupId: DataConstants.AttributeGroups.GeneralGroup.Id,
            attributeId: DataConstants.CatalogAttributes.BrandAttribute.Id
        ).Value;

        public static readonly List<AttributeGroupAttribute> DefaultAttributeGroupAttributes = new()
        {
            GeneralColor,
            GeneralSize,
            GeneralMaterial,
            GeneralBrand
        };
    }
}

using RuanFa.Shop.Domain.Attributes;
using RuanFa.Shop.Domain.Attributes.Enums;

namespace RuanFa.Shop.Tests.Shared.Constants;

public static partial class DataConstants
{
    public static class CatalogAttributes
    {
        // Constants for attribute codes and names
        public const string ColorCode = "color";
        public const string ColorName = "Color";
        public const string SizeCode = "size";
        public const string SizeName = "Size";
        public const string MaterialCode = "material";
        public const string MaterialName = "Material";
        public const string BrandCode = "brand";
        public const string BrandName = "Brand";

        // Default Catalog Attributes
        public static readonly Domain.Attributes.CatalogAttribute ColorAttribute =
            Domain.Attributes.CatalogAttribute.Create(
                attributeCode: ColorCode,
                attributeName: ColorName,
                type: AttributeType.Text,
                isRequired: true,
                displayOnFrontend: true,
                sortOrder: 1,
                isFilterable: true).Value;

        public static readonly CatalogAttribute SizeAttribute =
             Domain.Attributes.CatalogAttribute.Create(
                attributeCode: SizeCode,
                attributeName: SizeName,
                type: AttributeType.Select,
                isRequired: true,
                displayOnFrontend: true,
                sortOrder: 2,
                isFilterable: true).Value;


        public static readonly Domain.Attributes.CatalogAttribute MaterialAttribute =
            CatalogAttribute.Create(
                attributeCode: MaterialCode,
                attributeName: MaterialName,
                type: AttributeType.Text,
                isRequired: false,
                displayOnFrontend: false,
                sortOrder: 3,
                isFilterable: false).Value;

        public static readonly CatalogAttribute BrandAttribute =
            CatalogAttribute.Create(
                attributeCode: BrandCode,
                attributeName: BrandName,
                type: AttributeType.Select,
                isRequired: true,
                displayOnFrontend: true,
                sortOrder: 4,
                isFilterable: true).Value;

        // List of attributes for general use
        public static readonly List<Domain.Attributes.CatalogAttribute> DefaultAttributes = new()
        {
            ColorAttribute,
            SizeAttribute,
            MaterialAttribute,
            BrandAttribute,
        };
    }
}

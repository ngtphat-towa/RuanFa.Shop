using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Enums;

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
        public static readonly CatalogAttribute ColorAttribute =
            CatalogAttribute.Create(
                code: ColorCode,
                name: ColorName,
                type: AttributeType.Text,
                isRequired: true,
                displayOnFrontend: true,
                sortOrder: 1,
                isFilterable: true).Value;

        public static readonly CatalogAttribute SizeAttribute =
             CatalogAttribute.Create(
                code: SizeCode,
                name: SizeName,
                type: AttributeType.Select,
                isRequired: true,
                displayOnFrontend: true,
                sortOrder: 2,
                isFilterable: true).Value;


        public static readonly CatalogAttribute MaterialAttribute =
            CatalogAttribute.Create(
                code: MaterialCode,
                name: MaterialName,
                type: AttributeType.Text,
                isRequired: false,
                displayOnFrontend: false,
                sortOrder: 3,
                isFilterable: false).Value;

        public static readonly CatalogAttribute BrandAttribute =
            CatalogAttribute.Create(
                code: BrandCode,
                name: BrandName,
                type: AttributeType.Select,
                isRequired: true,
                displayOnFrontend: true,
                sortOrder: 4,
                isFilterable: true).Value;

        // List of attributes for general use
        public static readonly List<CatalogAttribute> DefaultAttributes = new()
        {
            ColorAttribute,
            SizeAttribute,
            MaterialAttribute,
            BrandAttribute,
        };
    }
}

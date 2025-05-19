using RuanFa.Shop.Domain.Catalogs.Entities;

namespace RuanFa.Shop.Tests.Shared.Constants;

public static partial class DataConstants
{
    public static class AttributeOptions
    {
        #region Color Options

        public static readonly AttributeOption Red = AttributeOption.Create(
            CatalogAttributes.ColorAttribute.Id,
            CatalogAttributes.ColorCode,
            "Red").Value;

        public static readonly AttributeOption Blue = AttributeOption.Create(
            CatalogAttributes.ColorAttribute.Id,
            CatalogAttributes.ColorCode,
            "Blue").Value;

        public static readonly AttributeOption Green = AttributeOption.Create(
            CatalogAttributes.ColorAttribute.Id,
            CatalogAttributes.ColorCode,
            "Green").Value;

        public static readonly AttributeOption Black = AttributeOption.Create(
            CatalogAttributes.ColorAttribute.Id,
            CatalogAttributes.ColorCode,
            "Black").Value;

        public static readonly AttributeOption White = AttributeOption.Create(
            CatalogAttributes.ColorAttribute.Id,
            CatalogAttributes.ColorCode,
            "White").Value;

        public static readonly AttributeOption DefaultColorOption = Red;

        public static readonly List<AttributeOption> ColorOptions = new()
        {
            Red, Blue, Green, Black, White
        };

        #endregion

        #region Size Options

        public static readonly AttributeOption Small = AttributeOption.Create(
            CatalogAttributes.SizeAttribute.Id,
            CatalogAttributes.SizeCode,
            "Small").Value;

        public static readonly AttributeOption Medium = AttributeOption.Create(
            CatalogAttributes.SizeAttribute.Id,
            CatalogAttributes.SizeCode,
            "Medium").Value;

        public static readonly AttributeOption Large = AttributeOption.Create(
            CatalogAttributes.SizeAttribute.Id,
            CatalogAttributes.SizeCode,
            "Large").Value;

        public static readonly AttributeOption XLarge = AttributeOption.Create(
            CatalogAttributes.SizeAttribute.Id,
            CatalogAttributes.SizeCode,
            "X-Large").Value;

        public static readonly AttributeOption DefaultSizeOption = Medium;

        public static readonly List<AttributeOption> SizeOptions = new()
        {
            Small, Medium, Large, XLarge
        };

        #endregion

        #region Material Options

        public static readonly AttributeOption Cotton = AttributeOption.Create(
            CatalogAttributes.MaterialAttribute.Id,
            CatalogAttributes.MaterialCode,
            "Cotton").Value;

        public static readonly AttributeOption Polyester = AttributeOption.Create(
            CatalogAttributes.MaterialAttribute.Id,
            CatalogAttributes.MaterialCode,
            "Polyester").Value;

        public static readonly AttributeOption Leather = AttributeOption.Create(
            CatalogAttributes.MaterialAttribute.Id,
            CatalogAttributes.MaterialCode,
            "Leather").Value;

        public static readonly AttributeOption DefaultMaterialOption = Cotton;

        public static readonly List<AttributeOption> MaterialOptions = new()
        {
            Cotton, Polyester, Leather
        };

        #endregion

        #region Brand Options

        public static readonly AttributeOption Nike = AttributeOption.Create(
            CatalogAttributes.BrandAttribute.Id,
            CatalogAttributes.BrandCode,
            "Nike").Value;

        public static readonly AttributeOption Adidas = AttributeOption.Create(
            CatalogAttributes.BrandAttribute.Id,
            CatalogAttributes.BrandCode,
            "Adidas").Value;

        public static readonly AttributeOption Puma = AttributeOption.Create(
            CatalogAttributes.BrandAttribute.Id,
            CatalogAttributes.BrandCode,
            "Puma").Value;

        public static readonly AttributeOption Reebok = AttributeOption.Create(
            CatalogAttributes.BrandAttribute.Id,
            CatalogAttributes.BrandCode,
            "Reebok").Value;

        public static readonly AttributeOption DefaultBrandOption = Nike;

        public static readonly List<AttributeOption> BrandOptions = new()
        {
            Nike, Adidas, Puma, Reebok
        };

        #endregion
    }
}

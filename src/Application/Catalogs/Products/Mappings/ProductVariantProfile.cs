using Mapster;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Attributes;
using RuanFa.Shop.Application.Catalogs.Products.Models.Products;
using RuanFa.Shop.Application.Catalogs.Products.Models.Varaiants;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Enums;

namespace RuanFa.Shop.Application.Catalogs.Products.Mappings;

internal class ProductVariantProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        List<AttributeType> opttionAttibuteTypes = [
            AttributeType.Dropdown,
            AttributeType.Swatch];
        config.NewConfig<ProductVariant, VariantResult>()
            .MapWith(variant => new VariantResult()
            {
                Id = variant.Id,
                Sku = variant.Sku,
                PriceOffset = variant.PriceOffset,
                StockQuantity = variant.StockQuantity,
                LowStockThreshold = variant.LowStockThreshold,
                IsActive = variant.IsActive,
                IsVisible = variant.IsVisible,
                ProductId = variant.ProductId,
                AttributeValues = variant.VariantAttributeValues
                    .Select(av => new VariantAttibuteValueResult
                    {
                        Id = av.Id,
                        Name = av.Attribute.Name,
                        Type = (int)av.Attribute.Type,
                        OptionId = av.AttributeOptionId,
                        Value = opttionAttibuteTypes.Contains(av.Attribute.Type)
                                    ? (av.AttributeOption != null ? av.AttributeOption.OptionValue : null)
                                    : av.Value
                    })
                    .ToList(),
                Images = variant.VariantImages
                    .Select(i => new  ProductImageResult
                    {
                        Id = i.Id,
                        VariantId = i.VariantId,
                        Url = i.Image.Url,
                        Alt = i.Image.Alt,
                        ImageType = (int)i.Image.ImageType,
                        IsDefault = i.IsDefault
                    })
                    .ToList()
            });

        config.NewConfig<CatalogAttribute, AttributeListResult>()
            .Map(dest => dest.Type, src => (int)src.Type)
            .Map(dest => dest.OptionCount, src => src.AttributeOptions.Count)
            .Map(dest => dest.GroupCount, src => src.AttributeGroupAttributes.Count);
    }
}

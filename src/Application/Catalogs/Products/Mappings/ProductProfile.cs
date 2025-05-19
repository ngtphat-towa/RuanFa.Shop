using Mapster;
using RuanFa.Shop.Application.Catalogs.Products.Models.Products;
using RuanFa.Shop.Application.Catalogs.Products.Models.Varaiants;
using RuanFa.Shop.Application.Common.Models.Results;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Commons.Enums;

namespace RuanFa.Shop.Application.Catalogs.Products.Mappings;

internal class ProductProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        List<AttributeType> opttionAttibuteTypes = [
            AttributeType.Dropdown,
            AttributeType.Swatch];
        config.NewConfig<Product, ProductListResult>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Sku, src => src.Sku)
            .Map(dest => dest.BasePrice, src => src.BasePrice)
            .Map(dest => dest.Weight, src => src.Weight)
            .Map(dest => dest.SalePrice, src => src.SalePrice)
            .Map(dest => dest.IsFeatured, src => src.IsFeatured)
            .Map(dest => dest.TaxClass, src => (int)src.TaxClass)
            .Map(dest => dest.Status, src => (int)src.Status)
            .Map(dest => dest.Images, src => src.Images.Select(i => new ProductImageResult
            {
                Id = i.Id,
                VariantId = i.VariantId,
                Url = i.Image.Url,
                Alt = i.Image.Alt,
                ImageType = (int)i.Image.ImageType,
                IsDefault = i.IsDefault
            }).ToList())
            .Map(dest => dest.Descriptions,
                src => src.Descriptions != null
                    ? src.Descriptions
                        .Where(m => m.Type == DescriptionType.General)
                        .Select(description => new DescriptionDataResult
                        {
                            Type = (int)description.Type,
                            Value = description.Value
                        }).ToList()
                    : new List<DescriptionDataResult>())
            .Map(dest => dest.Group, src => new ProductListResult.ProductAtttibuteGroupResult
            {
                Id = src.Group.Id,
                Name = src.Group.Name
            })
            .Map(dest => dest.Category, src => src.Category != null
                ? new ProductListResult.ProductCategoryResult
                {
                    CategoryId = src.Category.Id,
                    CategoryName = src.Category.Name
                }
                : null)
            .Map(dest => dest.CollectionCount, src => src.ProductCollections.Count)
            .Map(dest => dest.CollectionNames,
                src => string.Join(", ", src.ProductCollections
                    .Select(pc => pc.Collection.Name)
                    .Where(name => !string.IsNullOrEmpty(name))))
            .Map(dest => dest.VariantCount, src => src.Variants.Count)
            .Map(dest => dest.StockQuantity, src => src.Variants.Sum(v => v.StockQuantity))
            .Map(dest => dest.StockStatus, src =>
                src.Variants.Sum(v => v.StockQuantity) > 0
                    ? StockStatus.InStock
                    : StockStatus.OutOfStock
            );

        config.NewConfig<Product, ProductResult>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Sku, src => src.Sku)
            .Map(dest => dest.BasePrice, src => src.BasePrice)
            .Map(dest => dest.Weight, src => src.Weight)
            .Map(dest => dest.SalePrice, src => src.SalePrice)
            .Map(dest => dest.IsFeatured, src => src.IsFeatured)
            .Map(dest => dest.TaxClass, src => (int)src.TaxClass)
            .Map(dest => dest.Status, src => (int)src.Status)
            .Map(dest => dest.Images, src => src.Images.Select(i => new ProductImageResult
            {
                Id = i.Id,
                VariantId = i.VariantId,
                Url = i.Image.Url,
                Alt = i.Image.Alt,
                ImageType = (int)i.Image.ImageType,
                IsDefault = i.IsDefault
            }).ToList())
            .Map(dest => dest.Descriptions,
                src => src.Descriptions != null
                    ? src.Descriptions
                        .Where(m => m.Type == DescriptionType.General)
                        .Select(description => new DescriptionDataResult
                        {
                            Type = (int)description.Type,
                            Value = description.Value
                        }).ToList()
                    : new List<DescriptionDataResult>())
            .Map(dest => dest.Group, src => new ProductListResult.ProductAtttibuteGroupResult
            {
                Id = src.Group.Id,
                Name = src.Group.Name
            })
            .Map(dest => dest.Category, src => src.Category != null
                ? new ProductListResult.ProductCategoryResult
                {
                    CategoryId = src.Category.Id,
                    CategoryName = src.Category.Name
                }
                : null)
            .Map(dest => dest.Collections,
                src => src.ProductCollections
                .Select(pc => pc.Collection)
                .Select(c => new ProductResult.ProductCollectionResult
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList())
            .Map(dest => dest.DefaultVariant,
                src => src.Variants
                .Where(v => v.IsDefault)
                .Select(v => new VariantResult
                {
                    Id = v.Id,
                    Sku = v.Sku,
                    PriceOffset = v.PriceOffset,
                    StockQuantity = v.StockQuantity,
                    StockStatus = (int)(v.StockQuantity > 0
                        ? StockStatus.InStock
                        : StockStatus.OutOfStock),
                    IsDefault = v.IsDefault,
                    IsActive = v.IsActive,
                    IsVisible = v.IsVisible,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                    AttributeValues = v.VariantAttributeValues != null
                        ? v.VariantAttributeValues.Select(av => new VariantAttibuteValueResult
                        {
                            Id = av.Id,
                            Name = av.Attribute.Name,
                            Type = (int)av.Attribute.Type,
                            OptionId = av.AttributeOptionId,
                            Value = opttionAttibuteTypes.Contains(av.Attribute.Type)
                                    ? (av.AttributeOption != null ? av.AttributeOption.OptionValue : null)
                                    : av.Value
                        }).ToList()
                        : new List<VariantAttibuteValueResult>()
                }).FirstOrDefault());
    }
}

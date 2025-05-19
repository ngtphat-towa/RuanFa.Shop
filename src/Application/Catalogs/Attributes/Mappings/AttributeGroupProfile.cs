using Mapster;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Groups;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Mappings;

public class AttributeGroupProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CatalogAttribute, AttributeGroupResult.AttributeResult>()
            .Map(dest => dest.Type, src => (int)src.Type);
        config.NewConfig<AttributeGroup, AttributeGroupResult>()
            .Map(dest => dest.Attributes,
                 src => src.AttributeGroupAttributes
                 .Select(m => m.AttributeGroup.Adapt<AttributeGroupResult.AttributeResult>())
                 .ToList());
        config.NewConfig<AttributeGroup, AttributeGroupListResult>()
            .Map(dest => dest.ProductCount, src => src.Products.Count)
            .Map(dest => dest.AttributeCount, src => src.AttributeGroupAttributes.Count);
    }
}

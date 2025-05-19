using Mapster;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Attributes;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Entities;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Mappings;

public class CatalogAttributeProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AttributeGroup, AttributeResult.AttributeGroupResult>();
        config.NewConfig<AttributeOption, AttributeResult.AttributeOptionResult>();

        config.NewConfig<CatalogAttribute, AttributeResult>()
            .Map(dest => dest.Groups,
                 src => src.AttributeGroupAttributes
                 .Select(m => m.AttributeGroup.Adapt<AttributeResult.AttributeGroupResult>())
                 .ToList())
            .Map(dest => dest.Groups,
                 src => src.AttributeOptions
                 .Select(m => m.Adapt<AttributeResult.AttributeOptionResult>())
                 .ToList());

        config.NewConfig<CatalogAttribute, AttributeListResult>()
            .Map(dest => dest.Type, src => (int)src.Type)
            .Map(dest => dest.OptionCount, src => src.AttributeOptions.Count)
            .Map(dest => dest.GroupCount, src => src.AttributeGroupAttributes.Count);
    }
}

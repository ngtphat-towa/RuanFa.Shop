using Mapster;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Options;
using RuanFa.Shop.Domain.Catalogs.Entities;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Mappings;

public class AttributeOptionProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AttributeOption, AttributeOptionResult>()
            .Map(dest => dest.AttributeId,
                 src => src.Attribute.Id)
            .Map(dest => dest.AttributeCode,
                 src => src.Attribute.Code)
            .Map(dest => dest.AttributeName,
                 src => src.Attribute.Name);

        config.NewConfig<AttributeOption, AttributeOptionListResult>()
            .Map(dest => dest.AttributeId,
                 src => src.Attribute.Id)
            .Map(dest => dest.AttributeCode,
                 src => src.Attribute.Code)
            .Map(dest => dest.VariantCount,
                 src => src.VariantAttributeOptions.Count);
    }
}

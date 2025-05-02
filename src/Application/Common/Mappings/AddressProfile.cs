using Mapster;
using RuanFa.Shop.Application.Common.Models;
using RuanFa.Shop.Domain.Commons.ValueObjects;

namespace RuanFa.Shop.Application.Common.Mappings;

public class AddressProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Address, AddressReult>();

        config.NewConfig<UserAddress, UserAddressResult>()
            .Map(dest => dest.Type, src => (int)src.Type);
    }
}


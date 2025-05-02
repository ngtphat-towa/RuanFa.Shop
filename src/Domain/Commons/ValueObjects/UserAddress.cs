using RuanFa.Shop.Domain.Commons.Enums;

namespace RuanFa.Shop.Domain.Commons.ValueObjects;

public class UserAddress : Address
{
    public AddressType Type { get; set; }
    public bool IsDefault { get; set; }
}

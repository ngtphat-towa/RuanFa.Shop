using RuanFa.Shop.Domain.Commons.Enums;

namespace RuanFa.Shop.Domain.Commons.ValueObjects;

public class UserAddress : Address
{
    public string? DeliveryInstructions { get; set; }
    public string? BoutiquePickupLocation { get; set; }
    public AddressType Type { get; set; }
    public bool IsDefault { get; set; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Type;
        yield return IsDefault;
        yield return PostalCode;
        yield return AddressLine1;
        yield return AddressLine2;
        yield return Country;
        yield return State;
        yield return City;
    }

    public static UserAddress Create(
        string addressLine1,
        string? addressLine2,
        string city,
        string state,
        string country,
        string postalCode,
        string? deliveryInstructions,
        string? boutiquePickupLocation,
        AddressType type,
        bool isDefault)
    {
        return new UserAddress
        {
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            City = city,
            State = state,
            Country = country,
            PostalCode = postalCode,
            DeliveryInstructions = deliveryInstructions,
            BoutiquePickupLocation = boutiquePickupLocation,
            Type = type,
            IsDefault = isDefault
        };
    }
}

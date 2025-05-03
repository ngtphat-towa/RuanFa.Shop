using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Commons.ValueObjects;

public class Address : ValueObject
{
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string PostalCode { get; set; } = null!;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PostalCode;
        yield return AddressLine1;
        yield return AddressLine2;
        yield return Country;
        yield return State;
        yield return City;
    }

    public static Address CreateAddress(
       string addressLine1,
       string? addressLine2,
       string city,
       string state,
       string country,
       string postalCode)
    {
        return new Address
        {
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            City = city,
            State = state,
            Country = country,
            PostalCode = postalCode
        };
    }
}

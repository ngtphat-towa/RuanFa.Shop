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
}

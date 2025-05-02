namespace RuanFa.Shop.Application.Common.Models;

public record AddressReult
{
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
}

public record UserAddressResult : AddressReult
{
    public bool IsDefault { get; set; }
    public int Type { get; set; }
}

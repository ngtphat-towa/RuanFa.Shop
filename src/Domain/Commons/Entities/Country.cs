using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Commons.Entities;

public class Country : Entity<int>
{
    public string Name { get; set; } = null!;
    public string? Currency { get; set; }
    public string? CurrencySymbol { get; set; }
    public string? Iso3166_2 { get; set; }
    public string? Iso3166_3 { get; set; }
    public string? CallingCode { get; set; }
    public string? Flag { get; set; }
    public ICollection<State>? States { get; set; }
}

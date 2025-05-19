namespace RuanFa.Shop.Application.Catalogs.Products.Models.QueryParams;
public record ProuctAttributeFilterParams
{
    public Guid Id { get; set; }
    public List<string>? Value { get; set; }
}

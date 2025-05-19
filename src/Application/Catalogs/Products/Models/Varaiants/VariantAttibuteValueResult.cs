namespace RuanFa.Shop.Application.Catalogs.Products.Models.Varaiants;
public record VariantAttibuteValueResult
{
    public Guid Id { get; set; }
    public int Type { get; set; }
    public string? Name { get; set; }
    public Guid? OptionId { get; set; }
    public string? Value { get; set; }
}


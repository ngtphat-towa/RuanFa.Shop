namespace RuanFa.Shop.Application.Catalogs.Attributes.Models.Options;
public class AttributeOptionListResult
{
    public Guid Id { get; init; }
    public Guid AttributeId { get; init; }
    public string? AttributeCode { get; init; }
    public string? Code { get; init; } = null!;
    public string? OptionText { get; init; } = null!;
    public int VariantCount { get; set; }
}

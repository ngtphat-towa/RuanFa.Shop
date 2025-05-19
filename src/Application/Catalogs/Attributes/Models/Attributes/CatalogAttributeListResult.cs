namespace RuanFa.Shop.Application.Catalogs.Attributes.Models.Attributes;
public record CatalogAttributeListResult
{
    public Guid Id { get; init; }
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public int Type { get; init; }
    public bool IsRequired { get; init; }
    public bool DisplayOnFrontend { get; init; }
    public int SortOrder { get; init; }
    public bool IsFilterable { get; init; }
    public int OptionCount { get; init; }
    public int GroupCount { get; init; }
}

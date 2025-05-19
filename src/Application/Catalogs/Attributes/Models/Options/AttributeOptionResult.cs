namespace RuanFa.Shop.Application.Catalogs.Attributes.Models.Options;

public record AttributeOptionResult
{
    public Guid Id { get; init; }
    public Guid AttributeId { get; init; }
    public string? AttributeCode { get; init; }
    public string? AttributeName { get; init; }
    public string Value { get; init; } = null!;
}

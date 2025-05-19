using RuanFa.Shop.Domain.Catalogs.Enums;

namespace RuanFa.Shop.Application.Catalogs.Products.Models.Data;

public record AttributeValueInput
{
    public Guid AttributeId { get; private set; }
    public Guid? AttributeOptionId { get; private set; }
    public AttributeType AttributeType { get; private set; }
    public string? Value { get; private set; }
}

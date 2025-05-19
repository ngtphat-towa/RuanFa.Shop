namespace RuanFa.Shop.Application.Catalogs.Attributes.Models.Groups;
public record AttributeGroupResult
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<AttributeResult>? Attributes { get; set; }

    public record AttributeResult
    {
        public Guid Id { get; init; }
        public string Name { get; private set; } = null!;
        public string Code { get; private set; } = null!;
        public int Type { get; private set; }
    }
}

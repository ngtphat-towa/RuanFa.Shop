namespace RuanFa.Shop.Application.Catalogs.Attributes.Models.Attributes;

public record AttributeResult
{
    public Guid Id { get; init; }
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public int Type { get; init; }
    public bool IsRequired { get; init; }
    public bool DisplayOnFrontend { get; init; }
    public int SortOrder { get; init; }
    public bool IsFilterable { get; init; }
    public IReadOnlyCollection<AttributeOptionResult> Options { get; init; } = new List<AttributeOptionResult>();
    public IReadOnlyCollection<AttributeGroupResult> Groups { get; init; } = new List<AttributeGroupResult>();

    public record AttributeOptionResult
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = null!;
        public string OptionText { get; init; } = null!;
    }

    public record AttributeGroupResult
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
    }

}


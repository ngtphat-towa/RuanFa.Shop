namespace RuanFa.Shop.Application.Catalogs.Attributes.Models.Groups;
public class AttributeGroupListResult
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public int AttributeCount { get; init; } 
    public int ProductCount { get; init; }
    public DateTime? CreateAt { get; set;  }
}

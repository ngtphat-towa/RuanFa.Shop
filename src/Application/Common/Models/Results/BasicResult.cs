namespace RuanFa.Shop.Application.Common.Models.Results;
public record BasicResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

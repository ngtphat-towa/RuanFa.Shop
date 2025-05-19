namespace RuanFa.Shop.Application.Common.Models.Results;
public record DescriptionDataResult
{
    public int Type { get; set; }
    public string Value { get; set; } = string.Empty;
}

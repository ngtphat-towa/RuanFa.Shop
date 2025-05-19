using RuanFa.Shop.Domain.Commons.Enums;

namespace RuanFa.Shop.Application.Common.Models.Requests;
public record DescriptionDataInput
{
    public DescriptionType Type { get; set; }
    public string Value { get; set; } = null!;
}

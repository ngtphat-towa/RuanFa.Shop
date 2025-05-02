using RuanFa.Shop.SharedKernel.Enums;

namespace RuanFa.Shop.SharedKernel.Models.Wrappers;

public record FilterCriteria
{
    public required string Field { get; set; }
    public required FilterOperator Operator { get; set; }
    public object? Value { get; set; }
}

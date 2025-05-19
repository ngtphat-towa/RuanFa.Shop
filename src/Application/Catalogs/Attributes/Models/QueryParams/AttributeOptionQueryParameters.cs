using RuanFa.Shop.Application.Common.Models.Queries;
using RuanFa.Shop.Domain.Catalogs.Enums;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Models.QueryParams;
public record class AttributeOptionQueryParameters : QueryParameters
{
    public Guid? AttributeId { get; init; }
    public string? AttributeCode { get; init; }
    public AttributeType? Type { get; init; }
    public bool? IsRequired { get; init; }
    public bool? DisplayOnFrontend { get; init; }
    public bool? IsFilterable { get; init; }
}

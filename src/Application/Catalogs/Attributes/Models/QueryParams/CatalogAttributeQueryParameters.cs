using RuanFa.Shop.Application.Common.Models.Queries;
using RuanFa.Shop.Domain.Catalogs.Enums;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Models.QueryParams;

public record CatalogAttributeQueryParameters : QueryParameters
{
    public List<AttributeType>? Type { get; init; }
    public bool? IsRequired { get; init; }
    public bool? DisplayOnFrontend { get; init; }
    public bool? IsFilterable { get; init; }
}

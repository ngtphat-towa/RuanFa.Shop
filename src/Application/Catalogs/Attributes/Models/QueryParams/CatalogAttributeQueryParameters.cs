using RuanFa.Shop.Application.Common.Models.Queries;
using RuanFa.Shop.Domain.Catalogs.Enums;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Models.QueryParams;

public record CatalogAttributeQueryParameters : QueryParameters
{
    public AttributeType?[]? Type { get; set; }
    public bool? IsRequired { get; set; }
    public bool? DisplayOnFrontend { get; set; }
    public bool? IsFilterable { get; set; }
}

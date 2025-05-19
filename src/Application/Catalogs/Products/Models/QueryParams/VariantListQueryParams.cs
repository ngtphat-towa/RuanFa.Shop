using RuanFa.Shop.Application.Common.Models.Queries;

namespace RuanFa.Shop.Application.Catalogs.Products.Models.QueryParams;

public record VariantListQueryParams : QueryParameters
{
    public Guid? ProductId { get; set; } = null;
    public List<ProuctAttributeFilterParams>? Attributes { get; set; }
}

using RuanFa.Shop.Application.Common.Models.Queries;
using RuanFa.Shop.Domain.Catalogs.Enums;

namespace RuanFa.Shop.Application.Catalogs.Products.Models.QueryParams;
public record ProductListQueryParams : QueryParameters
{
    // Status & settings
    public TaxClass[]? TaxClass { get; set; }
    public ProductStatus[]? Status { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsVisible { get; set; }

    // Categorizations
    public Guid[]? GroupId { get; set; }
    public Guid[]? CategoryId { get; set; }

    // Attibute Filters
    public List<ProuctAttributeFilterParams>? Attributes { get; set; }
}

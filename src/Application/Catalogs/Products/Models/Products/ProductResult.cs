using RuanFa.Shop.Application.Catalogs.Products.Models.Varaiants;
using RuanFa.Shop.Application.Common.Models.Results;

namespace RuanFa.Shop.Application.Catalogs.Products.Models.Products;

public record ProductResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Sku { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? IsFeatured { get; set; }
    public decimal? Weight { get; set; }
    public int TaxClass { get; set; }
    public int Status { get; set; }
    public List<DescriptionDataResult>? Descriptions { get; }
    public VariantResult DefaultVariant { get; set; } = default!;

    public ProductAtttibuteGroupResult Group { get; set; } = new();
    public ProductCategoryResult Category { get; set; } = new();
    public List<ProductCollectionResult> Collections { get; set; } = new();
    public List<ProductImageResult> Images { get; set; } = new();

    public record ProductCategoryResult : BasicResult;
    public record ProductAtttibuteGroupResult : BasicResult;
    public record ProductCollectionResult : BasicResult;
}

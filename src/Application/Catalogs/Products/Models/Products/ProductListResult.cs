using RuanFa.Shop.Application.Common.Models.Results;

namespace RuanFa.Shop.Application.Catalogs.Products.Models.Products;

public record ProductListResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Sku { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? IsFeatured { get; set; }
    public decimal? Weight { get; set; }

    public int TaxClass { get; set; }

    // Images
    public List<ProductImageResult>? Images { get;  }
    public List<DescriptionDataResult>? Descriptions { get; }

    // Status
    public int Status { get; set; }

    // Total sums
    public int StockStatus { get; set; }
    public int StockQuantity { get; set; }

    // Categorizations
    public ProductCategoryResult? Category { get; set; }
    public ProductAtttibuteGroupResult Group { get; set; } = new();
    public int CollectionCount { get; set; }
    public string? CollectionNames { get; set; }

    // Active variants
    public int VariantCount { get; set; }

    public record ProductCategoryResult
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public record ProductAtttibuteGroupResult
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = null!;
    }
}

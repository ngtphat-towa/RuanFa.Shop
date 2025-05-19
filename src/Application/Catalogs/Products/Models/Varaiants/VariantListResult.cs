using RuanFa.Shop.Application.Catalogs.Products.Models.Products;

namespace RuanFa.Shop.Application.Catalogs.Products.Models.Varaiants;

public record VariantListResult
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductSku { get; set; } = null!;
    public string? Sku { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal PriceOffset { get; set; }
    public int StockQuantity { get; set; }
    public List<ProductImageResult>? Images { get; }
    public List<VariantAttibuteValueResult>? AttibuteValues { get; set;  }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}

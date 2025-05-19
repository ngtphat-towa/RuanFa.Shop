using RuanFa.Shop.Application.Catalogs.Products.Models.Products;

namespace RuanFa.Shop.Application.Catalogs.Products.Models.Varaiants;

public record VariantResult
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? Sku { get; set; } = null!;
    public decimal PriceOffset { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public int StockStatus { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; }
    public List<ProductImageResult>? Images { get; set; }
    public List<VariantAttibuteValueResult>? AttributeValues { get; set;  }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

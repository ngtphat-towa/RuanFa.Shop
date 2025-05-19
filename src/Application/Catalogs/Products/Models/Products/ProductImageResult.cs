namespace RuanFa.Shop.Application.Catalogs.Products.Models.Products;

public record ProductImageResult
{
    public Guid Id { get; set; }
    public Guid? VariantId { get; set; }
    public string Url { get; set; } = null!;
    public string Alt { get; set; } = null!;
    public int ImageType { get; set; } 
    public bool IsDefault { get; set; }
}

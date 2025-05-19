using RuanFa.Shop.Domain.Catalogs.Enums;

namespace RuanFa.Shop.Application.Catalogs.Products.Models.Data;

public class ProductImageInput
{
    public string Url { get; set; } = null!;
    public string Alt { get; set; } = null!;
    public ImageType ImageType { get; set; }
    public bool IsDefault { get; set; }
}

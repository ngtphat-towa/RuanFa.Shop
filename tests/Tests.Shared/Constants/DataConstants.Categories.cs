using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.ValueObjects;

namespace RuanFa.Shop.Tests.Shared.Constants;

public static partial class DataConstants
{
    public static class Categories
    {
        // Apparel (Master)
        public static readonly Guid ApparelId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Category Apparel = Category.Create(
            ApparelId, "Apparel", "apparel", true, true, 0, false,
            "Apparel overview", "All clothing types",
            CategoryImage.Create("apparel.jpg", "Apparel").Value,
            "Apparel Meta Title", "clothing, fashion", "Meta description for apparel"
        ).Value;

        public static readonly Category Jackets = CreateSubCategory("Jackets", "apparel/jackets", ApparelId, 1, "jackets.jpg");
        public static readonly Category TShirts = CreateSubCategory("T-Shirts", "apparel/t-shirts", ApparelId, 2, "tshirts.jpg");
        public static readonly Category Pants = CreateSubCategory("Pants", "apparel/pants", ApparelId, 3, "pants.jpg");
        public static readonly Category Dresses = CreateSubCategory("Dresses", "apparel/dresses", ApparelId, 4, "dresses.jpg");
        public static readonly Category Hoodies = CreateSubCategory("Hoodies", "apparel/hoodies", ApparelId, 5, "hoodies.jpg");

        // Footwear (Master)
        public static readonly Guid FootwearId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        public static readonly Category Footwear = Category.Create(
            FootwearId, "Footwear", "footwear", true, true, 0, false,
            "Shoes and more", "Explore all kinds of footwear",
            CategoryImage.Create("footwear.jpg", "Footwear").Value,
            "Footwear Meta", "shoes, sneakers", "Meta desc"
        ).Value;

        public static readonly Category Sneakers = CreateSubCategory("Sneakers", "footwear/sneakers", FootwearId, 1, "sneakers.jpg");
        public static readonly Category Boots = CreateSubCategory("Boots", "footwear/boots", FootwearId, 2, "boots.jpg");
        public static readonly Category Sandals = CreateSubCategory("Sandals", "footwear/sandals", FootwearId, 3, "sandals.jpg");
        public static readonly Category FormalShoes = CreateSubCategory("Formal Shoes", "footwear/formal", FootwearId, 4, "formal.jpg");

        // Accessories (Master)
        public static readonly Guid AccessoriesId = Guid.Parse("30000000-0000-0000-0000-000000000001");
        public static readonly Category Accessories = Category.Create(
            AccessoriesId, "Accessories", "accessories", true, true, 0, false,
            "Finish your look", "Bags, belts, and more",
            CategoryImage.Create("accessories.jpg", "Accessories").Value,
            "Accessories Meta", "accessories, bags", "Accessories meta desc"
        ).Value;

        public static readonly Category Bags = CreateSubCategory("Bags", "accessories/bags", AccessoriesId, 1, "bags.jpg");
        public static readonly Category Belts = CreateSubCategory("Belts", "accessories/belts", AccessoriesId, 2, "belts.jpg");
        public static readonly Category Hats = CreateSubCategory("Hats", "accessories/hats", AccessoriesId, 3, "hats.jpg");
        public static readonly Category Sunglasses = CreateSubCategory("Sunglasses", "accessories/sunglasses", AccessoriesId, 4, "sunglasses.jpg");

        // Collection
        public static readonly List<Category> DefaultCategories = new()
    {
        Apparel, Jackets, TShirts, Pants, Dresses, Hoodies,
        Footwear, Sneakers, Boots, Sandals, FormalShoes,
        Accessories, Bags, Belts, Hats, Sunglasses
    };

        private static Category CreateSubCategory(string name, string urlKey, Guid parentId, short position, string imageFile)
        {
            return Category.Create(
                id: Guid.NewGuid(),
                name: name,
                urlKey: urlKey,
                isActive: true,
                includeInNav: true,
                position: position,
                showProducts: true,
                shortDescription: $"Browse {name}",
                description: $"{name} in our fashion store",
                image: CategoryImage.Create(imageFile, name).Value,
                metaTitle: $"Buy {name} Online",
                metaKeywords: $"{name.ToLower()}",
                metaDescription: $"Top quality {name.ToLower()}",
                parentId: parentId
            ).Value;
        }
    }

}

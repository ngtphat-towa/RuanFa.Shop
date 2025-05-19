using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Enums;

namespace RuanFa.Shop.Tests.Shared.Constants;

public static partial class DataConstants
{
    public static class Products
    {
        // Apparel: Jackets
        public static readonly Product LeatherJacket = CreateProduct(
            name: "Leather Jacket",
            sku: "JACKET-LEATHER",
            basePrice: 149.99m,
            weight: 1.2m,
            categoryId: DataConstants.Categories.Jackets.Id
        );

        public static readonly Product DenimJacket = CreateProduct(
            name: "Denim Jacket",
            sku: "JACKET-DENIM",
            basePrice: 89.99m,
            weight: 0.9m,
            categoryId: DataConstants.Categories.Jackets.Id
        );

        // Apparel: TShirts
        public static readonly Product CottonTShirt = CreateProduct(
            name: "Cotton T-Shirt",
            sku: "TSHIRT-COTTON",
            basePrice: 19.99m,
            weight: 0.3m,
            categoryId: DataConstants.Categories.TShirts.Id
        );

        public static readonly Product GraphicTShirt = CreateProduct(
            name: "Graphic T-Shirt",
            sku: "TSHIRT-GRAPHIC",
            basePrice: 24.99m,
            weight: 0.3m,
            categoryId: DataConstants.Categories.TShirts.Id
        );

        // Apparel: Pants
        public static readonly Product Jeans = CreateProduct(
            name: "Blue Jeans",
            sku: "PANTS-JEANS",
            basePrice: 59.99m,
            weight: 0.7m,
            categoryId: DataConstants.Categories.Pants.Id
        );

        public static readonly Product Chinos = CreateProduct(
            name: "Chino Pants",
            sku: "PANTS-CHINOS",
            basePrice: 49.99m,
            weight: 0.6m,
            categoryId: DataConstants.Categories.Pants.Id
        );

        // Apparel: Dresses
        public static readonly Product SummerDress = CreateProduct(
            name: "Summer Dress",
            sku: "DRESS-SUMMER",
            basePrice: 69.99m,
            weight: 0.4m,
            categoryId: DataConstants.Categories.Dresses.Id
        );

        // Apparel: Hoodies
        public static readonly Product ZipHoodie = CreateProduct(
            name: "Zip Hoodie",
            sku: "HOODIE-ZIP",
            basePrice: 39.99m,
            weight: 0.8m,
            categoryId: DataConstants.Categories.Hoodies.Id
        );

        // Footwear: Sneakers
        public static readonly Product RunningSneakers = CreateProduct(
            name: "Running Sneakers",
            sku: "SNEAKERS-RUN",
            basePrice: 99.99m,
            weight: 0.9m,
            categoryId: DataConstants.Categories.Sneakers.Id
        );

        public static readonly Product CasualSneakers = CreateProduct(
            name: "Casual Sneakers",
            sku: "SNEAKERS-CASUAL",
            basePrice: 79.99m,
            weight: 0.8m,
            categoryId: DataConstants.Categories.Sneakers.Id
        );

        // Footwear: Boots
        public static readonly Product LeatherBoots = CreateProduct(
            name: "Leather Boots",
            sku: "BOOTS-LEATHER",
            basePrice: 129.99m,
            weight: 1.5m,
            categoryId: DataConstants.Categories.Boots.Id
        );

        // Footwear: Sandals
        public static readonly Product BeachSandals = CreateProduct(
            name: "Beach Sandals",
            sku: "SANDALS-BEACH",
            basePrice: 29.99m,
            weight: 0.4m,
            categoryId: DataConstants.Categories.Sandals.Id
        );

        // Footwear: FormalShoes
        public static readonly Product OxfordShoes = CreateProduct(
            name: "Oxford Shoes",
            sku: "FORMAL-OXFORD",
            basePrice: 109.99m,
            weight: 1.0m,
            categoryId: DataConstants.Categories.FormalShoes.Id
        );

        // Accessories: Bags
        public static readonly Product Backpack = CreateProduct(
            name: "Travel Backpack",
            sku: "BAG-BACKPACK",
            basePrice: 59.99m,
            weight: 1.0m,
            categoryId: DataConstants.Categories.Bags.Id
        );

        // Accessories: Belts
        public static readonly Product LeatherBelt = CreateProduct(
            name: "Leather Belt",
            sku: "BELT-LEATHER",
            basePrice: 34.99m,
            weight: 0.2m,
            categoryId: DataConstants.Categories.Belts.Id
        );

        // Accessories: Hats
        public static readonly Product BaseballCap = CreateProduct(
            name: "Baseball Cap",
            sku: "HAT-BASEBALL",
            basePrice: 24.99m,
            weight: 0.2m,
            categoryId: DataConstants.Categories.Hats.Id
        );

        // Accessories: Sunglasses
        public static readonly Product AviatorSunglasses = CreateProduct(
            name: "Aviator Sunglasses",
            sku: "SUNGLASS-AVI",
            basePrice: 49.99m,
            weight: 0.1m,
            categoryId: DataConstants.Categories.Sunglasses.Id
        );

        public static readonly List<Product> DefaultProducts = new()
        {
            LeatherJacket, DenimJacket,
            CottonTShirt, GraphicTShirt,
            Jeans, Chinos,
            SummerDress,
            ZipHoodie,
            RunningSneakers, CasualSneakers,
            LeatherBoots,
            BeachSandals,
            OxfordShoes,
            Backpack,
            LeatherBelt,
            BaseballCap,
            AviatorSunglasses
        };

        private static Product CreateProduct(
            string name,
            string sku,
            decimal basePrice,
            decimal weight,
            Guid categoryId)
        {
            var productResult = Product.Create(
                name: name,
                sku: sku,
                basePrice: basePrice,
                weight: weight,
                groupId: DataConstants.AttributeGroups.GeneralGroup.Id,
                taxClass: TaxClass.Standard,
                status: ProductStatus.Draft
            );

            var product = productResult.Value;
            product.AddCategory(categoryId);
            return product;
        }
    }
}

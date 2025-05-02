using ErrorOr;

namespace RuanFa.Shop.Domain.Accounts.Errors;

public static partial class DomainErrors
{
    public static class FashionPreferences
    {
        // Wishlist Errors
        public static Error WishlistRequired => Error.Validation(
            code: "FashionPreferences.WishlistRequired",
            description: "Wishlist is required.");

        // Clothing Size Errors
        public static Error ClothingSizeRequired => Error.Validation(
            code: "FashionPreferences.ClothingSizeRequired",
            description: "Clothing size is required.");

        public static Error InvalidClothingSize => Error.Validation(
            code: "FashionPreferences.InvalidClothingSize",
            description: "Clothing size is invalid.");

        // Category Errors
        public static Error FavoriteCategoriesRequired => Error.Validation(
            code: "FashionPreferences.FavoriteCategoriesRequired",
            description: "At least one favorite category is required.");

        public static Error InvalidCategoryName => Error.Validation(
            code: "FashionPreferences.InvalidCategoryName",
            description: "One or more category names are invalid.");
    }
}

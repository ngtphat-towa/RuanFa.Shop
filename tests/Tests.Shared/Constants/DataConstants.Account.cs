using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Application.Common.Security.Roles;
using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.Domain.Accounts.ValueObjects;
using RuanFa.Shop.Domain.Commons.Enums;
using RuanFa.Shop.Domain.Commons.ValueObjects;
using RuanFa.Shop.Infrastructure.Accounts.Entities;

namespace RuanFa.Shop.Tests.Shared.Constants;
public static partial class DataConstants
{
    public static class Accounts
    {
        // Idenitifiers
        public static readonly Guid AdminUserId = Guid.NewGuid();
        public static readonly Guid UserId = Guid.NewGuid();
        // Username 
        public const string Username = "user";
        public const string AdminUsername = "admin";
        // Email 
        public const string UserEmail = "user@example.com";
        public const string AdminEmail = "admin@example.com";
        // Password
        public const string Password = "User@123456!";
        // Permisions
        public static IReadOnlyList<string> AdminPermissions = [.. Permission.Administrator];
        public static IReadOnlyList<string> UserPermissions = [.. Permission.User];
        public static IReadOnlyList<string> AdminRoles = [Role.Administrator];
        public static IReadOnlyList<string> UserRoles = [Role.User];

        public static readonly ApplicationUser DefaultApplicationUser = new ApplicationUser
        {
            UserName = Username,
            Email = UserEmail,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "TestEnvironment",
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "TestEnvironment",
        };
        public static readonly ApplicationUser DefaultAdminUser = new ApplicationUser
        {
            UserName = AdminUsername,
            Email = AdminEmail,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "TestEnvironment",
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "TestEnvironment",
        };
    }
    public static class UserProfiles
    {
        // Basic Info
        public const string FullName = "John Doe";
        public const string PhoneNumber = "+1234567890";

        // Date of Birth
        public static readonly DateTimeOffset DateOfBirth = new(1990, 5, 20, 0, 0, 0, TimeSpan.Zero);

        // Gender
        public const GenderType Gender = GenderType.None;

        // Address
        public static readonly List<UserAddress> Addresses = [
            UserAddresses.ShippingAddress,
            UserAddresses.BillingAddress
        ];
        public static readonly FashionPreference StylePreferences =
            FashionPreference.Create(
               clothingSize: FashionPreferences.ClothingSize,
               favoriteCategories: FashionPreferences.FavoriteCategories);

        public static readonly UserProfile DefaultUserProfile =
           UserProfile.Create(
               userId: Accounts.UserId,
               email: Accounts.UserEmail,
               username: Accounts.Username,
               fullName: FullName,
               phoneNumber: PhoneNumber,
               gender: Gender,
               dateOfBirth: DateOfBirth,
               addresses: Addresses,
               preferences: StylePreferences,
               wishlist: FashionPreferences.Wishlist,
               loyaltyPoints: 250,
               marketingConsent: true
           ).Value;

        public static readonly UserProfile DefaultAdminProfile =
           UserProfile.Create(
               userId: Accounts.AdminUserId,
               email: Accounts.AdminEmail,
               username: Accounts.AdminUsername,
               fullName: FullName,
               phoneNumber: PhoneNumber,
               gender: Gender,
               dateOfBirth: DateOfBirth,
               addresses: Addresses,
               preferences: StylePreferences,
               wishlist: FashionPreferences.Wishlist,
               loyaltyPoints: 250,
               marketingConsent: true
           ).Value;
    }
    public static class FashionPreferences
    {
        public const string  ClothingSize = "M";
        public const string WishlistItem1 = "product-123";
        public const string WishlistItem2 = "product-456";
        public static readonly List<string> FavoriteCategories = new List<string>
        {
            "Shirts",
            "Jeans",
            "Sneakers",
            "Accessories"
        };

        public static readonly List<string> Wishlist = new()
        {
            WishlistItem1,
            WishlistItem2
        };
    }
}

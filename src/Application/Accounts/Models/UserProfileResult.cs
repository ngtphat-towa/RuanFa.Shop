using RuanFa.Shop.Application.Common.Models;

namespace RuanFa.Shop.Application.Accounts.Models;

public record UserProfileResult
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int Gender { get; set; } 
    public DateTime ? DateOfBirth { get; set; }
    public List<UserAddressResult>? Addresses { get; set; } 
    public FashionPreferencesResult? Preferences { get; set; }
    public List<string>? Wishlist { get; set; } 
    public int LoyaltyPoints { get; set; }
    public bool MarketingConsent { get; set; }
}

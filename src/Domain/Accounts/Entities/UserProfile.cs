using ErrorOr;
using RuanFa.Shop.Domain.Accounts.Events;
using RuanFa.Shop.Domain.Accounts.ValueObjects;
using RuanFa.Shop.Domain.Commons.Enums;
using RuanFa.Shop.Domain.Commons.ValueObjects;
using RuanFa.Shop.Domain.Orders;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;
using DomainErrors = RuanFa.Shop.Domain.Accounts.Errors.DomainErrors;

namespace RuanFa.Shop.Domain.Accounts.Entities;

public class UserProfile : Entity<Guid>
{
    #region Properties
    public string? UserId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public GenderType Gender { get; private set; } = GenderType.None;
    public DateTime? DateOfBirth { get; private set; }
    public List<UserAddress> Addresses { get; private set; } = new List<UserAddress>();
    public FashionPreferences Preferences { get; private set; } = new FashionPreferences();
    public List<string> Wishlist { get; private set; } = new List<string>();
    public int LoyaltyPoints { get; private set; }
    public bool MarketingConsent { get; private set; }
    public ICollection<Order>? Orders { get; private set; }
    #endregion

    #region Constructor
    private UserProfile()
    {
    }

    private UserProfile(
        string userId,
        string email,
        string fullName,
        string? phoneNumber,
        GenderType gender,
        DateTime? dateOfBirth,
        List<UserAddress> addresses,
        FashionPreferences preferences,
        List<string> wishlist,
        int loyaltyPoints,
        bool marketingConsent,
        ICollection<Order>? orders)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        Addresses = addresses ?? new List<UserAddress>();
        Preferences = preferences ?? new FashionPreferences();
        Wishlist = wishlist ?? new List<string>();
        LoyaltyPoints = loyaltyPoints;
        MarketingConsent = marketingConsent;
        Orders = orders;
    }
    #endregion

    #region Methods
    public static ErrorOr<UserProfile> Create(
        string? userId,
        string? email,
        string fullName,
        string? phoneNumber,
        GenderType gender,
        DateTime? dateOfBirth,
        List<UserAddress> addresses,
        FashionPreferences preferences,
        List<string> wishlist,
        int loyaltyPoints,
        bool marketingConsent,
        ICollection<Order>? orders = null)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(userId))
        {
            errors.Add(DomainErrors.UserProfile.InvalidUserId);
        }

        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
        {
            errors.Add(DomainErrors.UserProfile.InvalidEmailFormat);
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            errors.Add(DomainErrors.UserProfile.FullNameRequired);
        }

        if (errors.Count != 0)
        {
            return errors;
        }

        var profile = new UserProfile(
            userId!,
            email!,
            fullName,
            phoneNumber,
            gender,
            dateOfBirth,
            addresses,
            preferences,
            wishlist,
            loyaltyPoints,
            marketingConsent,
            orders);

        profile.AddDomainEvent(new UserProfileCreatedEvent(profile));
        return profile;
    }

    public ErrorOr<Updated> UpdatePersonalDetails(
        string email,
        string fullName,
        string? phoneNumber,
        GenderType gender,
        DateTime? dateOfBirth,
        bool marketingConsent)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(DomainErrors.UserProfile.EmailRequired);
        }
        if (!IsValidEmail(email))
        {
            errors.Add(DomainErrors.UserProfile.InvalidEmailFormat);
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            errors.Add(DomainErrors.UserProfile.FullNameRequired);
        }

        if (errors.Count != 0)
        {
            return errors;
        }

        Email = email;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        MarketingConsent = marketingConsent;

        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateAddresses(List<UserAddress> addresses)
    {
        Addresses = addresses ?? new List<UserAddress>();
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdatePreferences(FashionPreferences preferences)
    {
        Preferences = preferences ?? new FashionPreferences();
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateWishlist(List<string> wishlist)
    {
        Wishlist = wishlist ?? new List<string>();
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> AddLoyaltyPoints(int points)
    {
        if (points < 0)
        {
            return DomainErrors.UserProfile.InvalidPoints;
        }

        LoyaltyPoints += points;
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> RedeemLoyaltyPoints(int points)
    {
        if (points < 0 || points > LoyaltyPoints)
        {
            return DomainErrors.UserProfile.InvalidPoints;
        }

        LoyaltyPoints -= points;
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> AddOrder(Order order)
    {
        if (order == null)
        {
            return DomainErrors.UserProfile.InvalidOrder;
        }

        Orders ??= new List<Order>();
        Orders.Add(order);
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveOrder(Guid orderId)
    {
        if (Orders == null)
        {
            return DomainErrors.UserProfile.OrderNotFound;
        }

        var order = Orders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            return DomainErrors.UserProfile.OrderNotFound;
        }

        Orders.Remove(order);
        AddDomainEvent(new UserProfileUpdatedEvent(this));
        return Result.Updated;
    }

    public Updated SetAccount(string? userId = null)
    {
        UserId = userId;
        return Result.Updated;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}

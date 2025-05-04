using ErrorOr;
using RuanFa.Shop.Application.Accounts.Models;
using RuanFa.Shop.Application.Accounts.Services;
using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.Domain.Accounts.ValueObjects;
using RuanFa.Shop.Domain.Commons.Enums;
using RuanFa.Shop.Domain.Commons.ValueObjects;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Accounts.Authentication.Register;
public class RegisterCommand : ICommand<AccountInfoResult>
{
    public string Email { get;  set; } = string.Empty;
    public string? Username { get;  set; } = string.Empty;
    public string Password { get;  set; } = string.Empty;
    public string FullName { get;  set; } = string.Empty;
    public string? PhoneNumber { get;  set; }
    public GenderType Gender { get;  set; } = GenderType.None;
    public DateTimeOffset? DateOfBirth { get;  set; }
    public List<UserAddress> Addresses { get;  set; } = new List<UserAddress>();
    public FashionPreference? Preferences { get;  set; } = new FashionPreference();
    public List<string>? Wishlist { get;  set; } = new List<string>();
    public bool MarketingConsent { get;  set; }
}
internal class RegisterCommandHandler(IAccountService accountService)
    : ICommandHandler<RegisterCommand, AccountInfoResult>
{
    public async Task<ErrorOr<AccountInfoResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userProfileResult = UserProfile.Create(
             userId: null,
             email: request.Email,
             username: request.Username,
             fullName: request.FullName,
             phoneNumber: request.PhoneNumber,
             gender: request.Gender,
             dateOfBirth: request.DateOfBirth,
             addresses: request.Addresses,
             preferences: request.Preferences ?? new FashionPreference(),
             wishlist: request.Wishlist ?? new List<string>(),
             loyaltyPoints: 0,
             marketingConsent: request.MarketingConsent);

        if (userProfileResult.IsError)
        {
            return userProfileResult.Errors;
        }
        var createAccountResult = await accountService.CreateAccountAsync(
            userProfileResult.Value,
            request.Password,
            cancellationToken);

        return createAccountResult.IsError
            ? createAccountResult.Errors
            : createAccountResult.Value;
    }
}

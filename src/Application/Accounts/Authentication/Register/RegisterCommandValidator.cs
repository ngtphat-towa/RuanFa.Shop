using FluentValidation;
using RuanFa.Shop.Application.Accounts.Validation;
using RuanFa.Shop.Application.Common.Services;
using RuanFa.Shop.Domain.Accounts.Errors;
using RuanFa.Shop.Domain.Commons.Enums;

namespace RuanFa.Shop.Application.Accounts.Authentication.Register;

internal class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly UserAddressValidator _userAddressValidator;

    public RegisterCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        _userAddressValidator = new UserAddressValidator();

        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(DomainErrors.Account.EmailRequired.Description)
            .WithErrorCode(DomainErrors.Account.EmailRequired.Code)
            .EmailAddress()
            .WithMessage(DomainErrors.Account.InvalidEmailFormat.Description)
            .WithErrorCode(DomainErrors.Account.InvalidEmailFormat.Code);

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(DomainErrors.Account.PasswordRequired.Description)
            .WithErrorCode(DomainErrors.Account.PasswordRequired.Code)
            .MinimumLength(6)
            .WithMessage(DomainErrors.Account.PasswordTooShort.Description)
            .WithErrorCode(DomainErrors.Account.PasswordTooShort.Code)
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$")
            .WithMessage(DomainErrors.Account.InvalidPasswordFormat.Description)
            .WithErrorCode(DomainErrors.Account.InvalidPasswordFormat.Code);

        // FullName validation
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(DomainErrors.Account.FullNameRequired.Description)
            .WithErrorCode(DomainErrors.Account.FullNameRequired.Code)
            .MinimumLength(3)
            .WithMessage(DomainErrors.Account.FullNameTooShort.Description)
            .WithErrorCode(DomainErrors.Account.FullNameTooShort.Code);

        // PhoneNumber validation (optional)
        When(x => !string.IsNullOrEmpty(x.PhoneNumber), () => RuleFor(x => x.PhoneNumber)
                .Must(IsValidPhoneNumber)
                .WithMessage(DomainErrors.Account.InvalidPhoneNumber.Description)
                .WithErrorCode(DomainErrors.Account.InvalidPhoneNumber.Code));

        // Gender validation
        RuleFor(x => x.Gender)
            .NotEqual(GenderType.None)
            .WithMessage(DomainErrors.Account.GenderRequired.Description)
            .WithErrorCode(DomainErrors.Account.GenderRequired.Code);

        // DateOfBirth validation (optional)
        When(x => x.DateOfBirth.HasValue, () => RuleFor(x => x.DateOfBirth)
                .Must(dob => dob!.Value < dateTimeProvider.UtcNow.AddYears(-18) && dob.Value > dateTimeProvider.UtcNow.AddYears(-120))
                .WithMessage(DomainErrors.Account.InvalidDateOfBirth.Description)
                .WithErrorCode(DomainErrors.Account.InvalidDateOfBirth.Code));

        // Addresses validation
        RuleFor(x => x.Addresses)
            .NotEmpty()
            .WithMessage(DomainErrors.Address.AddressesRequired.Description)
            .WithErrorCode(DomainErrors.Address.AddressesRequired.Code)
            .Must(addresses => addresses.Any(a => a.IsDefault))
            .WithMessage("At least one address must be marked as default");

        RuleForEach(x => x.Addresses)
            .SetValidator(_userAddressValidator);

        // Preferences validation (optional)
        When(x => x.Preferences != null, () => RuleFor(x => x.Preferences!)
                .SetValidator(new FashionPreferencesValidator()));

        // Wishlist validation (optional)
        When(x => x.Wishlist != null && x.Wishlist.Any(), () => RuleFor(x => x.Wishlist)
                .NotEmpty()
                .WithMessage(DomainErrors.FashionPreferences.WishlistRequired.Description)
                .WithErrorCode(DomainErrors.FashionPreferences.WishlistRequired.Code));
    }

    private bool IsValidPhoneNumber(string? phoneNumber)
    {
        if (phoneNumber == null) return false;
        return phoneNumber.Length >= 10 && phoneNumber.All(char.IsDigit);
    }
}

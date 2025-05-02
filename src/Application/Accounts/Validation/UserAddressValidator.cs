using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;
using RuanFa.Shop.Domain.Commons.ValueObjects;

namespace RuanFa.Shop.Application.Accounts.Validation;

public class UserAddressValidator : AbstractValidator<UserAddress>
{
    private readonly AddressValidator _addressValidator;

    public UserAddressValidator()
    {
        _addressValidator = new AddressValidator();

        // Include base Address validation
        RuleFor(x => x)
            .SetValidator(_addressValidator);

        // Validate UserAddress specific properties
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage(DomainErrors.UserAddress.AddressTypeRequired.Description)
            .WithErrorCode(DomainErrors.UserAddress.AddressTypeRequired.Code);

        RuleFor(x => x.IsDefault)
            .Must(x => x == true || x == false)
            .WithMessage(DomainErrors.UserAddress.DefaultAddressFlagInvalid.Description)
            .WithErrorCode(DomainErrors.UserAddress.DefaultAddressFlagInvalid.Code);
    }
}

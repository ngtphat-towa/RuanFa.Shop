using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;
using RuanFa.Shop.Domain.Commons.ValueObjects;

namespace RuanFa.Shop.Application.Accounts.Validation;

internal class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithMessage(DomainErrors.Address.AddressLine1Required.Description)
            .WithErrorCode(DomainErrors.Address.AddressLine1Required.Code);

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage(DomainErrors.Address.CityRequired.Description)
            .WithErrorCode(DomainErrors.Address.CityRequired.Code);

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage(DomainErrors.Address.StateRequired.Description)
            .WithErrorCode(DomainErrors.Address.StateRequired.Code);

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage(DomainErrors.Address.CountryRequired.Description)
            .WithErrorCode(DomainErrors.Address.CountryRequired.Code);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .WithMessage(DomainErrors.Address.PostalCodeRequired.Description)
            .WithErrorCode(DomainErrors.Address.PostalCodeRequired.Code)
            .Matches(@"^\d{5}(?:[-\s]\d{4})?$")
            .WithMessage(DomainErrors.Address.InvalidPostalCodeFormat.Description)
            .WithErrorCode(DomainErrors.Address.InvalidPostalCodeFormat.Code);
    }
}

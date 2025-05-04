using System.Text.RegularExpressions;
using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;

namespace RuanFa.Shop.Application.Accounts.Authentication.Login;

internal class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        // Basic non-empty validation
        RuleFor(x => x.UserIdentifier)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(BeEmailOrUsername)
            .WithMessage(DomainErrors.Account.InvalidUserIdentifier.Description)
            .WithErrorCode(DomainErrors.Account.InvalidUserIdentifier.Code)
            .When(x => !string.IsNullOrEmpty(x.UserIdentifier));

        // Email format validation (when '@' is present)
        When(x => x.UserIdentifier.Contains('@'), () => RuleFor(x => x.UserIdentifier)
                .EmailAddress()
                .WithMessage(DomainErrors.Account.InvalidEmailFormat.Description)
                .WithErrorCode(DomainErrors.Account.InvalidEmailFormat.Code));

        // Username format validation (when no '@')
        When(x => !x.UserIdentifier.Contains('@'), () => RuleFor(x => x.UserIdentifier)
                .Must(IsValidUsername)
                .WithMessage(DomainErrors.Account.InvalidUsernameFormat.Description)
                .WithErrorCode(DomainErrors.Account.InvalidUsernameFormat.Code));

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(DomainErrors.Account.PasswordRequired.Description)
            .WithErrorCode(DomainErrors.Account.PasswordRequired.Code)
            .MinimumLength(6)
            .WithMessage(DomainErrors.Account.PasswordTooShort.Description)
            .WithErrorCode(DomainErrors.Account.PasswordTooShort.Code);
    }

    private bool IsValidUsername(string username)
    {
        return !string.IsNullOrWhiteSpace(username) &&
               username.Length >= 3 &&
               username.Length <= 20 &&
               Regex.IsMatch(username, @"^[a-zA-Z0-9_\.]+$");
    }

    private bool BeEmailOrUsername(string identifier)
    {
        return identifier.Contains('@') || IsValidUsername(identifier);
    }

}

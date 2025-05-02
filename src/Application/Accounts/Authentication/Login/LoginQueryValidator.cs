using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;

namespace RuanFa.Shop.Application.Accounts.Authentication.Login;

internal class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        // Basic non-empty validation
        RuleFor(x => x.UserIdentifier)
            .NotEmpty()
            .WithMessage(DomainErrors.Account.UsernameRequired.Description)
            .WithErrorCode(DomainErrors.Account.UsernameRequired.Code);

        // Email format validation (when '@' is present)
        When(x => x.UserIdentifier.Contains('@'), () => RuleFor(x => x.UserIdentifier)
                .Must(IsValidEmail)
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

    private bool IsValidEmail(string email)
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

    private bool IsValidUsername(string username)
    {
        return !string.IsNullOrWhiteSpace(username) &&
               username.Length >= 3 &&
               username.Length <= 30 &&
               username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.');
    }
}

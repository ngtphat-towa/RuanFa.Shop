using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;

namespace RuanFa.Shop.Application.Accounts.Passwords.Update;

internal class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        // Old password validation
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage(DomainErrors.PasswordUpdate.OldPasswordRequired.Description)
            .WithErrorCode(DomainErrors.PasswordUpdate.OldPasswordRequired.Code);

        // New password validation
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(DomainErrors.PasswordUpdate.NewPasswordRequired.Description)
            .WithErrorCode(DomainErrors.PasswordUpdate.NewPasswordRequired.Code)
            .MinimumLength(6)
            .WithMessage(DomainErrors.PasswordUpdate.NewPasswordTooShort.Description)
            .WithErrorCode(DomainErrors.PasswordUpdate.NewPasswordTooShort.Code)
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$")
            .WithMessage(DomainErrors.PasswordUpdate.InvalidPasswordFormat.Description)
            .WithErrorCode(DomainErrors.PasswordUpdate.InvalidPasswordFormat.Code)
            .NotEqual(x => x.OldPassword)
            .WithMessage(DomainErrors.PasswordUpdate.PasswordsCannotMatch.Description)
            .WithErrorCode(DomainErrors.PasswordUpdate.PasswordsCannotMatch.Code);
    }
}
